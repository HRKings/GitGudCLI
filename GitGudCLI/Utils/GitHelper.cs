using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GitGudCLI.Response;
using GitGudCLI.Structure;

namespace GitGudCLI.Utils
{
	public class GitHelper
	{
		private bool _needsRefresh;

		public GitHelper()
		{
			var status = GetStatus();
			if (status.Message.Contains("fatal: not a git repository"))
			{
				HasRepo = false;
				return;
			}

			HasRepo = true;
			_needsRefresh = true;
			Refresh();
		}

		public List<string> LocalBranches { get; private set; }
		public List<string> RemoteBranches { get; private set; }
		public List<string> Commits { get; private set; }

		public bool HasOrigin { get; private set; }
		public bool HasRepo { get; private set; }

		private void Refresh()
		{
			if (HasRepo && _needsRefresh)
			{
				LocalBranches = GetLocalBranches()?.ToList();
				Commits = GetAllCommitSubjects()?.ToList();
				RemoteBranches = GetRemoteBranches()?.ToList();

				HasOrigin = VerifyOrigin();

				_needsRefresh = false;
			}
		}

		public static GitResponse ExecuteGitCommand(string command)
		{
			var process = new Process();
			process.StartInfo.FileName = "git";
			process.StartInfo.Arguments = command;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.Start();

			// Synchronously read the standard output of the spawned process.
			var outputReader = process.StandardOutput;
			string output = outputReader.ReadToEnd();

			var errorReader = process.StandardError;
			string error = errorReader.ReadToEnd();

			process.WaitForExit();

			if (!string.IsNullOrWhiteSpace(error))
			{
				if (error.StartsWith("fatal"))
					return new GitResponse(false, EnumGitResponse.FATAL_ERROR, $"{output}{error}");

				if (error.StartsWith("error"))
					return new GitResponse(false, EnumGitResponse.GENERIC_ERROR, $"{output}{error}");
			}

			return new GitResponse(true, EnumGitResponse.NONE, $"{output}{error}");
		}

		public static IEnumerable<string> GetLocalBranches()
		{
			var output = ExecuteGitCommand(@"branch --format=""%(refname:short)""");
			if (!output.Success)
				return null;

			return output.Message.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}

		private static IEnumerable<string> GetRemoteBranches()
		{
			var output = ExecuteGitCommand(@"branch -r --format=""%(refname:short)""");
			if (!output.Success)
				return null;

			return output.Message.Replace("origin/", string.Empty)
				.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}

		public static IEnumerable<string> GetAllCommitSubjects(string branch = "")
		{
			var output = ExecuteGitCommand($@"log --no-merges --pretty=format:""%s"" {branch}");
			if (!output.Success)
				return null;

			return output.Message.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}

		public static IEnumerable<string> GetCommitSubjectsSince(string date, string branch = "")
		{
			var output = ExecuteGitCommand($@"log --since={date} --no-merges --pretty=format:""%s"" {branch}");
			if (!output.Success)
				return null;

			return output.Message.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}

		public static string GetLatestCommitSubject(string branch = "master")
		{
			var output = ExecuteGitCommand($@"log --no-merges --pretty=format:""%s"" -1 {branch}");
			if (!output.Success)
				return null;

			return output.Message;
		}

		public static bool VerifyOrigin()
		{
			var output = ExecuteGitCommand("config --get remote.origin.url");
			if (!output.Success)
				return false;

			return !string.IsNullOrWhiteSpace(output.Message);
		}

		public GitResponse CreateRepository()
		{
			if (HasRepo)
				return new GitResponse(false, EnumGitResponse.FATAL_ERROR, "Repository already exists.");

			var response = ExecuteGitCommand("init");
			if (!response.Success)
				return response;

			response = ExecuteGitCommand("checkout -b master");
			if (!response.Success)
				return response;

			response = ExecuteGitCommand(@"commit --allow-empty -m ""[misc] Master branch start""");
			if (!response.Success)
				return response;

			HasRepo = true;
			_needsRefresh = true;
			Refresh();

			return new GitResponse(true, EnumGitResponse.NONE, "Initialized the repository");
		}

		public GitResponse GetStatus()
		{
			Refresh();

			return ExecuteGitCommand("status");
		}

