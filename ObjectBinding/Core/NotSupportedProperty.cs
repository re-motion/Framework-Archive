using System;
using System.Reflection;

namespace Rubicon.ObjectBinding
{
  public sealed class NotSupportedProperty : PropertyBase
  {
    public NotSupportedProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isRequired)
        : base(propertyInfo, itemType, isList, isRequired)
    {
        
    }
  }
}