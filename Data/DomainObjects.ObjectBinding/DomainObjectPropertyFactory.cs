using System;
using System.Reflection;

using Rubicon.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class DomainObjectPropertyFactory : ReflectionPropertyFactory
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
      IRelationEndPointDefinition relationEndPointDefinition = _classDefinition.GetRelationEndPointDefinition (propertyInfo.Name);

      if (relationEndPointDefinition != null)
      {
        IRelationEndPointDefinition oppositeRelationEndPointDefinition = _classDefinition.GetOppositeEndPointDefinition (propertyInfo.Name);
        itemType = oppositeRelationEndPointDefinition.ClassDefinition.ClassType;
      }
    }

    return itemType;
  }

  protected override bool IsDateType (PropertyInfo propertyInfo)
  {
    PropertyDefinition propertyDefinition = _classDefinition.GetPropertyDefinition (propertyInfo.Name);
    return propertyDefinition != null && propertyDefinition.MappingType == "date";
  }

  protected override bool IsPropertyRequired (PropertyInfo propertyInfo)
  {
    PropertyDefinition propertyDefinition = _classDefinition.GetPropertyDefinition (propertyInfo.Name);
    return (propertyDefinition != null) ? !propertyDefinition.IsNullable : base.IsPropertyRequired (propertyInfo);
  }

  protected override NaInt32 GetMaxStringLength (PropertyInfo propertyInfo)
  {
    PropertyDefinition propertyDefinition = _classDefinition.GetPropertyDefinition (propertyInfo.Name);
    return (propertyDefinition != null) ? propertyDefinition.MaxLength : base.GetMaxStringLength (propertyInfo);
  }

  protected override bool IsRelationMandatory (PropertyInfo propertyInfo)
  {
    IRelationEndPointDefinition relationEndPointDefinition = _classDefinition.GetRelationEndPointDefinition (propertyInfo.Name);
    if (relationEndPointDefinition != null)
      return relationEndPointDefinition.IsMandatory;

    return base.IsRelationMandatory (propertyInfo);
  }
}
}
