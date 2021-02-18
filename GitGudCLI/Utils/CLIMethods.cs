using System;
using System.Linq;
using ConsoleHelper;
using GitGudCLI.Modules;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using Sharprompt;

namespace GitGudCLI.Utils
{
	public static class CLIMethods
	{
		public static void Flow(string argument1, string argument2, GitHelper helper)
		{
			Flow flow = new(helper);
			FlowResponse response = new();

			if (argument1 == "fullinit")
			{
				var createResponse = helper.CreateRepository();

				if (!createResponse.Success)
				{
					ColorConsole.WriteError(createResponse.Message);
					return;
				}

				response = flow.Init();

				if (response.Success)
					ColorConsole.WriteWarning(response.Message);
				else
					ColorConsole.WriteError(response.Message);

				return;
			}

			if (!helper.HasRepo)
			{
				ColorConsole.WriteError("There is no repository");
				return;
			}

			switch (argument1)
			{
				case "init":
					response = flow.Init();
					break;

				case "start":
					if (string.IsNullOrWhiteSpace(argument2))
						argument2 = Prompt.Input<string>("Please provide a branch name:");

					string type = Prompt.Select("Select the branch type: ", Constants.ValidWorkingBranchTypeWithDescriptions)
						.Split(':', StringSplitOptions.TrimEntries)[0];

					response = flow.Start(argument2, type);
					break;

				case "publish":
					if (string.IsNullOrWhiteSpace(argument2))
						argument2 = Prompt.Input<string>("Please provide a branch name:");

					response = flow.Publish(argument2);
					break;

				case "complete":
					if (string.IsNullOrWhiteSpace(argument2))
						argument2 = Prompt.Input<string>("Please provide a branch name:");
					

					response = flow.Complete(argument2);
					break;

				default:
					ColorConsole.WriteError("Flow command not found.");
					break;
			}

			if (response.Success)
			{
				if (response.GitReponse is EnumGitResponse.NONE)
					ColorConsole.WriteSuccess(response.Message);
				else
					ColorConsole.WriteWarning(response.Message);
			}
			else
			{
				ColorConsole.WriteError(response.Message);
			}
		}
	}
}