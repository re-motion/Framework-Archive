// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Design.BindableObject;
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  [Designer (typeof (BindableObjectDataSourceDesigner))]
  public class BindableObjectDataSourceControl : BusinessObjectDataSourceControl
  {
    private readonly BindableObjectDataSource _dataSource = new BindableObjectDataSource();

    public BindableObjectDataSourceControl ()
    {
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [DefaultValue (null)]
    [Editor (typeof (BindableObjectTypePickerEditor), typeof (UITypeEditor))]
    [TypeConverter (typeof (TypeNameConverter))]
    public Type Type
    {
      get { return _dataSource.Type; }
      set { _dataSource.Type = value; }
    }

    protected override IBusinessObjectDataSource GetDataSource ()
    {
      return _dataSource;
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      _dataSource.Site = Site;
    }
  }
}
