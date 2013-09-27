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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.MappingSerialization
{
  public class EnumSerializer : IEnumSerializer
  {
    public EnumSerializer ()
    {
      
    }

    public IEnumerable<XElement> Serialize (EnumTypeCollection enumTypeCollection)
    {
      ArgumentUtility.CheckNotNull ("enumTypeCollection", enumTypeCollection);

      return enumTypeCollection.Select (t => new XElement ("enumType", 
        new XAttribute("type", TypeUtility.GetAbbreviatedTypeName(t, false)),
        GetValues(t)
        ));
    }

    private IEnumerable<XElement> GetValues (Type type)
    {
      if (ExtensibleEnumUtility.IsExtensibleEnumType (type))
      {
        return ExtensibleEnumUtility.GetDefinition(type).GetValueInfos().Select (
          info => new XElement (
              "value",
              new XAttribute ("name", info.Value.ValueName),
              new XAttribute ("columnValue", info.Value.ID)));

      }

      return Enum.GetValues (type).Cast<object>().Select (
          value => new XElement (
              "value",
              new XAttribute ("name", Enum.GetName (type, value)),
              new XAttribute ("columnValue", Convert.ChangeType(value, Enum.GetUnderlyingType(type)))
              ));
    }
  }
}