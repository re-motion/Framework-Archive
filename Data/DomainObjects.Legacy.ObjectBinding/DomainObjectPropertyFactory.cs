using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  // TODO Doc: 
  public class DomainObjectPropertyFactory: ReflectionPropertyFactory
  {
    public DomainObjectPropertyFactory (DomainObjectClass businessObjectClass)
      : base (businessObjectClass)
    {
    }

    protected DomainObjectClass DomainObjectClass
    {
      get { return (DomainObjectClass) BusinessObjectClass; }
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
      if (DomainObjectClass.ClassDefinition is ReflectionBasedClassDefinition)
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
      return DomainObjectClass.ClassDefinition.GetPropertyDefinition (GetPropertyName (propertyInfo));
    }

    private IRelationEndPointDefinition GetRelationEndPointDefinition(PropertyInfo propertyInfo)
    {
      return DomainObjectClass.ClassDefinition.GetRelationEndPointDefinition (GetPropertyName (propertyInfo));
    }

    private IRelationEndPointDefinition GetMandatoryOppositeEndPointDefinition (PropertyInfo propertyInfo)
    {
      return DomainObjectClass.ClassDefinition.GetMandatoryOppositeEndPointDefinition (GetPropertyName (propertyInfo));
    }

    private string GetPropertyName (PropertyInfo propertyInfo)
    {
      if (DomainObjectClass.ClassDefinition is ReflectionBasedClassDefinition)
        return ReflectionUtility.GetPropertyName (propertyInfo);
      else
        return propertyInfo.Name;
    }
  }
}