using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  // TODO Doc: 
  public class DomainObjectPropertyFactory: ReflectionPropertyFactory
  {
    private ClassDefinition _classDefinition;

    public DomainObjectPropertyFactory (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      _classDefinition = classDefinition;
    }

    protected override Type GetItemTypeForDomainObjectCollection (PropertyInfo propertyInfo)
    {
      Type itemType = base.GetItemTypeForDomainObjectCollection (propertyInfo);

      if (itemType == typeof (BindableDomainObject))
      {
        IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyInfo);

        if (relationEndPointDefinition != null)
        {
          IRelationEndPointDefinition oppositeRelationEndPointDefinition = GetMandatoryOppositeEndPointDefinition (propertyInfo);
          itemType = oppositeRelationEndPointDefinition.ClassDefinition.ClassType;
        }
      }

      return itemType;
    }

    protected override bool IsDateType (PropertyInfo propertyInfo)
    {
      //TODO: Remove this test once the Data.DomainObjects.ObjectBinding.Legacy assembly has been extracted.
      if (_classDefinition is ReflectionBasedClassDefinition)
        return base.IsDateType (propertyInfo);

      XmlBasedPropertyDefinition propertyDefinition = GetPropertyDefinition (propertyInfo) as XmlBasedPropertyDefinition;
      return propertyDefinition != null && propertyDefinition.MappingTypeName == "date";
    }

    protected override bool IsPropertyRequired (PropertyInfo propertyInfo)
    {
      PropertyDefinition propertyDefinition = GetPropertyDefinition (propertyInfo);
      return (propertyDefinition != null) ? !propertyDefinition.IsNullable : base.IsPropertyRequired (propertyInfo);
    }

    protected override int? GetMaxStringLength (PropertyInfo propertyInfo)
    {
      PropertyDefinition propertyDefinition = GetPropertyDefinition(propertyInfo);
      return (propertyDefinition != null) ? propertyDefinition.MaxLength : base.GetMaxStringLength (propertyInfo);
    }

    protected override bool IsRelationMandatory (PropertyInfo propertyInfo)
    {
      IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition(propertyInfo);
      if (relationEndPointDefinition != null)
        return relationEndPointDefinition.IsMandatory;

      return base.IsRelationMandatory (propertyInfo);
    }

    private PropertyDefinition GetPropertyDefinition(PropertyInfo propertyInfo)
    {
      return _classDefinition.GetPropertyDefinition (GetPropertyName (propertyInfo));
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition(PropertyInfo propertyInfo)
    {
      return _classDefinition.GetRelationEndPointDefinition (GetPropertyName (propertyInfo));
    }

    private IRelationEndPointDefinition GetMandatoryOppositeEndPointDefinition (PropertyInfo propertyInfo)
    {
      return _classDefinition.GetMandatoryOppositeEndPointDefinition (GetPropertyName (propertyInfo));
    }

    private string GetPropertyName (PropertyInfo propertyInfo)
    {
      if (_classDefinition is ReflectionBasedClassDefinition)
        return ReflectionUtility.GetPropertyName (propertyInfo);
      else
        return propertyInfo.Name;
    }
  }
}