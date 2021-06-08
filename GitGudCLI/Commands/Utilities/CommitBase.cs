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
		[CommandArgument(0, "[CommitMessage]")]
		public string CommitMessage { get; set; }
	}
	
	public class CommitBaseFullSettings : CommitBaseSettings
	{
		[Description("A full commit with body and footer")]
		[CommandOption("-f|--full")]
		public bool FullCommit { get; set; }
	}
	
	public static class CommitBase
	{
		public static int PerformCommit(string commitMessage, bool addUntracked, bool stageTracked, bool fullCommit)
		{
			var gitHelper = new GitHelper();
			
			if (!gitHelper.HasRepo)
			{
				SpectreHelper.WriteError("There is no repository");
				return 1;
			}
			
			var response = CommitCommandUtils.Commit(gitHelper, addUntracked, stageTracked,
				fullCommit, commitMessage);
			
			if (!response.Success)
			{
				SpectreHelper.WriteError(response.Message);
				return 1;
			}

			SpectreHelper.WriteSuccess($"{response.Message}\nCommit made successfully.");
			return 0;
		}
	}
}