		public GitResponse CreateBranchChekout(string branchName)
		{
			Refresh();

			if (LocalBranches.Contains(branchName))
				return new GitResponse(false, EnumGitResponse.BRANCH_ALREADY_EXISTS,
					$"The branch {branchName} already exists.");

			var output = ExecuteGitCommand($"checkout -b {branchName}");

			if (output.Success)
				_needsRefresh = true;

			return new GitResponse(true, EnumGitResponse.NONE,
				$"Branch {branchName} created successfully\n{output.Message}");
		}

		public GitResponse CreateEmptyBranchChekout(string branchName)
		{
			Refresh();

			if (LocalBranches.Contains(branchName))
				return new GitResponse(false, EnumGitResponse.BRANCH_ALREADY_EXISTS,
					$"The branch {branchName} already exists.");

			var output = ExecuteGitCommand($"checkout --orphan {branchName}");

			if (output.Success)
				_needsRefresh = true;

			return new GitResponse(true, EnumGitResponse.NONE,
				$"{output.Message}\nBranch {branchName} created successfully.");
		}

		public GitResponse DeleteBranch(string branchName)
		{
			Refresh();

			if (!LocalBranches.Contains(branchName))
				return new GitResponse(false, EnumGitResponse.BRANCH_DOESNT_EXISTS,
					$"The branch {branchName} doesn't exists.");

			var output = ExecuteGitCommand($"branch -D {branchName}");

			if (output.Success)
				_needsRefresh = true;

			return output;
		}

		public GitResponse Checkout(string branchName)
		{
			Refresh();

			if (!LocalBranches.Contains(branchName))
				return new GitResponse(false, EnumGitResponse.BRANCH_DOESNT_EXISTS,
					$"The branch {branchName} doesn't exists.");

			var output = ExecuteGitCommand($"checkout {branchName}");

			return output;
		}

		public GitResponse Merge(string fromBranch, string toBranch)
		{
			Refresh();

			if (!LocalBranches.Contains(fromBranch))
				return new GitResponse(false, EnumGitResponse.BRANCH_DOESNT_EXISTS,
					$"The branch {fromBranch} doesn't exists.");

			if (!LocalBranches.Contains(toBranch))
				return new GitResponse(false, EnumGitResponse.BRANCH_DOESNT_EXISTS,
					$"The branch {toBranch} doesn't exists.");

			var response = Checkout(toBranch);
			if (!response.Success)
				return response;

			return ExecuteGitCommand($"merge {fromBranch}");
		}

		public GitResponse PushBranchToOrigin(string branchName)
		{
			Refresh();

			if (!HasOrigin)
				return new GitResponse(false, EnumGitResponse.REPOSITORY_HAS_NO_ORIGIN,
					"The repository has no origin.");

			if (!LocalBranches.Contains(branchName))
				return new GitResponse(false, EnumGitResponse.BRANCH_DOESNT_EXISTS,
					$"The branch {branchName} doesn't exists.");

			return ExecuteGitCommand($"push -u origin {branchName}");
		}

		public GitResponse CanCommit()
		{
			var output = GetStatus();

			if (output.Message.Contains("nothing to commit, working tree clean"))
				return new GitResponse(false, EnumGitResponse.GENERIC_ERROR, "Nothing to commit, working tree clean')");

			if (output.Message.Contains("no changes added to commit"))
				return new GitResponse(false, EnumGitResponse.GENERIC_ERROR,
					"No changes added to commit. (use 'git add' or 'git commit -am')");

			if (output.Message.Contains("nothing added to commit but untracked files present"))
				return new GitResponse(false, EnumGitResponse.GENERIC_ERROR,
					"Nothing added to commit, but untracked files are present (use 'git add')");

			return new GitResponse(true, EnumGitResponse.NONE, "There is changes to commit.");
		}

		public GitResponse Commit(string commitMessage)
		{
			var output = CanCommit();
			if (!output.Success)
				return output;

			output = ExecuteGitCommand($@"commit -m ""{commitMessage}""");

			if (output.Success)
				_needsRefresh = true;

			return output;
		}

		public GitResponse CommitAdd(string commitMessage)
		{
			var output = CanCommit();
			if (!output.Success)
				return output;

			output = ExecuteGitCommand($@"commit -am ""{commitMessage}""");

			if (output.Success)
				_needsRefresh = true;

			return output;
		}

		public GitResponse AddAllFiles()
		{
			var output = ExecuteGitCommand(@"add .");

			if (output.Success)
				_needsRefresh = true;

			return output;
		}
	}
}