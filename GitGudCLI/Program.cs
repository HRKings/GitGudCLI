using CommandLine;
using GitGudCLI.Commands;
using GitGudCLI.Commands.Flow;
using GitGudCLI.Options;
using GitGudCLI.Utils;
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
			});

			return app.Run(args);
		}
	}
}