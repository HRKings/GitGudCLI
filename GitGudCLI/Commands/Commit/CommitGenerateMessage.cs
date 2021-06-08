using GitGudCLI.Commands.Utilities;
using GitGudCLI.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Commit
{
	public class CommitGenerateMessage : Command
	{
		public override int Execute(CommandContext context)
		{
			var subject = AnsiConsole.Ask<string>("Enter the commit subject:");
			var message = CommitCommandUtils.GenerateCommitMessage(true, subject);
			
			if (string.IsNullOrWhiteSpace(message))
			{
				SpectreHelper.WriteError("There was an error generating the commit message.");
				return 1;
			}

			SpectreHelper.WriteWrappedHeader("Your commit message:");
			SpectreHelper.WriteInfo(message);
			return 0;
		}
	}
}