using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Flow
{
	public class FlowFullInit : Command
	{
		public override int Execute(CommandContext context)
		{
			var gitHelper = new GitHelper();
			var flowHelper = new Modules.Flow(gitHelper);
			FlowResponse response;

			var createResponse = gitHelper.CreateRepository();

			if (!createResponse.Success)
			{
				response = new FlowResponse(createResponse.Success, EnumFlowResponse.GIT_ERROR,
					createResponse.Response, createResponse.Message);
			}
			else
			{
				response = flowHelper.Init();
			}

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