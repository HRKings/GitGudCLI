using System;
using System.Linq;
using ConsoleHelper;
using GitGudCLI.Modules;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using Sharprompt;

namespace GitGudCLI.Utils
{
	public static class CLIMethods
	{
		public static void ValidateCommit(string commit)
		{
			if (string.IsNullOrWhiteSpace(commit))
			{
				Console.WriteLine("Please provide a commit message: ");
				commit = Console.ReadLine();
			}

			CommitMessageLinter toValidate = new(commit);

			toValidate.WriteReport();
		}
		
		public static void QuickCommit(string commitMessage, bool fullAdd, GitHelper helper)
		{
			if (!helper.HasRepo)
			{
				ColorConsole.WriteError("There is no repository");
				return;
			}
			
			var response = helper.CanCommit(true, true && fullAdd);
			if (!response.Success)
			{
				ColorConsole.WriteError(response.Message);
				return;
			}
			
			string tag =  Prompt.Select("Select the commit tag: ", Constants.CommitTagsWithDescriptions[..^1])
				.Split(':', StringSplitOptions.TrimEntries)[0];

			var flags = Prompt.MultiSelect("Select the flags for this commit: ", 
					Constants.CommitFlagsWithDescriptions, pageSize: 7, minimum: 0)
				.OrderBy(flag => flag).Select(flag => flag.Split(':', StringSplitOptions.TrimEntries)[0]);

			if (string.IsNullOrWhiteSpace(commitMessage))
			{
				ColorConsole.WriteWrappedHeader("Please provide a commit message:");
				commitMessage = Console.ReadLine();
			}

			var commitAdd = 0;
			if (!fullAdd)
			{
				ColorConsole.WriteWrappedHeader("Commit mode:");
				commitAdd = CLIHelper.MenuChoice(false, new[] {"Equivalent to 'git commit -am", "A plain 'git commit'"},
					"Commit Add", "Just Commit");
			}

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags as string[],
				Subject = commitMessage
			};

			var commit = generator.GenerateValidCommitMessage();

			if (commit is null)
			{
				ColorConsole.WriteError("There was an error generating the commit message.");
			}
			else
			{
				if (fullAdd)
				{
					response = helper.CommitFullAdd(commit.CommitMessage);
				}
				else
				{
					response = commitAdd == 0 ? helper.CommitAdd(commit.CommitMessage) : helper.Commit(commit.CommitMessage);
				}

				if (response.Success)
					ColorConsole.WriteSuccess($"{response.Message}\nCommit made successfully.");
				else
					ColorConsole.WriteError(response.Message);
			}
		}
		
		public static void FullCommit(string commitMessage, GitHelper helper)
		{
			if (!helper.HasRepo)
			{
				ColorConsole.WriteError("There is no repository");
				return;
			}

			var response = helper.CanCommit();
			if (!response.Success)
			{
				ColorConsole.WriteError(response.Message);
				return;
			}
			
			string tag =  Prompt.Select("Select the commit tag: ", Constants.CommitTagsWithDescriptions[..^1])
				.Split(':', StringSplitOptions.TrimEntries)[0];

			var flags = Prompt.MultiSelect("Select the flags for this commit: ", 
					Constants.CommitFlagsWithDescriptions, pageSize: 7, minimum: 0)
				.OrderBy(flag => flag).Select(flag => flag.Split(':', StringSplitOptions.TrimEntries)[0]);

			if (string.IsNullOrWhiteSpace(commitMessage))
			{
				ColorConsole.WriteWrappedHeader("Please provide a commit message:");
				commitMessage = Console.ReadLine();
			}

			ColorConsole.WriteWrappedHeader("Enter the commit body (Optional):");
			string commitBody = Console.ReadLine();

			ColorConsole.WriteWrappedHeader("Enter the closed issues, comma separated (Optional):");
			string[] closedIssues = Console.ReadLine()
				?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			ColorConsole.WriteWrappedHeader("Enter the 'see also' issues, comma separated (Optional):");
			string[] seeAlso = Console.ReadLine()
				?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			ColorConsole.WriteWrappedHeader("Commit mode:");
			int commitAdd = CLIHelper.MenuChoice(false, new[] {"Equivalent to 'git commit -am", "A plain 'git commit'"},
				"Commit Add", "Just Commit");

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags as string[],
				Subject = commitMessage,
				Body = commitBody,
				ClosedIssues = closedIssues,
				SeeAlso = seeAlso
			};

