using CommandLine;

namespace GitGudCLI.Options
{
	[Verb("flow", HelpText = "GitGud Flow actions")]
	public class FlowOptions
	{
		[Value(0, Required = true, HelpText = "The Flow action to execute (fullinit, init, start, publish, complete)")] 
		public string Action { get; set; }
		
		[Value(1, HelpText = "The branch name to use in the action")] 
		public string BranchName { get; set; }
	}
}