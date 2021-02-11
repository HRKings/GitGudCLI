using System.Linq;
using GitGudCLI.Utils;

namespace GitGudCLI.Modules
{
	public class CommitMessageGenerator
	{
		public string Tag { get; init; }
		public string[] Flags { get; init; }
		public string Subject { get; init; }
		public string Body { get; init; }
		public string[] ClosedIssues { get; init; }
		public string[] SeeAlso { get; init; }

		public CommitMessageLinter GenerateValidCommitMessage()
		{
			if (!Constants.ValidCommitTags.Contains(Tag))
				return null;

			if (string.IsNullOrWhiteSpace(Subject))
				return null;

			var result = $@"[{Tag}]";

			if (Flags?.Length > 0)
			{
				if (Flags.Any(flag => !Constants.ValidCommitFlags.Contains(flag)))
				{
					return null;
				}

				result += $@"{{{string.Join("/", Flags)}}}";
			}

			result += $" {Subject}";

			if (!string.IsNullOrWhiteSpace(Body))
				result += $"\n\n{Body}";

			if (ClosedIssues?.Length > 0)
				result += $"\n\nCloses: {string.Join(", ", ClosedIssues)}";

			if (ClosedIssues?.Length > 0)
				result += $"\nSee Also: {string.Join(", ", SeeAlso)}";

			return new CommitMessageLinter(result);
		}
	}
}