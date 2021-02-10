namespace GitGudCLI.Structure
{
	public enum EnumGitResponse : byte
	{
		NONE = 0,
		GENERIC_ERROR = 1,
		FATAL_ERROR = 2,
		BRANCH_ALREADY_EXISTS = 3,
		BRANCH_DOESNT_EXISTS = 4,
		REPOSITORY_HAS_NO_ORIGIN = 5
	}
}