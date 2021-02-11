using GitGudCLI.Modules;
using Xunit;

namespace GitGudCLI.Tests
{
	public class CommitTests
	{
		[Fact]
		public void TrueWithFullHeader()
		{
			CommitMessageLinter linter = new("[feature]{wip} This is a test");

			Assert.True(linter.IsValid);
		}
		
		[Fact]
		public void TrueWithNoFlags()
		{
			CommitMessageLinter linter = new("[feature] This is a test");

			Assert.True(linter.IsValid);
		}
		
		[Fact]
		public void TrueWithManyFlags()
		{
			CommitMessageLinter linter = new("[feature]{!!!/wip/db} This is a test");

			Assert.True(linter.IsValid);
		}
		
		[Fact]
		public void FalseWithNoSubject()
		{
			CommitMessageLinter linter = new("[feature]{wip}");

			Assert.False(linter.IsValid);
		}
		
		[Fact]
		public void TrueWithBody()
		{
			CommitMessageLinter linter = new("[feature]{wip} This is a test\n~~~\nIt works");

			Assert.True(linter.IsValid);
		}
		
		[Fact]
		public void TrueWithBodyAndFooter()
		{
			CommitMessageLinter linter = new(@"[feature]{!!!/wip} This is a test
			~~~
			This is a body
			~~~
			Closes: #123
			See also: #456, #789");

			Assert.True(linter.IsValid);
		}
	}
}