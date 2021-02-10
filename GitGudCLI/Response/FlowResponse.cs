using GitGudCLI.Structure;

namespace GitGudCLI.Response
{
	public readonly struct FlowResponse
	{
		public bool Success { get; }
		public EnumFlowResponse Response { get; }
		public EnumGitResponse GitReponse { get; }
		public string Message { get; }

		public FlowResponse(bool success, EnumFlowResponse response, EnumGitResponse gitResponse, string message)
		{
			Success = success;
			Response = response;
			GitReponse = gitResponse;
			Message = message;
		}
	}
}