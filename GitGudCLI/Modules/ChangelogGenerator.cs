using System.Collections.Generic;
using System.Linq;
using GitGudCLI.Utils;

namespace GitGudCLI.Modules
{
	public class ChangelogGenerator
	{
		public static string GenerateSince(string startDate)
		{
			var commits = GitHelper.GetCommitSubjectsSince(startDate).ToList();
			return commits.Count == 0 ? null : Generate(commits);
		}

		public static string Generate(IEnumerable<string> commitMessages)
		{
			var messageList = commitMessages?.ToList();
			
			if (messageList is null || messageList.Count == 0)
			{
				ColorConsole.WriteError("No commits were provided.");
				return null;
			}

			var lintedMessages = messageList.Select(message => new CommitMessageLinter(message)).ToList();
			
			var added = lintedMessages.Where(message => message.IsValid && message.Tag is "feature").Select(linter => linter.Subject).ToList();
			var changed = lintedMessages.Where(message => message.IsValid && message.Tag is "change").Select(linter => linter.Subject).ToList();
			var breakingChanges = lintedMessages.Where(message => message.IsValid && message.HasFlags && message.Flags.Contains("!!!")).Select(linter => linter.Subject).ToList();
			var fixes = lintedMessages.Where(message => message.IsValid && message.Tag is "fix").Select(linter => linter.Subject).ToList();
			var updated = lintedMessages.Where(message => message.IsValid && message.Tag is "chore").Select(linter => linter.Subject).ToList();
			var deprecated = lintedMessages.Where(message => message.IsValid && message.HasFlags && message.Flags.Contains("dpc")).Select(linter => linter.Subject).ToList();
			var removed = lintedMessages.Where(message => message.IsValid && message.HasFlags && message.Flags.Contains("rm")).Select(linter => linter.Subject).ToList();

			var changelog = string.Empty;

			if (added.Count != 0)
				changelog += $"### Added\n{string.Join('\n', added)}\n\n";
			
			if (changed.Count != 0)
				changelog += $"### Changed\n{string.Join('\n', changed)}\n\n";
			
			if (breakingChanges.Count != 0)
				changelog += $"### Breaking Changes\n{string.Join('\n', breakingChanges)}\n\n";
			
			if (fixes.Count != 0)
				changelog += $"### Fixed\n{string.Join('\n', fixes)}\n\n";
			
			if (fixes.Count != 0)
				changelog += $"### Fixed\n{string.Join('\n', fixes)}\n\n";
			
			if (updated.Count != 0)
				changelog += $"### Updated\n{string.Join('\n', updated)}\n\n";
			
			if (updated.Count != 0)
				changelog += $"### Deprecated\n{string.Join('\n', deprecated)}\n\n";
			
			if (removed.Count != 0)
				changelog += $"### Removed\n{string.Join('\n', removed)}\n\n";
			
			return changelog.TrimEnd('\n');
		}
	}
}