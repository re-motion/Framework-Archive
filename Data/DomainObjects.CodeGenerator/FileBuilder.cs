using System;
using System.IO;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public abstract class BaseBuilder : IBuilder
{
  // types

  // static members and constants

  // member fields

  private bool _disposed = false;
  private string _fileName = null;
  private StreamWriter _writer;

  // construction and disposing

  protected BaseBuilder (string fileName)
  {
    ArgumentUtility.CheckNotNull ("fileName", fileName);

    _fileName = fileName;
  }

  ~BaseBuilder ()      
  {
    Dispose (false);
  }

  public void Dispose ()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose (bool disposing)
  {
    if(!_disposed)
    {
      if(disposing)
      {
        _writer.Close ();
        _writer = null;
      }
    }
    _disposed = true;         
  }

  // methods and properties

  protected string FileName
  {
    get { return _fileName; }
    set { _fileName = value;}
  }

  protected void OpenFile ()
  {
    if (_fileName != null)
      _writer = new StreamWriter (_fileName);
  }

  protected virtual void CloseFile ()
  {
    _writer.Close ();
  }

  protected void Write (string text)
  {
    _writer.Write (text);
  }

  protected void WriteLine ()
  {
    Write (Environment.NewLine);
  }

  public string ReplaceTag (string original, string tag, string value)
  {
    string newString = original.Replace (tag, value);
    if (newString == original)
      throw new ApplicationException (string.Format ("ReplaceTag did not replace tag '{0}' with '{1}' in string '{2}'. Tag was not found.", tag, value, original));
    return newString;
  }

  #region IBuilder Members

  public abstract void Build ();

  #endregion
}
}
