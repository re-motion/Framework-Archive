using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions.Building
{
  public class AttributeDefinitionBuilder
  {
    private IAttributableDefinition _attributableDefinition;

    public AttributeDefinitionBuilder (IAttributableDefinition attributableDefinition)
    {
      ArgumentUtility.CheckNotNull ("attributableDefinition", attributableDefinition);
      _attributableDefinition = attributableDefinition;
    }

    public void Apply (IEnumerable<CustomAttributeData> attributes)
    {
      foreach (CustomAttributeData attributeData in attributes)
      {
        if (!typeof (SerializableAttribute).Equals(attributeData.Constructor.DeclaringType))
          _attributableDefinition.CustomAttributes.Add (new AttributeDefinition (_attributableDefinition, attributeData));
      }
    }
  }
}
