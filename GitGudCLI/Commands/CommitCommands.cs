﻿using System;
using System.Linq;
using GitGudCLI.Modules;
using GitGudCLI.Options;
using GitGudCLI.Response;
using GitGudCLI.Utils;
using Spectre.Console;

namespace GitGudCLI.Commands
{
	public static class CommitCommands
	{
		private static CommitOptions _options;
		private static GitHelper _helper;
		private static GitResponse _response;

		private static string _tag;
		private static string[] _flags;

		public static int Run(CommitOptions options, GitHelper helper)
		{
			_options = options;
			_helper = helper;
			
			if (!helper.HasRepo)
			{
				SpectreHelper.WriteError("There is no repository");
				return 1;
			}
			
			_tag = AnsiConsole.Prompt(
				new SelectionPrompt<string>()
					.Title("Select the commit tag:")
					.PageSize(10)
					.MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
					.AddChoices(Constants.CommitTagsWithDescriptions[..^1]))
				.Split(':', StringSplitOptions.TrimEntries)[0];
			
			_flags = AnsiConsole.Prompt(
				new MultiSelectionPrompt<string>()
					.Title("Select the flags for this commit:")
					.NotRequired()
					.PageSize(10)
					.MoreChoicesText("[grey](Move up and down to reveal more flags)[/]")
					.InstructionsText(
						"[grey](Press [blue]<space>[/] to toggle a flag, " + 
						"[green]<enter>[/] to accept)[/]")
					.AddChoices(Constants.CommitFlagsWithDescriptions))
				.OrderBy(flag => flag)
				.Select(flag => flag.Split(':', StringSplitOptions.TrimEntries)[0])
				.ToArray();

			if (string.IsNullOrWhiteSpace(options.Message))
				_options.Message = AnsiConsole.Ask<string>("Please provide a commit message:");
			
			switch (options.Mode)
			{
				case "quickadd":
				case "q":
					Commit(true, false);
					break;

				case "plain":
				case "p":
					Commit(false, false);
					break;

				case "fullcommit":
				case "f":
					Commit(false, true);
					break;

				case "lint":
				case "l":
					CommitMessageLinter toValidate = new(_options.Message);
					toValidate.WriteReport();
					return 0;

				case "generate":
				case "g":
					string message = GenerateCommitMessage(false, false, false, false);
					if (string.IsNullOrWhiteSpace(message))
					{
						SpectreHelper.WriteError("There was an error generating the commit message.");
						return 1;
					}
						
					SpectreHelper.WriteWrappedHeader("Your commit message:");
					SpectreHelper.WriteInfo(message);
					return 0;
				
				default:
					SpectreHelper.WriteError($"The mode '{options.Mode}' is not valid.");
					return 1;
			}

			if (!_response.Success)
			{
				SpectreHelper.WriteError(_response.Message);
				return 1;
			}

			SpectreHelper.WriteSuccess($"{_response.Message}\nCommit made successfully.");
			return 0;
		}

		private static string GenerateCommitMessage(bool completeCommit, bool allowChanges, bool fullAdd, bool validate = true)
		{
			var response = _helper.CanCommit(allowChanges, fullAdd);
			if (validate && !response.Success)
			{
				SpectreHelper.WriteError(response.Message);
				return string.Empty;
			}

			var commitBody = string.Empty;
			string[] closedIssues = null;
			string[] seeAlso = null;
			if (completeCommit)
			{
				SpectreHelper.WriteWrappedHeader("Enter the commit body (Optional):");
				commitBody = Console.ReadLine();

				SpectreHelper.WriteWrappedHeader("Enter the closed issues, comma separated (Optional):");
				closedIssues = Console.ReadLine()
					?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				SpectreHelper.WriteWrappedHeader("Enter the 'see also' issues, comma separated (Optional):");
				seeAlso = Console.ReadLine()
					?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			}
			
			CommitMessageGenerator generator = new()
			{
				Tag = _tag,
				Flags = _flags,
				Subject = _options.Message,
				Body = commitBody,
				ClosedIssues = closedIssues,
				SeeAlso = seeAlso
			};

			return generator.GenerateValidCommitMessage().CommitMessage;
		}
		
		private static void Commit(bool fullAdd, bool isComplete)
		{
			var commitMode = string.Empty;
			if (!fullAdd)
			{
				commitMode = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Commit command:")
						.PageSize(10)
						.MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
						.AddChoices("git commit -m", "git commit -am"));
			}

			var commitMessage = GenerateCommitMessage(isComplete, true, fullAdd);
			if (commitMessage is null)
			{
				SpectreHelper.WriteError("There was an error generating the commit message.");
				return;
			}
			
			if (fullAdd)
			{
				_response = _helper.CommitFullAdd(commitMessage);
				return;
			}
			
			_response = commitMode is "git commit -am" ? _helper.CommitAdd(commitMessage) : _helper.Commit(commitMessage);
		}
	}
}