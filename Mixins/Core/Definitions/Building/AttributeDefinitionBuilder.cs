using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
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
        Type attributeType = attributeData.Constructor.DeclaringType;
        if (attributeType.IsVisible && !IsIgnoredAttributeType (attributeType))
          _attributableDefinition.CustomAttributes.Add (new AttributeDefinition (_attributableDefinition, attributeData));
      }
    }

    private bool IsIgnoredAttributeType (Type type)
    {
      return type == typeof (SerializableAttribute) || type.Assembly.Equals (typeof (Mixin).Assembly);
    }
  }
}
