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
					CLIMethods.ValidateCommit(args.Length >= 2 ? args[1] : string.Empty);
					break;

				case "commitgen":
				case "cmgn":
					CLIMethods.GenerateValidCommit(args.Length >= 2 ? args[1] : string.Empty);
					break;

				case "changelog":
				case "chlg":

					break;

				case "flow":
					switch (args.Length)
					{
						case 2:
							CLIMethods.Flow(args[1], string.Empty, _gitHelper);
							break;

						case 3:
							CLIMethods.Flow(args[1], args[2], _gitHelper);
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
	}
}