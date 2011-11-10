// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Configuration;
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Configuration
{
  /// <summary>
  /// The <see cref="ConfigurationHelper"/> is a ulitilty class designed to deserialize xml-fragments into configuration elements.
  /// </summary>
  public static class ConfigurationHelper
  {
    public static void DeserializeElement (ConfigurationElement configurationElement, string xmlFragment)
    {
      ArgumentUtility.CheckNotNull ("configurationElement", configurationElement);
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      using (XmlTextReader reader = new XmlTextReader (xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        reader.IsStartElement();
        PrivateInvoke.InvokeNonPublicMethod (configurationElement, "DeserializeElement", reader, false);
      }
    }

    public static void DeserializeSection (ConfigurationSection configurationSection, string xmlFragment)
    {
      ArgumentUtility.CheckNotNull ("configurationSection", configurationSection);
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      using (XmlTextReader reader = new XmlTextReader (xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        PrivateInvoke.InvokeNonPublicMethod (configurationSection, "DeserializeSection", reader);
      }
    }
  }
}
