using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Utilities;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectPropertyReflector : PropertyReflector
  {
    private PropertyDefinition _propertyDefinition;
    private IRelationEndPointDefinition _relationEndPointDefinition;

    public BindableDomainObjectPropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
        : base (propertyInfo, businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      InitializeMappingDefinitions (concreteType, propertyInfo);
    }

    private void InitializeMappingDefinitions (Type concreteType, IPropertyInformation propertyInfo)
    {
      Type targetType = Mixins.TypeUtility.GetUnderlyingTargetType (concreteType);
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[targetType];
      if (classDefinition != null)
      {
        string propertyName = ReflectionUtility.GetPropertyName (propertyInfo.GetOriginalDeclaringType(), propertyInfo.Name);
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