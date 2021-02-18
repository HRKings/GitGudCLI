﻿using System;
using System.Linq;
using ConsoleHelper;
using GitGudCLI.Modules;
using GitGudCLI.Options;
using GitGudCLI.Response;
using GitGudCLI.Utils;
using Sharprompt;

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
				ColorConsole.WriteError("There is no repository");
				return 1;
			}
			
			_tag =  Prompt.Select("Select the commit tag: ", Constants.CommitTagsWithDescriptions[..^1])
				.Split(':', StringSplitOptions.TrimEntries)[0];
			_flags = Prompt.MultiSelect("Select the flags for this commit: ", 
					Constants.CommitFlagsWithDescriptions, pageSize: 7, minimum: 0)
				.OrderBy(flag => flag).Select(flag => flag.Split(':', StringSplitOptions.TrimEntries)[0]).ToArray();
			Console.Write(new string('\n', _flags.Length));
			
			if (string.IsNullOrWhiteSpace(options.Message))
				_options.Message = Prompt.Input<string>("Please provide a commit message:");
			
			switch (options.Mode)
			{
				case "quickadd":
				case "q":
					Commit(true, false);
					break;

				case "commit":
				case "c":
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
						ColorConsole.WriteError("There was an error generating the commit message.");
						return 1;
					}
						
					ColorConsole.WriteWrappedHeader("Your commit message:");
					ColorConsole.WriteInfo(message);
					return 0;
			}

			if (!_response.Success)
			{
				ColorConsole.WriteError(_response.Message);
				return 1;
			}

			ColorConsole.WriteSuccess($"{_response.Message}\nCommit made successfully.");
			return 0;
		}

		private static string GenerateCommitMessage(bool completeCommit, bool allowChanges, bool fullAdd, bool validate = true)
		{
			var response = _helper.CanCommit(allowChanges, fullAdd);
			if (validate && !response.Success)
			{
				ColorConsole.WriteError(response.Message);
				return string.Empty;
			}

			var commitBody = string.Empty;
			string[] closedIssues = null;
			string[] seeAlso = null;
			if (completeCommit)
			{
				ColorConsole.WriteWrappedHeader("Enter the commit body (Optional):");
				commitBody = Console.ReadLine();

				ColorConsole.WriteWrappedHeader("Enter the closed issues, comma separated (Optional):");
				closedIssues = Console.ReadLine()
					?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				ColorConsole.WriteWrappedHeader("Enter the 'see also' issues, comma separated (Optional):");
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
				commitMode = Prompt.Select("Commit command:", 
					new[] {"git commit -am", "git commit"}, defaultValue: "git commit -am");
			}

			string commitMessage = GenerateCommitMessage(isComplete, true, fullAdd);
			if (commitMessage is null)
			{
				ColorConsole.WriteError("There was an error generating the commit message.");
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