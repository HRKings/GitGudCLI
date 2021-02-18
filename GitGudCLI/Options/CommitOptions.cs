using CommandLine;

namespace GitGudCLI.Options
{
	[Verb("commit", true, HelpText = "Simplify commiting")]
	public class CommitOptions
	{
		[Value(0, HelpText = "The commit mode (quickadd/qa, commit/c, fullcommit/fc)")] 
		public string Mode { get; set; }
		
		[Value(1, HelpText = "The commit message")] 
		public string Message { get; set; }
	}
}