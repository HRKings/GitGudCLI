using System.Collections.Generic;
using CommandLine;

namespace GitGudCLI.Options
{
	[Verb("commit", true, HelpText = "Simplify commiting")]
	public class CommitOptions
	{
		[Value(0, Required = true, Default = "quickadd", HelpText = "The commit mode (quickadd/qa, commit/c, fullcommit/fc)")] 
		public string Mode { get; set; }
		
		[Value(1, HelpText = "The commit message")] 
		public string Message { get; set; }
		
		[Value(2, HelpText = "The commit body")] 
		public string Body { get; set; }
		
		[Value(3, HelpText = "The commit closed issues")] 
		public IEnumerable<string> ClosedIssues { get; set; }
		
		[Value(4, HelpText = "The commit see also issues")] 
		public IEnumerable<string> SeeAlso { get; set; }
	}
}