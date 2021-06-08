using System.ComponentModel;
using GitGudCLI.Commands.Utilities;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Commit
{
	public class CommitAdd : Command<CommitBaseFullSettings>
	{
		public override int Execute(CommandContext context, CommitBaseFullSettings options)
			=> CommitBase.PerformCommit(options.CommitMessage, false, true, options.FullCommit);
	}
}