using System;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.Text.CommandLine;

namespace Rubicon.Core.UnitTests.Text.CommandLine
{
  public class Arguments
  {
    [CommandLineStringArgument (true, Placeholder = "source-directory", Description = "Directory to copy from")]
    public string SourceDirectory;

    [CommandLineStringArgument (true, Placeholder = "destination-directory", Description = "This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to.")]
    public string DestinationDirectory;

    [CommandLineFlagArgument ("b", true, Description = "binary copy on (+, default) or off (-)")]
    public bool CopyBinary;

    [CommandLineEnumArgument ("rep", true)]
    public TestOption ReplaceTarget;
  }

  [TestFixture]
	public class CommandLineClassParserTest
	{
    [Test] 
    public void TestSerializer ()
    {
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      Arguments arguments = (Arguments) parser.Parse ("sdir ddir /b- /rep:y", true);
      Assertion.AssertEquals ("sdir", arguments.SourceDirectory);
      Assertion.AssertEquals ("ddir", arguments.DestinationDirectory);
      Assertion.AssertEquals (false, arguments.CopyBinary);
      Assertion.AssertEquals (TestOption.yes, arguments.ReplaceTarget);
    }
  }
}
