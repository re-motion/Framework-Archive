using System;
using System.Text;
using NUnit.Framework;
using Rubicon.Text;

namespace Rubicon.Text.UnitTests
{

[TestFixture]
public class AsciiTextFormatTest
{
  [Test]
  public void TestSplitTextOnSeperator ()
  {
    AssertTextSplit ("12345 abcde",         10, "12345",        "abcde");
    AssertTextSplit ("1234567890 abcde",    10, "1234567890",   "abcde");
    AssertTextSplit ("12345678901 abcde",   10, "1234567890",   "1 abcde");
    AssertTextSplit ("1234 6789 bcde fghi", 10, "1234 6789",    "bcde fghi");
    AssertTextSplit ("1234567",             10, "1234567",      null);
    AssertTextSplit ("1234567890",          10, "1234567890",   null);
    AssertTextSplit ("12345678901",         10, "1234567890",   "1");
    AssertTextSplit ("",                     0, "",             null);
  }

  private void AssertTextSplit (string text, int splitAt, string expectedBefore, string expectedAfter)
  {
    string before;
    string after;
    AsciiTextFormat.SplitTextOnSeperator (text, out before, out after, splitAt, new char[] {' '});
    Assertion.AssertEquals (expectedBefore, before);
    Assertion.AssertEquals (expectedAfter, after);
  }

  [Test]
  public void TestAppendIndentedText()
  {
    string label = "this is the label  ";
    string description = "the quick brown fox jumps over the lazy dog. THE (VERY QUICK) FOX JUMPS OVER THE LAZY DOG.";
    StringBuilder sb = new StringBuilder (label);
    AsciiTextFormat.AppendIndentedText (sb, label.Length, 30, description);
    string expectedText = 
            "this is the label  the quick"
        + "\n                   brown fox"
        + "\n                   jumps over"
        + "\n                   the lazy"
        + "\n                   dog. THE"
        + "\n                   (VERY"
        + "\n                   QUICK) FOX"
        + "\n                   JUMPS OVER"
        + "\n                   THE LAZY"
        + "\n                   DOG.";
    Assertion.AssertEquals (expectedText, sb.ToString());
  }
}

}
