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
using System.Collections.Specialized;

namespace Remotion.Globalization
{
  /// <summary> A <b>Null Object</b> implementation of <see cref="IResourceManager"/>. </summary>
  /// <remarks> 
  ///   Use <see cref="Instance"/> to access the well defined instance of the <see cref="NullResourceManager"/>.
  /// </remarks>
  public sealed class NullResourceManager : IResourceManager
  {
    public static readonly NullResourceManager Instance = new NullResourceManager();

    private NullResourceManager ()
    {
    }

    NameValueCollection IResourceManager.GetAllStrings ()
    {
      return new NameValueCollection();
    }

    NameValueCollection IResourceManager.GetAllStrings (string prefix)
    {
      return new NameValueCollection();
    }

    string IResourceManager.GetString (string id)
    {
      return id;
    }

    public bool ContainsResource (string id)
    {
      return false;
    }

    string IResourceManager.Name
    {
      get { return "Remotion.Globalization.NullResourceManager"; }
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
