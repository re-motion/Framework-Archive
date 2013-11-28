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
using System.Reflection;
using Remotion.ExtensibleEnums;
using Remotion.Globalization;

namespace Remotion.UnitTests.Globalization.TestDomain
{
  [MultiLingualResources ("Remotion.UnitTests.Globalization.Resources.ExtensibleEnumWithResources")]
  public static class ExtensibleEnumWithResourcesExtensions
  {
    public static ExtensibleEnumWithResources Value1 (this ExtensibleEnumDefinition<ExtensibleEnumWithResources> definition)
    {
      return new ExtensibleEnumWithResources (MethodBase.GetCurrentMethod ());
    }

    public static ExtensibleEnumWithResources Value2 (this ExtensibleEnumDefinition<ExtensibleEnumWithResources> definition)
    {
      return new ExtensibleEnumWithResources (MethodBase.GetCurrentMethod ());
    }

    public static ExtensibleEnumWithResources ValueWithoutResource (this ExtensibleEnumDefinition<ExtensibleEnumWithResources> definition)
    {
      return new ExtensibleEnumWithResources (MethodBase.GetCurrentMethod ());
    }
  }
}