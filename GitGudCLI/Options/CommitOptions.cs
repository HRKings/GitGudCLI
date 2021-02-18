using CommandLine;

namespace GitGudCLI.Options
{
	[Verb("commit", true, HelpText = "Simplify commiting")]
	public class CommitOptions
	{
		[Value(0, HelpText = "The commit message")] 
		public string Message { get; set; }
	}
}