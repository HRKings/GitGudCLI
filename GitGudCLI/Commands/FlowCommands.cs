using System;
using System.Linq;
using GitGudCLI.Modules;
using GitGudCLI.Options;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Spectre.Console;

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
				SpectreHelper.WriteError("There is no repository");
				return 1;
			}

			if (string.IsNullOrWhiteSpace(_options.BranchName) && _options.Action is "publish" or "complete")
				_options.BranchName = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Please select a branch:")
						.PageSize(10)
						.MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
						.AddChoices(helper.LocalBranches));
			
			if (string.IsNullOrWhiteSpace(_options.BranchName) && _options.Action is "start")
				_options.BranchName = AnsiConsole.Ask<string>("Please provide a branch name");

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
					SpectreHelper.WriteError($"Action '{_options.Action}' is not valid.");
					return 1;
			}

			if (!_response.Success)
			{
				SpectreHelper.WriteError(_response.Message);
				return 1;
			}
			
			if (_response.GitReponse is not EnumGitResponse.NONE)
			{
				SpectreHelper.WriteWarning(_response.Message);
				return 0;
			}
				
			SpectreHelper.WriteSuccess(_response.Message); 
			return 0;
		}

		private static void FullInit(GitHelper gitHelper)
		{
			var createResponse = gitHelper.CreateRepository();
			if (!createResponse.Success)
			{
				_response = new FlowResponse(createResponse.Success, EnumFlowResponse.GIT_ERROR,
					createResponse.Response, createResponse.Message);
				return;
			}

			_response = _flowHelper.Init();
		}

		private static void Start()
		{
			var type = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
					.Title("Select the branch type:")
					.PageSize(10)
					.MoreChoicesText("[grey](Move up and down to reveal more branch types)[/]")
					.AddChoices(Constants.ValidWorkingBranchTypeWithDescriptions))
				.Split(':', StringSplitOptions.TrimEntries)[0];

			_response = _flowHelper.Start(_options.BranchName, type);
		}
	}
}