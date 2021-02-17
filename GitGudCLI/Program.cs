using System;
using System.Collections.Immutable;
using System.Linq;
using ConsoleHelper;
using GitGudCLI.Modules;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Sharprompt;

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
					CLIMethods.QuickCommit(args.Length >= 2 ? args[1] : string.Empty, true, _gitHelper);
					break;

				case "commit":
				case "c":
					CLIMethods.QuickCommit(args.Length >= 2 ? args[1] : string.Empty, false, _gitHelper);
					break;

				case "fullcommit":
				case "fc":
					CLIMethods.FullCommit(args.Length >= 2 ? args[1] : string.Empty, _gitHelper);
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

			string tag =  Prompt.Select("Select the commit tag: ", Constants.CommitTagsWithDescriptions[..^1])
				.Split(':', StringSplitOptions.TrimEntries)[0];

			ColorConsole.WriteWrappedHeader(
				"Navigate the flags with the arrow keys, select using enter and confirm with ESC:");

			var flags = Prompt.MultiSelect("Select the flags for this commit: ", Constants.ValidCommitFlags, pageSize: 7)
				.Select(flag => flag.Split(':', StringSplitOptions.TrimEntries)[0]);

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags as string[],
				Subject = message
			};

			ColorConsole.WriteWrappedHeader("Your commit message:");

			Console.WriteLine(generator.GenerateValidCommitMessage().CommitMessage);
		}
	}
}