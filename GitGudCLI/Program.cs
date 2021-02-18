using System;
using CommandLine;
using ConsoleHelper;
using GitGudCLI.Options;
using GitGudCLI.Utils;

namespace GitGudCLI
{
	internal static class Program
	{
		private static GitHelper _gitHelper;

		private static void Main(string[] args)
		{
			_gitHelper = new GitHelper();
			
			Parser.Default.ParseArguments<CommitOptions>(args)
				.WithParsed<CommitOptions>(options => CommitCommands.Run(options, _gitHelper));
				//.WithNotParsed(errors => ...);
			
			if (args.Length == 0)
			{
				Console.WriteLine("HELP_TEXT");
				return;
			}

			switch (args[0])
			{
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