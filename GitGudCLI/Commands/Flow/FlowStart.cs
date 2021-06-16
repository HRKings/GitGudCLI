using System;
using System.ComponentModel;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Flow
{
	public class FlowStartSettings : CommandSettings
	{
		[Description("The branch name")]
		[CommandArgument(0, "[BranchName]")]
		public string BranchName { get; set; }
	}
	
	public class FlowStart : Command<FlowStartSettings>
	{
		public override int Execute(CommandContext context, FlowStartSettings options)
		{
			var gitHelper = new GitHelper();
			var flowHelper = new Modules.Flow(gitHelper);
			
			if (!gitHelper.HasRepo)
			{
				SpectreHelper.WriteError("There is no repository");
				return 1;
			}
			
			var type = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Select the branch type:")
						.PageSize(10)
						.MoreChoicesText("(Move up and down to reveal more branch types)")
						.AddChoices(Constants.ValidWorkingBranchTypeWithDescriptions))
				.Split(':', StringSplitOptions.TrimEntries)[0];

			var response = flowHelper.Start(options.BranchName, type);

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