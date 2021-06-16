using System.ComponentModel;
using System.Text.RegularExpressions;
using GitGudCLI.Modules;
using GitGudCLI.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GitGudCLI.Commands.Changelog
{
	public class ChangelogGenerateOptions : CommandSettings
	{
		[Description("An ISO formatted date (YYYY-MM-dd) from where to start the changelog")]
		[CommandArgument(0, "[StartDate]")]
		public string StartDate { get; set; }

		public override ValidationResult Validate()
		{
			if (Regex.Match(StartDate, @"\d{4}-\d{2}-\d{2}").Success) 
				return ValidationResult.Success();
			
			return ValidationResult.Error("The date is invalid");
		}
	}
	
	public class ChangelogGenerate : Command<ChangelogGenerateOptions>
	{
		public override int Execute(CommandContext context, ChangelogGenerateOptions options)
		{
			var changelog = ChangelogGenerator.GenerateSince(options.StartDate);

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