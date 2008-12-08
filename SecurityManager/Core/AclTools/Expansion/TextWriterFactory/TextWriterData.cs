// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  public class TextWriterData
  {
    public TextWriterData (TextWriter textWriter, string directory, string extension)
    {
      ArgumentUtility.CheckNotNull ("textWriter", textWriter);
      ArgumentUtility.CheckNotNull ("directory", directory); // directory empty OK
      // extension NULL OK
      TextWriter = textWriter;
      Directory = directory;
      Extension = extension;
    }

    public TextWriter TextWriter { get; private set; }
    public string Directory { get; private set; }
    public string Extension { get; private set; }
  }
}
