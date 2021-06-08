using GitGudCLI.Commands.Changelog;
using GitGudCLI.Commands.Commit;
using GitGudCLI.Commands.Flow;
using Spectre.Console.Cli;

namespace GitGudCLI
{
	internal static class Program
	{
		private static int Main(string[] args)
		{
			var app = new CommandApp();
			
			app.Configure(config =>
			{
				config.AddBranch("commit", commit =>
				{
					commit.AddCommand<CommitPlain>("plain")
						.WithDescription("Commit staged changes")
						.WithAlias("p");
					commit.AddCommand<CommitAdd>("add")
						.WithDescription("Performs a 'git commit -am'")
						.WithAlias("a");
					commit.AddCommand<CommitQuick>("quick")
						.WithDescription("Tracks all files and commit them")
						.WithAlias("q");
					commit.AddCommand<CommitLint>("lint")
						.WithDescription("Lint a commit");
					commit.AddCommand<CommitGenerateMessage>("generate")
						.WithDescription("Generate a valid commit message")
						.WithAlias("gen");
				});
				
				config.AddBranch("flow", flow =>
				{
					flow.AddCommand<FlowFullInit>("fullinit")
						.WithDescription("Initializes a repository with a master and stable branch");
					flow.AddCommand<FlowInit>("init")
						.WithDescription("Creates a stable branch off the master");
					flow.AddCommand<FlowStart>("start")
						.WithDescription("Start a working branch and checkout to it");
					flow.AddCommand<FlowPublish>("publish")
						.WithDescription("Pushes a working branch to the origin");
					flow.AddCommand<FlowComplete>("complete")
						.WithDescription("Merges and deletes a local working branch");
				});

				config.AddCommand<ChangelogGenerate>("changelog");
			});

			return app.Run(args);
		}
	}
}