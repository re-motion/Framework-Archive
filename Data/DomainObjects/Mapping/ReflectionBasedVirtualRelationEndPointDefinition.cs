using System;
using System.Reflection;
using System.Runtime.Serialization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
  [Serializable]
  public class ReflectionBasedVirtualRelationEndPointDefinition : VirtualRelationEndPointDefinition
  {
    private readonly PropertyInfo _propertyInfo;

    public ReflectionBasedVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string propertyTypeName,
        string sortExpression,
        PropertyInfo propertyInfo)
        : base (classDefinition, propertyName, isMandatory, cardinality, propertyTypeName, sortExpression)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    public ReflectionBasedVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string sortExpression,
        PropertyInfo propertyInfo)
      : base (classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpression)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    protected ReflectionBasedVirtualRelationEndPointDefinition (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("context", context);

      _propertyInfo = (PropertyInfo) info.GetValue ("_propertyInfo", typeof (PropertyInfo));
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("context", context);

      base.GetObjectData (info, context);
      info.AddValue ("_propertyInfo", _propertyInfo);
    }
  }
}