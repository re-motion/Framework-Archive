using System;
using System.IO;
using Rubicon.Utilities;

namespace Rubicon.Development.UnitTesting.IO
{
  /// <summary>
  /// The <see cref="TempFile"/> class represents a disposable temp file created via the <see cref="Path.GetTempFileName"/> method.
  /// </summary>
  public class TempFile : DisposableBase
  {
    private string _fileName;

    public TempFile ()
    {
      _fileName = Path.GetTempFileName ();
    }

    protected override void Dispose (bool disposing)
    {
      if (_fileName != null && File.Exists (_fileName))
      {
        File.Delete (_fileName);
        _fileName = null;
      }
    }

    public string FileName
    {
      get { return _fileName; }
    }

    public void SaveStream (Stream stream)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);

      using (StreamReader streamReader = new StreamReader (stream))
      {
        using (StreamWriter streamWriter = new StreamWriter (_fileName))
        {
          while (!streamReader.EndOfStream)
            streamWriter.WriteLine (streamReader.ReadLine ());
        }
      }
    }
  }
}