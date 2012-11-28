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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.Data.DomainObjects.Web.Test.Performance
{
  public class BocListWithRowMenues : BocList
  {
    protected override Remotion.Web.UI.Controls.WebMenuItem[] InitializeRowMenuItems (Remotion.ObjectBinding.IBusinessObject businessObject, int listIndex)
    {
      return new WebMenuItem[] { 
      //new WebMenuItem {Text = "Item 1", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 2", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 3", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 4", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 5", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 6", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 7", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 8", Command = new Command (CommandType.Event)},
      //new WebMenuItem {Text = "Item 9", Command = new Command (CommandType.Event)},
      };
    }
  }
}