			var commit = generator.GenerateValidCommitMessage();

			if (commit is null)
			{
				ColorConsole.WriteError("There was an error generating the commit message.");
			}
			else
			{
				response = commitAdd == 0 ? helper.CommitAdd(commit.CommitMessage) : helper.Commit(commit.CommitMessage);

				if (response.Success)
					ColorConsole.WriteSuccess($"{response.Message}\nCommit made successfully.");
				else
					ColorConsole.WriteError(response.Message);
			}
		}
		
		public static void GenerateValidCommit(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				ColorConsole.WriteWrappedHeader("Please provide a commit subject: ");
				message = Console.ReadLine();
			}

			string tag =  Prompt.Select("Select the commit tag: ", Constants.CommitTagsWithDescriptions[..^1])
				.Split(':', StringSplitOptions.TrimEntries)[0];

			var flags = Prompt.MultiSelect("Select the flags for this commit: ", 
					Constants.CommitFlagsWithDescriptions, pageSize: 7, minimum: 0)
				.OrderBy(flag => flag).Select(flag => flag.Split(':', StringSplitOptions.TrimEntries)[0]);

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags as string[],
				Subject = message
			};

			ColorConsole.WriteWrappedHeader("Your commit message:");

			Console.WriteLine(generator.GenerateValidCommitMessage().CommitMessage);
		}
		
		public static void Flow(string argument1, string argument2, GitHelper helper)
		{
			Flow flow = new(helper);
			FlowResponse response = new();

			if (argument1 == "fullinit")
			{
				var createResponse = helper.CreateRepository();

				if (!createResponse.Success)
				{
					ColorConsole.WriteError(createResponse.Message);
					return;
				}

				response = flow.Init();

				if (response.Success)
					ColorConsole.WriteWarning(response.Message);
				else
					ColorConsole.WriteError(response.Message);

				return;
			}

			if (!helper.HasRepo)
			{
				ColorConsole.WriteError("There is no repository");
				return;
			}

			switch (argument1)
			{
				case "init":
					response = flow.Init();
					break;

				case "start":
					if (string.IsNullOrWhiteSpace(argument2))
					{
						ColorConsole.WriteWrappedHeader("Please provide a branch name:");
						argument2 = Console.ReadLine();
					}

					ColorConsole.WriteWrappedHeader(
						"Navigate the branch types with the arrow keys and select using enter:");
					string type = Constants.ValidWorkingBranchTypes[
						CLIHelper.MenuChoice(false, Constants.ValidWorkingBranchTypeDescriptions,
							Constants.ValidWorkingBranchTypes)];

					response = flow.Start(argument2, type);
					break;

				case "publish":
					if (string.IsNullOrWhiteSpace(argument2))
					{
						ColorConsole.WriteWrappedHeader("Please provide a branch name:");
						argument2 = Console.ReadLine();
					}

					response = flow.Publish(argument2);
					break;

				case "complete":
					if (string.IsNullOrWhiteSpace(argument2))
					{
						ColorConsole.WriteWrappedHeader("Please provide a branch name:");
						argument2 = Console.ReadLine();
					}

					response = flow.Complete(argument2);
					break;

				default:
					ColorConsole.WriteError("Flow command not found.");
					break;
			}

			if (response.Success)
			{
				if (response.GitReponse is EnumGitResponse.NONE)
					ColorConsole.WriteSuccess(response.Message);
				else
					ColorConsole.WriteWarning(response.Message);
			}
			else
			{
				ColorConsole.WriteError(response.Message);
			}
		}
	}
}