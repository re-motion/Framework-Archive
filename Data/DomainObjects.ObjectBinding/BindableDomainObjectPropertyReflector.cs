using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectPropertyReflector : PropertyReflector
  {
    private PropertyDefinition _propertyDefinition;
    private IRelationEndPointDefinition _relationEndPointDefinition;

    public BindableDomainObjectPropertyReflector (Type targetType, PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
        : base (propertyInfo, businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      InitializeMappingDefinitions (targetType, propertyInfo);
    }

    private void InitializeMappingDefinitions (Type targetType, PropertyInfo propertyInfo)
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[targetType];
      if (classDefinition != null)
      {
        string propertyName = ReflectionUtility.GetPropertyName (propertyInfo);
        _propertyDefinition = classDefinition.GetPropertyDefinition (propertyName);
        _relationEndPointDefinition = classDefinition.GetRelationEndPointDefinition (propertyName);
      }
    }

    protected override bool GetIsRequired ()
    {
      if (_relationEndPointDefinition != null)
        return _relationEndPointDefinition.IsMandatory;
      else if (_propertyDefinition != null)
        return !_propertyDefinition.IsNullable;
      else
        return base.GetIsRequired ();
    }

    protected override int? GetMaxLength ()
    {
      if (_propertyDefinition != null)
        return _propertyDefinition.MaxLength;
      else
        return base.GetMaxLength ();
    }
  }
}