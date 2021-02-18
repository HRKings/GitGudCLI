using System;
using ConsoleHelper;
using GitGudCLI.Modules;
using GitGudCLI.Options;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Sharprompt;

namespace GitGudCLI.Commands
{
	public static class FlowCommands
	{
		private static Flow _flowHelper;
		private static FlowResponse _response;
		private static FlowOptions _options;

		public static int Run(FlowOptions options, GitHelper helper)
		{
			_options = options;
			_flowHelper = new Flow(helper);
			_response = new FlowResponse();
			
			if (!helper.HasRepo && _options.Action is not "fullinit")
			{
				ColorConsole.WriteError("There is no repository");
				return 1;
			}
			
			if (string.IsNullOrWhiteSpace(_options.BranchName) && _options.Action is not "init" or "fullinit")
				_options.BranchName = Prompt.Input<string>("Please provide a branch name:");

			switch (_options.Action)
			{
				case "fullinit":
					FullInit(helper);
					break;
				
				case "init":
					_response = _flowHelper.Init();
					break;
				
				case "start":
					Start();
					break;
				
				case "publish":
					_response = _flowHelper.Publish(_options.BranchName);
					break;
				
				case "complete":
					_response = _flowHelper.Complete(_options.BranchName);
					break;
				
				default:
					ColorConsole.WriteError($"Action '{_options.Action}' is not valid.");
					return 1;
			}

			if (!_response.Success)
			{
				ColorConsole.WriteError(_response.Message);
				return 1;
			}

			if (_response.GitReponse is not EnumGitResponse.NONE)
			{
				ColorConsole.WriteWarning(_response.Message);
				return 0;
			}
				
			ColorConsole.WriteSuccess(_response.Message); 
			return 0;
		}

		private static void FullInit(GitHelper gitHelper)
		{
			var createResponse = gitHelper.CreateRepository();
			if (!createResponse.Success)
			{
				ColorConsole.WriteError(createResponse.Message);
				return;
			}

			_response = _flowHelper.Init();
		}

		private static void Start()
		{
			string type = Prompt.Select("Select the branch type: ", Constants.ValidWorkingBranchTypeWithDescriptions)
				.Split(':', StringSplitOptions.TrimEntries)[0];

			_response = _flowHelper.Start(_options.BranchName, type);
		}
	}
}