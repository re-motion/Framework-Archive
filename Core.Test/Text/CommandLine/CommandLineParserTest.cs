using System;
using System.Runtime.Serialization;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.NullableValueTypes;
using Rubicon.UnitTests;

using Rubicon.Text.CommandLineParser;

namespace Rubicon.Text.CommandLineParser.UnitTests
{

[TestFixture]
public class CommandLineParserTest
{
  private enum IncrementalTestOptions { no, nor, normal, anything };
  private enum TestOption { yes, no, almost };

  private CommandLineParser CreateParser()
  {
    CommandLineParser parser = new CommandLineParser();

    CommandLineStringArgument argSourceDir = new CommandLineStringArgument (true);
    argSourceDir.Placeholder = "source-directory";
    argSourceDir.Description = "Directory to copy from";
    parser.Arguments.Add (argSourceDir);

    CommandLineStringArgument argDestinationDir = new CommandLineStringArgument (true);
    argDestinationDir.Placeholder = "destination-directory";
    argDestinationDir.Description = "This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to.";
    parser.Arguments.Add (argDestinationDir);

    CommandLineFlagArgument argCopyBinary = new CommandLineFlagArgument ("b", true);
    argCopyBinary.Description = "binary copy on (+, default) or off (-)";
    parser.Arguments.Add (argCopyBinary);

    CommandLineEnumArgument argEnumOption = new CommandLineEnumArgument ("r", true, typeof (TestOption));
    argEnumOption.Description = "replace target";
    parser.Arguments.Add (argEnumOption);

    return parser;
  }

  public void TestSynopsis ()
  {
    CommandLineParser parser = CreateParser();
    string synopsis = parser.GetAsciiSynopsis ("app.exe");
    
    string expectedResult = 
        "app.exe [source-directory [destination-directory]] [/b-] [/r{yes|no|almost}]" 
        + "\n"
        + "\n  source-directory       Directory to copy from" 
        + "\n  destination-directory  This is the directory to copy to. This is the directory" 
        + "\n                         to copy to. This is the directory to copy to. This is" 
        + "\n                         the directory to copy to. This is the directory to copy" 
        + "\n                         to. This is the directory to copy to. This is the" 
        + "\n                         directory to copy to. This is the directory to copy to." 
        + "\n  /b                     binary copy on (+, default) or off (-)" 
        + "\n  /r                     replace target";

    Assertion.AssertEquals (expectedResult, synopsis);
  }

  [Test]
  public void TestEnumValues ()
  {
    CommandLineEnumArgument enumArg;
    
    enumArg = new CommandLineEnumArgument (false, typeof (TestOption));
    PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "yes");
    Assertion.AssertEquals (TestOption.yes, (TestOption) enumArg.Value);

    enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));
    PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "no");
    Assertion.AssertEquals (IncrementalTestOptions.no, (IncrementalTestOptions) enumArg.Value);
  }

  [Test]
  public void TestInt32Values ()
  {
    CommandLineInt32Argument intArg;

    intArg = new CommandLineInt32Argument (true);
    PrivateInvoke.InvokeNonPublicMethod (intArg, "SetStringValue", "32");
    Assertion.AssertEquals ((NaInt32)32, intArg.Value);

    intArg = new CommandLineInt32Argument (true);
    PrivateInvoke.InvokeNonPublicMethod (intArg, "SetStringValue", " ");
    Assertion.AssertEquals (NaInt32.Null, intArg.Value);
  }

  [Test]
  [ExpectedException (typeof (InvalidCommandLineArgumentValueException))]
  public void TestEnumAmbiguous ()
  {
    try
    {
      CommandLineEnumArgument enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));
      PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "n");
    }
    catch (InvalidCommandLineArgumentValueException e)
    {
      Assertion.Assert (e.Message.IndexOf ("Ambiguous") >= 0);
      throw e;
    }
  }

  [Test]
  [ExpectedException (typeof (InvalidCommandLineArgumentValueException))]
  public void TestEnumInvalid ()
  {
    try
    {
      CommandLineEnumArgument enumArg = new CommandLineEnumArgument (false, typeof (IncrementalTestOptions));
      PrivateInvoke.InvokeNonPublicMethod (enumArg, "SetStringValue", "invalidvalue");
    }
    catch (InvalidCommandLineArgumentValueException e)
    {
      Assertion.Assert (e.Message.IndexOf ("Use one of") >= 0);
      throw e;
    }
  }
}

}