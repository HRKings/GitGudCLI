using System.Linq;
using GitGudCLI.Response;
using GitGudCLI.Structure;
using GitGudCLI.Utils;

namespace GitGudCLI.Modules
{
	public class Flow
	{
		private readonly GitHelper _gitRepo;

		public Flow(GitHelper repo)
		{
			_gitRepo = repo;
		}

		private static FlowResponse ParseGitResponse(GitResponse response)
		{
			return !response.Success
				? new FlowResponse(false, EnumFlowResponse.GIT_ERROR, response.Response, response.Message)
				: new FlowResponse(true, EnumFlowResponse.GIT_ERROR, response.Response, response.Message);
		}

		public FlowResponse Init()
		{
			if (_gitRepo.LocalBranches.Contains("stable"))
				return new FlowResponse(false, EnumFlowResponse.STABLE_ALREADY_EXISTS, EnumGitResponse.NONE,
					"The repository can't have two stable branches");

			if (!_gitRepo.LocalBranches.Contains("master"))
				return new FlowResponse(false, EnumFlowResponse.NONE, EnumGitResponse.BRANCH_DOESNT_EXISTS,
					"The repository doesn't have a master branch");

			var response = ParseGitResponse(_gitRepo.CreateBranchChekout("stable"));
			if (!response.Success)
				return response;
			
			response = ParseGitResponse(_gitRepo.Checkout("master"));
			if (!response.Success)
				return response;

			response = ParseGitResponse(_gitRepo.PushBranchToOrigin("stable"));
			if (response.Success)
				return new FlowResponse(true, EnumFlowResponse.NONE, EnumGitResponse.NONE,
					"Initialized GitGud Flow successfully.");
			
			if (response.GitReponse is EnumGitResponse.REPOSITORY_HAS_NO_ORIGIN)
				return new FlowResponse(true, EnumFlowResponse.NONE, EnumGitResponse.REPOSITORY_HAS_NO_ORIGIN,
					"Initialized GitGud Flow successfully, but didn't push stable to origin.");

			return response;

		}

		public FlowResponse Start(string branchName, string workingType)
		{
			if (!Constants.ValidWorkingBranchTypes.Contains(workingType))
				return new FlowResponse(false, EnumFlowResponse.INVALID_WORKING_TYPE, EnumGitResponse.NONE,
					$"The working type must be one of: {string.Join(", ", Constants.ValidWorkingBranchTypes)}.");

			switch (branchName)
			{
				case "stable":
					return new FlowResponse(false, EnumFlowResponse.STABLE_ALREADY_EXISTS, EnumGitResponse.NONE,
						"The repository can't have two stable branches.");
				case "master":
					return new FlowResponse(false, EnumFlowResponse.MASTER_ALREADY_EXISTS, EnumGitResponse.NONE,
						"The repository can't have two master branches.");
			}

			if (_gitRepo.LocalBranches.Contains(branchName))
				return new FlowResponse(false, EnumFlowResponse.NONE, EnumGitResponse.BRANCH_ALREADY_EXISTS,
					$"The branch {branchName} already exists.");

			var response = ParseGitResponse(_gitRepo.Checkout("master"));
			if (!response.Success)
				return response;

			response = ParseGitResponse(_gitRepo.CreateBranchChekout($"{workingType}{branchName}"));
			if (!response.Success)
				return response;

			return new FlowResponse(true, EnumFlowResponse.NONE, EnumGitResponse.NONE,
				$"Started {workingType}{branchName} successfully.");
		}

		public FlowResponse Publish(string branchName)
		{
			var response = ParseGitResponse(_gitRepo.PushBranchToOrigin(branchName));
			if (!response.Success)
				return response;

			return new FlowResponse(true, EnumFlowResponse.NONE, EnumGitResponse.NONE,
				$"Published {branchName} successfully.");
		}

		public FlowResponse Complete(string branchName)
		{
			switch (branchName)
			{
				case "stable":
					return new FlowResponse(false, EnumFlowResponse.CANNOT_COMPLETE_PERMANENT_BRANCHES,
						EnumGitResponse.NONE, "Permanent branches cannot be completed.");
				case "master":
					return new FlowResponse(false, EnumFlowResponse.CANNOT_COMPLETE_PERMANENT_BRANCHES,
						EnumGitResponse.NONE, "Permanent branches cannot be completed.");
			}

			if (_gitRepo.RemoteBranches.Contains(branchName))
				return new FlowResponse(false, EnumFlowResponse.REMOTE_BRANCHES_NEED_PULL_REQUEST, EnumGitResponse.NONE,
					"This branch is on a remote, use a Pull Request to merge it.");

			if (branchName.StartsWith("hotfix/"))
			{
				var stableResponse = ParseGitResponse(_gitRepo.Merge(branchName, "stable"));
				if (!stableResponse.Success)
					return stableResponse;
			}

			var response = ParseGitResponse(_gitRepo.Merge(branchName, "master"));
			if (!response.Success)
				return response;

			response = ParseGitResponse(_gitRepo.DeleteBranch(branchName));
			if (!response.Success)
				return response;

			return new FlowResponse(true, EnumFlowResponse.NONE, EnumGitResponse.NONE,
				$"Merged and deleted {branchName} successfully.");
		}
	}
}