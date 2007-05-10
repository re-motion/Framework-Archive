using System;
using System.IO;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Legacy.CodeGenerator
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
      _writer.WriteLine ();
    }

    protected virtual void FinishFile ()
    {
      _writer.Flush ();
    }

    public string ReplaceTag (string originalString, string tag, string value)
    {
      ArgumentUtility.CheckNotNull ("originalString", originalString);
      if (!originalString.Contains (tag))
        throw new ArgumentException ("tag", string.Format ("Tag '{0}' could not be found in original string '{1}'.", tag, originalString));

      return originalString.Replace (tag, value);
    }
  }
}
