using System.Collections.Generic;
using CommandLine;

namespace GitGudCLI.Options
{
	[Verb("commit", true, HelpText = "Simplify commiting")]
	public class CommitOptions
	{
		[Value(0, Required = true, Default = "quickadd", HelpText = @"The commit mode
quickadd   [q] : Tracks all files and commit them
plain      [p] : Can choose between a plain commit or 'commit -am'
fullcommit [f] : A full commit with body and footer
lint       [l] : Lint a commit
generate   [g] : Generate a valid commit message")] 
		public string Mode { get; set; }
		
		[Value(1, HelpText = "The commit message")] 
		public string Message { get; set; }
	}
}