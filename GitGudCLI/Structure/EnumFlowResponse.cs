namespace GitGudCLI.Structure
{
    public enum EnumFlowResponse : byte
    {
        NONE = 0,
        GENERIC_ERROR = 1,
        GIT_ERROR = 2,
        STABLE_ALREADY_EXISTS = 3,
        MASTER_ALREADY_EXISTS = 4,
        INVALID_WORKING_TYPE = 5,
        CANNOT_COMPLETE_PERMANENT_BRANCHES = 6,
        REMOTE_BRANCHES_NEED_PULLREQUEST = 7
    }
}