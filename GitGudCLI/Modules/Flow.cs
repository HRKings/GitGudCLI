using ConsoleHelper;
using GitGudCLI.Structure;
using GitGudCLI.Response;
using GitGudCLI.Utils;
using System;
using System.Linq;

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
            if (!response.Success)
                return new(false, EnumFlowResponse.GIT_ERROR, response.Response, response.Message);

            return new(true, EnumFlowResponse.GIT_ERROR, response.Response, response.Message);
        }

        public FlowResponse Init()
        {
            if (_gitRepo.LocalBranches.Contains("stable"))
                return new(false, EnumFlowResponse.STABLE_ALREADY_EXISTS, EnumGitResponse.NONE, "The repository can't have two stable branches");

            if (!_gitRepo.LocalBranches.Contains("master"))
                return new(false, EnumFlowResponse.NONE, EnumGitResponse.BRANCH_DOESNT_EXISTS, "The repository doesn't have a master branch");

            var response = ParseGitResponse(_gitRepo.CreateEmptyBranchChekout("stable"));
            if (!response.Success)
                return response;

            _ = ParseGitResponse(GitHelper.ExecuteGitCommand("rm -rf ."));

            response = ParseGitResponse(GitHelper.ExecuteGitCommand(@"commit --allow-empty -m ""[misc] Stable branch start"""));
            if (!response.Success)
                return response;

            response = ParseGitResponse(_gitRepo.Checkout("master"));
            if (!response.Success)
                return response;

            response = ParseGitResponse(_gitRepo.PushBranchToOrigin("stable"));
            if (!response.Success)
            {
                if (response.GitReponse is EnumGitResponse.REPOSITORY_HAS_NO_ORIGIN)
                    return new(true, EnumFlowResponse.NONE, EnumGitResponse.REPOSITORY_HAS_NO_ORIGIN, "Initialized GitGud Flow successfully, but didn't push stable to origin.");

                return response;
            }

            return new(true, EnumFlowResponse.NONE, EnumGitResponse.NONE, "Initialized GitGud Flow successfully.");
        }

        public FlowResponse Start(string branchName, string workingType)
        {
            if (!Constants.VALID_WORKING_BRANCH_TYPES.Contains(workingType))
                return new(false, EnumFlowResponse.INVALID_WORKING_TYPE, EnumGitResponse.NONE,
                    $"The working type must be one of: {string.Join(", ", Constants.VALID_WORKING_BRANCH_TYPES)}.");

            if (branchName == "stable")
                return new(false, EnumFlowResponse.STABLE_ALREADY_EXISTS, EnumGitResponse.NONE, "The repository can't have two stable branches.");

            if (branchName == "master")
                return new(false, EnumFlowResponse.MASTER_ALREADY_EXISTS, EnumGitResponse.NONE, "The repository can't have two master branches.");

            if (_gitRepo.LocalBranches.Contains(branchName))
                return new(false, EnumFlowResponse.NONE, EnumGitResponse.BRANCH_ALREADY_EXISTS, $"The branch {branchName} already exists.");

            var response = ParseGitResponse(_gitRepo.Checkout("master"));
            if (!response.Success)
                return response;

            response = ParseGitResponse(_gitRepo.CreateBranchChekout($"{workingType}{branchName}"));
            if (!response.Success)
                return response;

            return new(true, EnumFlowResponse.NONE, EnumGitResponse.NONE, $"Started {workingType}{branchName} successfully.");
        }

        public FlowResponse Publish(string branchName)
        {
            var response = ParseGitResponse(_gitRepo.PushBranchToOrigin(branchName));
            if (!response.Success)
                return response;

            return new(true, EnumFlowResponse.NONE, EnumGitResponse.NONE, $"Published {branchName} successfully.");
        }

        public FlowResponse Complete(string branchName)
        {
            if (branchName == "stable")
                return new(false, EnumFlowResponse.CANNOT_COMPLETE_PERMANENT_BRANCHES, EnumGitResponse.NONE, "Permanent branches cannot be completed.");

            if (branchName == "master")
                return new(false, EnumFlowResponse.CANNOT_COMPLETE_PERMANENT_BRANCHES, EnumGitResponse.NONE, "Permanent branches cannot be completed.");

            if (_gitRepo.RemoteBranches.Contains(branchName))
                return new(false, EnumFlowResponse.REMOTE_BRANCHES_NEED_PULLREQUEST, EnumGitResponse.NONE, "This branch is on a remote, use a Pull Request to merge it.");

            if (branchName.StartsWith("hotfix/"))
            {
                var stableReponse = ParseGitResponse(_gitRepo.Merge(branchName, "stable"));
                if (!stableReponse.Success)
                    return stableReponse;
            }

            var response = ParseGitResponse(_gitRepo.Merge(branchName, "master"));
            if (!response.Success)
                return response;

            response = ParseGitResponse(_gitRepo.DeleteBranch(branchName));
            if (!response.Success)
                return response;

            return new(true, EnumFlowResponse.NONE, EnumGitResponse.NONE, $"Merged and deleted {branchName} successfully.");
        }
    }
}