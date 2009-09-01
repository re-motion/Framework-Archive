// This file is part of re-vision (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.IO
{
  /// <summary>
  /// The <see cref="TempFile"/> class represents a disposable temp file created via the <see cref="Path.GetTempFileName"/> method.
  /// </summary>
  public class TempFile : DisposableBase
  {
    private string _fileName;

    public TempFile ()
    {
      _fileName = Path.GetTempFileName();
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
      get
      {
        //TODO: Use AssertNotDisposed once new DisposableBase is commited
        if (Disposed)
          throw new InvalidOperationException ("Object disposed.");
        return _fileName;
      }
    }

    public void WriteStream (Stream stream)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);

      using (StreamReader streamReader = new StreamReader (stream))
      {
        using (StreamWriter streamWriter = new StreamWriter (_fileName))
        {
          while (!streamReader.EndOfStream)
          {
            char[] buffer = new char[100];
            streamWriter.Write (buffer, 0, streamReader.Read (buffer, 0, buffer.Length));
          }
        }
      }
    }

    public void WriteAllBytes (byte[] bytes)
    {
      ArgumentUtility.CheckNotNull ("bytes", bytes);

      File.WriteAllBytes (_fileName, bytes);
    }

    public long Lenght
    {
      get
      {
        var fileInfo = new FileInfo (_fileName);
        return fileInfo.Length;
      }
    }
  }
}