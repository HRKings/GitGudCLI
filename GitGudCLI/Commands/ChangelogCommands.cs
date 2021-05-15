using System.Text.RegularExpressions;
using GitGudCLI.Modules;
using GitGudCLI.Options;
using GitGudCLI.Utils;

namespace GitGudCLI.Commands
{
	public static class ChangelogCommands
	{
		public static int Run(ChangelogOptions options)
		{
			if (!Regex.Match(options.StartDate, @"\d{4}-\d{2}-\d{2}").Success)
			{
				SpectreHelper.WriteError("The date is invalid");
				return 1;
			}
			
			string changelog = ChangelogGenerator.GenerateSince(options.StartDate);

			if (string.IsNullOrWhiteSpace(changelog))
			{
				SpectreHelper.WriteError("There were no commits to generate the changelog from.");
				return 1;
			}
			
			SpectreHelper.WriteInfo(changelog);
			return 0;
		}
	}
}