using System;

namespace GitGudCLI.Structure
{
    [Flags]
    public enum EnumCommitWarning : byte
    {
        NONE = 0,
        SUBJECT_TOO_LONG = 1,
        BREAKING_CHANGE_IS_NOT_THE_FIRST_FLAG = 2
    }
}