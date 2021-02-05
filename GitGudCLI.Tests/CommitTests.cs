using GitGudCLI.Modules;
using System;
using Xunit;

namespace GitGudCLI.Tests
{
    public class CommitTests
    {
        [Fact]
        public void ShouldValidateMessage()
        {
            CommitMessageLinter linter = new("[feature]{wip} This is a test");

            Assert.True(linter.IsValid);
        }
    }
}