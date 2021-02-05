using GitGudCLI.Structure;

namespace GitGudCLI.Response
{
    public struct FlowResponse
    {
        public bool Success { get; init; }
        public EnumFlowResponse Response { get; init; }
        public EnumGitResponse GitReponse { get; init; }
        public string Message { get; init; }

        public FlowResponse(bool success, EnumFlowResponse response, EnumGitResponse gitResponse, string message)
        {
            Success = success;
            Response = response;
            GitReponse = gitResponse;
            Message = message;
        }
    }
}