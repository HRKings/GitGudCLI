using System.Text.RegularExpressions;
using GitGudCLI.Modules;
using GitGudCLI.Options;

namespace GitGudCLI.Commands
{
	public static class ChangelogCommands
	{
		public static int Run(ChangelogOptions options)
		{
			if (!Regex.Match(options.StartDate, @"\d{4}-\d{2}-\d{2}").Success)
			{
				ColorConsole.WriteError("The date is invalid");
				return 1;
			}
			
			string changelog = ChangelogGenerator.GenerateSince(options.StartDate);

			if (string.IsNullOrWhiteSpace(changelog))
			{
				ColorConsole.WriteError("There were no commits to generate the changelog from.");
				return 1;
			}
			
			ColorConsole.WriteInfo(changelog);
			return 0;
		}
	}
}