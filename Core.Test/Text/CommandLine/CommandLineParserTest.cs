using System;
using System.Runtime.Serialization;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.NullableValueTypes;
using Rubicon.UnitTests;

using Rubicon.Text.CommandLine;

namespace Rubicon.Text.CommandLine.UnitTests
{

[TestFixture]
public class CommandLineParserTest
{
  private enum IncrementalTestOptions { no, nor, normal, anything };
  private enum TestOption { yes, no, almost };

  private CommandLineParser CreateParser (
      out CommandLineStringArgument argSourceDir, 
      out CommandLineStringArgument argDestinationDir, 
      out CommandLineFlagArgument argCopyBinary,
      out CommandLineEnumArgument argEnumOption)
  {
    CommandLineParser parser = new CommandLineParser();

    argSourceDir = new CommandLineStringArgument (true);
    argSourceDir.Placeholder = "source-directory";
    argSourceDir.Description = "Directory to copy from";
    parser.Arguments.Add (argSourceDir);

    argDestinationDir = new CommandLineStringArgument (true);
    argDestinationDir.Placeholder = "destination-directory";
    argDestinationDir.Description = "This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to. This is the directory to copy to.";
    parser.Arguments.Add (argDestinationDir);

    argCopyBinary = new CommandLineFlagArgument ("b", true);
    argCopyBinary.Description = "binary copy on (+, default) or off (-)";
    parser.Arguments.Add (argCopyBinary);

    argEnumOption = new CommandLineEnumArgument ("rep", true, typeof (TestOption));
    argEnumOption.Description = "replace target";
    parser.Arguments.Add (argEnumOption);

    return parser;
  }

  private CommandLineParser CreateParser()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    return CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);
  }

  public void TestParsingSucceed ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Re:y" });

    Assertion.AssertEquals ("source", argSourceDir.Value);
    Assertion.AssertEquals ("dest", argDestinationDir.Value);
    Assertion.AssertEquals (NaBoolean.False, argCopyBinary.Value);
    Assertion.AssertEquals (true, argEnumOption.HasValue);
    Assertion.AssertEquals (TestOption.yes, argEnumOption.Value);
  }

  public void TestParsingLeaveOutOptional ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source"} );

    Assertion.AssertEquals ("source", argSourceDir.Value);
    Assertion.AssertEquals (null, argDestinationDir.Value);
    Assertion.AssertEquals (NaBoolean.True, argCopyBinary.Value);
    Assertion.AssertEquals (false, argEnumOption.HasValue);
  }

  [ExpectedException (typeof (MissingRequiredCommandLineParameterException))]
  public void TestParsingLeaveOutRequired ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);
    argEnumOption.IsOptional = false;

    parser.Parse (new string[] {
        "source"} );
  }

  [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
  public void TestParsingCaseSensitiveFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IsCaseSensitive = true;

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Re:y" });
  }

  [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
  public void TestParsingNotIncrementalFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IncrementalNameValidation = false;

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/b-",
        "/re:y" });
  }

  public void TestParsingNotIncrementalSucceed ()
  {
    CommandLineStringArgument argSourceDir;
    CommandLineStringArgument argDestinationDir;
    CommandLineFlagArgument argCopyBinary;
    CommandLineEnumArgument argEnumOption;
    CommandLineParser parser = CreateParser (out argSourceDir, out argDestinationDir, out argCopyBinary, out argEnumOption);

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "/B-",
        "/Rep:y" });

    Assertion.AssertEquals ("source", argSourceDir.Value);
    Assertion.AssertEquals ("dest", argDestinationDir.Value);
    Assertion.AssertEquals (NaBoolean.False, argCopyBinary.Value);
    Assertion.AssertEquals (TestOption.yes, argEnumOption.Value);
  }

  [ExpectedException (typeof (InvalidNumberOfCommandLineArgumentsException))]
  public void TestParsingTooManyPositionalFail ()
  {
    CommandLineParser parser = CreateParser ();
    parser.IncrementalNameValidation = false;

    parser.Parse (new string[] {
        "source", 
        "dest", 
        "another"} );
  }

  public void TestSynopsis ()
  {
    CommandLineParser parser = CreateParser();
    string synopsis = parser.GetAsciiSynopsis ("app.exe", 80);
    
    string expectedResult = 
        "app.exe [source-directory [destination-directory]] [/b-] [/rep{yes|no|almost}]" 
        + "\n"
        + "\n  source-directory       Directory to copy from" 
        + "\n  destination-directory  This is the directory to copy to. This is the directory" 
        + "\n                         to copy to. This is the directory to copy to. This is" 
        + "\n                         the directory to copy to. This is the directory to copy" 
        + "\n                         to. This is the directory to copy to. This is the" 
        + "\n                         directory to copy to. This is the directory to copy to." 
        + "\n  /b                     binary copy on (+, default) or off (-)" 
        + "\n  /rep                   replace target";

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