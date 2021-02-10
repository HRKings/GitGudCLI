using System.Linq;
using GitGudCLI.Utils;

namespace GitGudCLI.Modules
{
	public class CommitMessageGenerator
	{
		public string Tag { get; set; }
		public string[] Flags { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string[] ClosedIssues { get; set; }
		public string[] SeeAlso { get; set; }

		public CommitMessageLinter GenerateValidCommitMessage()
		{
			if (!Constants.ValidCommitTags.Contains(Tag))
				return null;

			if (string.IsNullOrWhiteSpace(Subject))
				return null;

			var result = $@"[{Tag}]";

			if (Flags?.Length > 0)
			{
				foreach (string flag in Flags)
					if (!Constants.ValidCommitFlags.Contains(flag))
						return null;

				result += $@"{{{string.Join("}{", Flags)}}}";
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