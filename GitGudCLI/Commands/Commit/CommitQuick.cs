using GitGudCLI.Commands.Utilities;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Commit
{
	public class CommitQuick : Command<CommitBaseFullSettings>
	{
		public override int Execute(CommandContext context, CommitBaseFullSettings options)
			=> CommitBase.PerformCommit(options.CommitMessage, true, false, options.FullCommit);
	}
}