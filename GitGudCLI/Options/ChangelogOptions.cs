using CommandLine;

namespace GitGudCLI.Options
{
	[Verb("changelog", HelpText = "GitGud Flow actions")]
	public class ChangelogOptions
	{
		[Value(0,Required = true, HelpText = "The ISO formatted date (YYYY-MM-DD) from where to start the changelog.")]
		public string StartDate { get; set; }
	}
}