using GitGudCLI.Commands.Utilities;
using GitGudCLI.Modules;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Commit
{
	public class CommitLint : Command<CommitBaseSettings>
	{
		public override int Execute(CommandContext context, CommitBaseSettings settings)
		{
			CommitMessageLinter toValidate = new(settings.CommitMessage);
			toValidate.WriteReport();
			return 0;
		}
	}
}