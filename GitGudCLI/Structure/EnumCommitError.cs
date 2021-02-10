using System;

namespace GitGudCLI.Structure
{
	[Flags]
	public enum EnumCommitError : byte
	{
		NONE = 0,
		NO_TAG = 1,
		NO_SUBJECT = 2,
		FLAG = 4,
		CLOSED_ISSUES = 8,
		SEE_ALSO = 16,
		INVALID_TAG = 32,
		INVALID_FLAG = 64
	}
}