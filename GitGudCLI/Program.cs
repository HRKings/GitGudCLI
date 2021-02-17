using System;
using ConsoleHelper;
using GitGudCLI.Modules;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;

namespace GitGudCLI
{
	internal static class Program
	{
		private static GitHelper _gitHelper;

		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("HELP_TEXT");
				return;
			}

			_gitHelper = new GitHelper();

			switch (args[0])
			{
				case "commitadd":
				case "ca":
					CommitAdd(args.Length >= 2 ? args[1] : string.Empty);
					break;

				case "commit":
				case "c":
					QuickCommit(args.Length >= 2 ? args[1] : string.Empty);
					break;

				case "fullcommit":
				case "fc":
					FullCommit(args.Length >= 2 ? args[1] : string.Empty);
					break;

				case "commitlint":
				case "cmli":
					ValidateCommit(args.Length >= 2 ? args[1] : string.Empty);
					break;

				case "commitgen":
				case "cmgn":
					GenerateValidCommit(args.Length >= 2 ? args[1] : string.Empty);
					break;

				case "changelog":
				case "chlg":

					break;

				case "flow":
					switch (args.Length)
					{
						case 2:
							Flow(args[1], string.Empty);
							break;

						case 3:
							Flow(args[1], args[2]);
							break;

						default:
							ColorConsole.WriteError("Not enough arguments.");
							break;
					}

					break;

				default:
					Console.WriteLine($"Argument \"{args[0]}\" is not valid.");
					break;
			}
		}

		private static void CommitAdd(string commitMessage)
		{
			if (!_gitHelper.HasRepo)
			{
				ColorConsole.WriteError("There is no repository");
				return;
			}
			
			var response = _gitHelper.CanCommit(true);
			if (!response.Success)
			{
				ColorConsole.WriteError(response.Message);
				return;
			}

			ColorConsole.WriteWrappedHeader("Navigate the tags with the arrow keys and select using enter:");
			string tag = Constants.ValidCommitTags[
				CLIHelper.MenuChoice(false, Constants.CommitTagsDescriptions[..^1],
					Constants.ValidCommitTags[..^1])];

			ColorConsole.WriteWrappedHeader(
				"Navigate the flags with the arrow keys, select using enter and confirm with ESC (Optional):");
			var choices = CLIHelper.MultipleChoice(Constants.CommitFlagsDescriptions, Constants.ValidCommitFlags);
			choices.Sort();
			var flags = new string[choices.Count];
			for (var i = 0; i < choices.Count; i++) flags[i] = Constants.ValidCommitFlags[choices[i]];

			if (string.IsNullOrWhiteSpace(commitMessage))
			{
				ColorConsole.WriteWrappedHeader("Please provide a commit message:");
				commitMessage = Console.ReadLine();
			}

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags,
				Subject = commitMessage
			};

			var commit = generator.GenerateValidCommitMessage();

			if (commit is null)
			{
				ColorConsole.WriteError("There was an error generating the commit message.");
			}
			else
			{
				response = _gitHelper.CommitFullAdd(commit.CommitMessage);

				if (response.Success)
					ColorConsole.WriteSuccess($"{response.Message}\nCommit made successfully.");
				else
					ColorConsole.WriteError(response.Message);
			}
		}

		private static void QuickCommit(string commitMessage)
		{
			if (!_gitHelper.HasRepo)
			{
				ColorConsole.WriteError("There is no repository");
				return;
			}

			var response = _gitHelper.CanCommit();
			if (!response.Success)
			{
				ColorConsole.WriteError(response.Message);
				return;
			}

			ColorConsole.WriteWrappedHeader("Navigate the tags with the arrow keys and select using enter:");
			string tag = Constants.ValidCommitTags[
				CLIHelper.MenuChoice(false, Constants.CommitTagsDescriptions[..^1],
					Constants.ValidCommitTags[..^1])];

			ColorConsole.WriteWrappedHeader(
				"Navigate the flags with the arrow keys, select using enter and confirm with ESC (Optional):");
			var choices = CLIHelper.MultipleChoice(Constants.CommitFlagsDescriptions, Constants.ValidCommitFlags);
			choices.Sort();
			var flags = new string[choices.Count];
			for (var i = 0; i < choices.Count; i++) flags[i] = Constants.ValidCommitFlags[choices[i]];

			if (string.IsNullOrWhiteSpace(commitMessage))
			{
				ColorConsole.WriteWrappedHeader("Please provide a commit message:");
				commitMessage = Console.ReadLine();
			}

			ColorConsole.WriteWrappedHeader("Commit mode:");
			int commitAdd = CLIHelper.MenuChoice(false, new[] {"Equivalent to 'git commit -am", "A plain 'git commit'"},
				"Commit Add", "Just Commit");

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags,
				Subject = commitMessage
			};

			var commit = generator.GenerateValidCommitMessage();

			if (commit is null)
			{
				ColorConsole.WriteError("There was an error generating the commit message.");
			}
			else
			{
				response = commitAdd == 0 ? _gitHelper.CommitAdd(commit.CommitMessage) : _gitHelper.Commit(commit.CommitMessage);

				if (response.Success)
					ColorConsole.WriteSuccess($"{response.Message}\nCommit made successfully.");
				else
					ColorConsole.WriteError(response.Message);
			}
		}

		private static void FullCommit(string commitMessage)
		{
			if (!_gitHelper.HasRepo)
			{
				ColorConsole.WriteError("There is no repository");
				return;
			}

			var response = _gitHelper.CanCommit();
			if (!response.Success)
			{
				ColorConsole.WriteError(response.Message);
				return;
			}

			ColorConsole.WriteWrappedHeader("Navigate the tags with the arrow keys and select using enter:");
			string tag = Constants.ValidCommitTags[
				CLIHelper.MenuChoice(false, Constants.CommitTagsDescriptions[..^1],
					Constants.ValidCommitTags[..^1])];

			ColorConsole.WriteWrappedHeader(
				"Navigate the flags with the arrow keys, select using enter and confirm with ESC (Optional):");
			var choices = CLIHelper.MultipleChoice(Constants.CommitFlagsDescriptions, Constants.ValidCommitFlags);
			choices.Sort();
			var flags = new string[choices.Count];
			for (var i = 0; i < choices.Count; i++) flags[i] = Constants.ValidCommitFlags[choices[i]];

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
				Flags = flags,
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
				response = commitAdd == 0 ? _gitHelper.CommitAdd(commit.CommitMessage) : _gitHelper.Commit(commit.CommitMessage);

				if (response.Success)
					ColorConsole.WriteSuccess($"{response.Message}\nCommit made successfully.");
				else
					ColorConsole.WriteError(response.Message);
			}
		}

		private static void ValidateCommit(string commit)
		{
			if (string.IsNullOrWhiteSpace(commit))
			{
				Console.WriteLine("Please provide a commit message: ");
				commit = Console.ReadLine();
			}

			CommitMessageLinter toValidate = new(commit);

			toValidate.WriteReport();
		}

		private static void Flow(string argument1, string argument2)
		{
			Flow flow = new(_gitHelper);
			FlowResponse response = new();

			if (argument1 == "fullinit")
			{
				var createReponse = _gitHelper.CreateRepository();

				if (!createReponse.Success)
				{
					ColorConsole.WriteError(createReponse.Message);
					return;
				}

				response = flow.Init();

				if (response.Success)
					ColorConsole.WriteWarning(response.Message);
				else
					ColorConsole.WriteError(response.Message);

				return;
			}

			if (!_gitHelper.HasRepo)
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

		private static void GenerateValidCommit(string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				ColorConsole.WriteWrappedHeader("Please provide a commit subject: ");
				message = Console.ReadLine();
			}

			ColorConsole.WriteWrappedHeader("Navigate the tags with the arrow keys and select using enter:");

			string tag = Constants.ValidCommitTags[
				CLIHelper.MenuChoice(false, Constants.CommitTagsDescriptions[..^1],
					Constants.ValidCommitTags[..^1])];

			ColorConsole.WriteWrappedHeader(
				"Navigate the flags with the arrow keys, select using enter and confirm with ESC:");

			var choices = CLIHelper.MultipleChoice(Constants.CommitFlagsDescriptions, Constants.ValidCommitFlags);
			choices.Sort();
			var flags = new string[choices.Count];
			for (var i = 0; i < choices.Count; i++) flags[i] = Constants.ValidCommitFlags[choices[i]];

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags,
				Subject = message
			};

			ColorConsole.WriteWrappedHeader("Your commit message:");

			Console.WriteLine(generator.GenerateValidCommitMessage().CommitMessage);
		}
	}
}