using System;
using System.Linq;
using GitGudCLI.Modules;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;
using Spectre.Console;

namespace GitGudCLI.Commands.Utilities
{
	public static class CommitCommandUtils
	{
		public static string GenerateCommitMessage(bool completeCommit, string message)
		{
			var commitBody = string.Empty;
			string[] closedIssues = null;
			string[] seeAlso = null;
			
			if(string.IsNullOrWhiteSpace(message))
				message= AnsiConsole.Ask<string>("Enter the commit subject:");
			
			var tag = AnsiConsole.Prompt(
					new SelectionPrompt<string>()
						.Title("Select the commit tag:")
						.PageSize(10)
						.MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
						.AddChoices(Constants.CommitTagsWithDescriptions[..^1]))
				.Split(':', StringSplitOptions.TrimEntries)[0];
			
			var flags = AnsiConsole.Prompt(
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
			
			if (completeCommit)
			{
				SpectreHelper.WriteWrappedHeader("Enter the commit body (Optional):");
				commitBody =  AnsiConsole.Prompt(new TextPrompt<string>(">").AllowEmpty());

				SpectreHelper.WriteWrappedHeader("Enter the closed issues, comma separated (Optional):");
				closedIssues = AnsiConsole.Prompt(new TextPrompt<string>(">").AllowEmpty())
					?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

				SpectreHelper.WriteWrappedHeader("Enter the 'see also' issues, comma separated (Optional):");
				seeAlso = AnsiConsole.Prompt(new TextPrompt<string>(">").AllowEmpty())
					?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			}

			CommitMessageGenerator generator = new()
			{
				Tag = tag,
				Flags = flags,
				Subject = message,
				Body = commitBody,
				ClosedIssues = closedIssues,
				SeeAlso = seeAlso
			};

			return generator.GenerateValidCommitMessage().CommitMessage;
		}

		public static GitResponse Commit(GitHelper helper, bool addUntracked, bool stageTracked,
			bool isComplete, string message)
		{
			var response = helper.CanCommit(true, addUntracked);
			if (!response.Success)
				return response;
			
			var commitMessage = GenerateCommitMessage(isComplete, message);
			
			if (commitMessage is null)
				return new GitResponse(false, EnumGitResponse.GENERIC_ERROR, 
					"There was an error generating the commit message.");
			
			if (addUntracked)
				return helper.CommitFullAdd(commitMessage);
			
			return stageTracked ? helper.CommitAdd(commitMessage) : helper.Commit(commitMessage);
		}
	}
}