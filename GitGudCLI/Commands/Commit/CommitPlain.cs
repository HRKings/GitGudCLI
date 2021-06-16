using System;
using System.ComponentModel;
using System.Linq;
using GitGudCLI.Commands.Utilities;
using GitGudCLI.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Commit
{
	public class CommitPlain : Command<CommitBaseFullSettings>
	{
		public override int Execute(CommandContext context, CommitBaseFullSettings options)
			=> CommitBase.PerformCommit(options.CommitMessage, false, false, options.FullCommit, options.Amend);
	}
}