using GitGudCLI.Structure;

namespace GitGudCLI.Response
{
    public struct GitResponse
    {
        public bool Success { get; init; }
        public EnumGitResponse Response { get; init; }
        public string Message { get; init; }

        public GitResponse(bool success, EnumGitResponse response, string message)
        {
            Success = success;
            Response = response;
            Message = message;
        }
    }
}