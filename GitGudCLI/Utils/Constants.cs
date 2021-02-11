namespace GitGudCLI.Utils
{
	public static class Constants
	{
		public const string ValidationRegex =
			@"^(?<tag>\[\w+\])?(?<flags>\{.+\})?\s(?<subject>.+)?\n?\s?\n?(?<body>.*)?\n?\s?\n?(?<closed_issues>Closes:\s.*)?\n?(?<see_also>See also:\s.*)?";

		public const string HeaderValidationRegex = @"^(?<tag>\[.+\])?(?<flags>\{.+\})?\s(?<subject>.+?)?";
		public const string FooterValidationRegex = @"^(?<closed_issues>Closes:\s.*)?\n?(?<see_also>See also:\s.*)";

		public static readonly string[] ValidCommitTags =
		{
			"feature",
			"change",
			"fix",
			"style",
			"refactor",
			"test",
			"docs",
			"chore",
			"misc",
			"merge"
		};

		public static readonly string[] CommitTagsDescriptions =
		{
			"A new feature and small additions",
			"Any changes on existing functionality",
			"A bugfix or hotfix",
			"Any change in styling, layout, css, design, etc",
			"Any code refactoring, cleanup, formatting, improvements in code style and readability",
			"Adding, changing or refactoring tests, with no production code change",
			"Changes in documentation, readme, guides, etc.",
			"Updating dependencies, package manager configs, build tasks, etc.",
			"Anything not covered by the above categories",
			"Special tag used only when merging pull requests, mainly used by the Flow Submodel"
		};

		public static readonly string[] ValidCommitFlags = {"!!!", "db", "api", "ux", "dpc", "rm", "wip"};

		public static readonly string[] CommitFlagsDescriptions =
		{
			"Breaking change - Significant changes in software architecture and/or logic, that affects existing code.",
			"Changes that require database structure or data to be updated",
			"Changes that modify the API usage, models or structure",
			"Change in user experience - Anything that needs the user to relearn to use a feature",
			"Deprecated - Commits with this flag deprecates existing code",
			"Code Removal - Means that this commit removes old/legacy/deprecated code",
			"Work In Progress -  Commits marked as WIP can never be merged"
		};

		public static readonly string[] ValidWorkingBranchTypes = {"wip/", "fix/", "chore/", "hotfix/"};

		public static readonly string[] ValidWorkingBranchTypeDescriptions =
		{
			"Receive direct commits and can be freely edited by the developers.",
			"Used to fix bugs or missing resources found in the master branch.",
			"Only used when updating dependencies, frameworks, build tasks and other updates required.",
			"A priority fix for when you find a serious bug in the stable branch that needs to be resolved ASAP."
		};
	}
}