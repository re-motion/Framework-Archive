using System;
using System.Reflection;
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
        IRelationEndPointDefinition relationEndPointDefinition = _classDefinition.GetRelationEndPointDefinition (GetPropertyName (propertyInfo));

        if (relationEndPointDefinition != null)
        {
          IRelationEndPointDefinition oppositeRelationEndPointDefinition = _classDefinition.GetMandatoryOppositeEndPointDefinition (GetPropertyName (propertyInfo));
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

      PropertyDefinition propertyDefinition = _classDefinition.GetPropertyDefinition (GetPropertyName (propertyInfo));
      return propertyDefinition != null && propertyDefinition.MappingTypeName == "date";
    }

    protected override bool IsPropertyRequired (PropertyInfo propertyInfo)
    {
      PropertyDefinition propertyDefinition = _classDefinition.GetPropertyDefinition (GetPropertyName (propertyInfo));
      return (propertyDefinition != null) ? !propertyDefinition.IsNullable : base.IsPropertyRequired (propertyInfo);
    }

    protected override NaInt32 GetMaxStringLength (PropertyInfo propertyInfo)
    {
      PropertyDefinition propertyDefinition = _classDefinition.GetPropertyDefinition (GetPropertyName (propertyInfo));
      return (propertyDefinition != null) ? propertyDefinition.MaxLength : base.GetMaxStringLength (propertyInfo);
    }

    protected override bool IsRelationMandatory (PropertyInfo propertyInfo)
    {
      IRelationEndPointDefinition relationEndPointDefinition = _classDefinition.GetRelationEndPointDefinition (GetPropertyName (propertyInfo));
      if (relationEndPointDefinition != null)
        return relationEndPointDefinition.IsMandatory;

      return base.IsRelationMandatory (propertyInfo);
    }

    private static string GetPropertyName (PropertyInfo propertyInfo)
    {
      return ReflectionUtility.GetPropertyName (propertyInfo);
    }
  }
}