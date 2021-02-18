using System;
using CommandLine;
using ConsoleHelper;
using GitGudCLI.Commands;
using GitGudCLI.Options;
using GitGudCLI.Response;
using GitGudCLI.Utils;

namespace GitGudCLI
{
	internal static class Program
	{
		private static GitHelper _gitHelper;

		private static int Main(string[] args)
		{
			_gitHelper = new GitHelper();
			
			Parser.Default.ParseArguments<CommitOptions, FlowOptions, ChangelogOptions>(args)
				.WithParsed<CommitOptions>(options => CommitCommands.Run(options, _gitHelper))
				.WithParsed<FlowOptions>(options => FlowCommands.Run(options, _gitHelper))
				.WithParsed<ChangelogOptions>(options => ChangelogCommands.Run(options));

			return 0;
		}
	}
}