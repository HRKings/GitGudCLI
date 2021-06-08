using System;
using System.ComponentModel;
using System.Linq;
using GitGudCLI.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Utilities
{
	public class CommitBaseSettings : CommandSettings
	{
		[Description("The commit message")]
		[CommandArgument(0, "[CommitMessage]")]
		public string CommitMessage { get; set; }
	}
	
	public class CommitBaseFullSettings : CommitBaseSettings
	{
		[Description("A full commit with body and footer")]
		[CommandOption("-f|--full")]
		public bool FullCommit { get; set; }
		
		[Description("Amend a previous commit")]
		[CommandOption("--amend")]
		public bool Amend { get; set; }
	}
	
	public static class CommitBase
	{
		public static int PerformCommit(string commitMessage, bool addUntracked, bool stageTracked, bool fullCommit, bool amend)
		{
			var gitHelper = new GitHelper();
			
			if (!gitHelper.HasRepo)
			{
				SpectreHelper.WriteError("There is no repository");
				return 1;
			}
			
			var response = CommitCommandUtils.Commit(gitHelper, addUntracked, stageTracked,
				fullCommit, commitMessage, amend);
			
			if (!response.Success)
			{
				SpectreHelper.WriteError(response.Message);
				return 1;
			}

			if (amend)
			{
				SpectreHelper.WriteSuccess($"{response.Message}\nCommit amend made successfully.");
				return 0;
			}

			SpectreHelper.WriteSuccess($"{response.Message}\nCommit made successfully.");
			return 0;
		}
	}
}