using CommandLine;

namespace GitGudCLI.Options
{
	[Verb("flow", HelpText = "GitGud Flow actions")]
	public class FlowOptions
	{
		[Value(0, Required = true, HelpText = @"The Flow action to execute:
fullinit : Initializes a repository with a master and stable branch
init     : Creates a stable branch off the master
start    : Start a working branch and checkout to it
publish  : Pushes a working branch to the origin
complete : Merges and deletes a local working branch")] 
		public string Action { get; set; }
		
		[Value(1, HelpText = "The branch name to use in the action")] 
		public string BranchName { get; set; }
	}
}