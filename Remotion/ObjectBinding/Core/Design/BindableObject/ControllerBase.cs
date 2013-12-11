// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public abstract class ControllerBase
  {
    protected ControllerBase ()
    {
    }

    protected void AppendImage (ImageList imageList, Enum treeViewIcon)
    {
      Type type = GetType();
      string resourceID = EnumDescription.GetDescription (treeViewIcon);
      Stream stream = type.Assembly.GetManifestResourceStream (type, resourceID);
      Assertion.IsNotNull (stream, string.Format ("Resource '{0}' was not found in namespace '{1}'.", resourceID, type.Namespace));
      try
      {
        imageList.Images.Add (treeViewIcon.ToString (), Image.FromStream (stream));
      }
      catch
      {
        stream.Close();
        throw;
      }
    }

    //TODO: create {enum, string} tuple instead of enum
    protected ImageList CreateImageList (params Enum[] resourceEnums)
    {
      ImageList imageList = new ImageList ();
      imageList.TransparentColor = Color.Magenta;
      try
      {
        foreach (Enum enumValue in resourceEnums)
          AppendImage (imageList, enumValue);

        return imageList;
      }
      catch
      {
        imageList.Dispose();
        throw;
      }
    }
  }
}
