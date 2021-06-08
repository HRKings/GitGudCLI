using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Flow
{
	public class FlowInit : Command
	{
		public override int Execute(CommandContext context)
		{
			var gitHelper = new GitHelper();
			
			if (!gitHelper.HasRepo)
			{
				SpectreHelper.WriteError("There is no repository");
				return 1;
			}
			
			var flowHelper = new Modules.Flow(gitHelper);
			
			var response = flowHelper.Init();

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