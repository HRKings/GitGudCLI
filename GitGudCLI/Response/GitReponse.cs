using GitGudCLI.Structure;

namespace GitGudCLI.Response
{
	public readonly struct GitResponse
	{
		public bool Success { get; }
		public EnumGitResponse Response { get; }
		public string Message { get; }

		public GitResponse(bool success, EnumGitResponse response, string message)
		{
			Success = success;
			Response = response;
			Message = message;
		}
	}
}