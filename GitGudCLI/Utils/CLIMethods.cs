using System;
using System.Linq;
using ConsoleHelper;
using GitGudCLI.Modules;
using Sharprompt;

namespace GitGudCLI.Utils
{
	public static class CLIMethods
	{
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

			var flags = Prompt.MultiSelect("Select the flags for this commit: ", Constants.CommitFlagsWithDescriptions, pageSize: 7, minimum: 0)
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

			var flags = Prompt.MultiSelect("Select the flags for this commit: ", Constants.CommitFlagsWithDescriptions, pageSize: 7, minimum: 0)
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
	}
}