using System;
using System.IO;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{

public abstract class FileBuilder
{
  private TextWriter _writer = null;

  protected FileBuilder (TextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);

    _writer = writer;
  }

  protected void Write (string text)
  {
    _writer.Write (text);
  }

  protected void WriteLine ()
  {
    Write (Environment.NewLine);
  }

  protected virtual void FinishFile()
  {
    _writer.Flush();
  }

  public string ReplaceTag (string original, string tag, string value)
  {
    string newString = original.Replace (tag, value);
    if (newString == original)
      throw new ApplicationException (string.Format ("ReplaceTag did not replace tag '{0}' with '{1}' in string '{2}'. Tag was not found.", tag, value, original));
    return newString;
  }
}
}
