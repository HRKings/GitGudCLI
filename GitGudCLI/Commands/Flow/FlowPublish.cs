using System;
using System.ComponentModel;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Flow
{
	public class FlowPublishSettings : CommandSettings
	{
		[Description("The branch name")]
		[CommandArgument(0, "[BranchName]")]
		public string BranchName { get; set; }
	}
	
	public class FlowPublish : Command<FlowPublishSettings>
	{
		public override int Execute(CommandContext context, FlowPublishSettings options)
		{
			var gitHelper = new GitHelper();
			
			if (!gitHelper.HasRepo)
			{
				SpectreHelper.WriteError("There is no repository");
				return 1;
			}
			
			if (string.IsNullOrWhiteSpace(options.BranchName))
				options.BranchName = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Please select a branch:")
						.PageSize(10)
						.MoreChoicesText("(Move up and down to reveal more branches)")
						.AddChoices(gitHelper.LocalBranches));
			
			var flowHelper = new Modules.Flow(gitHelper);
			
			var response = flowHelper.Publish(options.BranchName);

			if (!response.Success)
			{
				SpectreHelper.WriteError(response.Message);
				return 1;
			}

			if (response.GitReponse is not EnumGitResponse.NONE)
			{
				SpectreHelper.WriteWarning(response.Message);
				return 0;
			}

			SpectreHelper.WriteSuccess(response.Message);
			return 0;
		}
	}
}