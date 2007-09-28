using System;
using System.Runtime.Serialization;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{
  /// <summary><see cref="ClassDefinition"/> used when loading the mapping from the reflection meta data.</summary>
  [Serializable]
  public class ReflectionBasedClassDefinition: ClassDefinition
  {
    private readonly bool _isAbstract;
    private readonly Type _classType;

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract)
        : this (id, entityName, storageProviderID, classType, isAbstract, null)
    {
    }

    public ReflectionBasedClassDefinition (
        string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, true)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);
      if (!classType.IsSubclassOf (typeof (DomainObject)))
        throw CreateMappingException ("Type '{0}' of class '{1}' is not derived from 'Rubicon.Data.DomainObjects.DomainObject'.", classType, ID);
     
      _classType = classType;
      _isAbstract = isAbstract;

      if (baseClass != null)
      {
        // Note: CheckBasePropertyDefinitions does not have to be called, because member _propertyDefinitions is
        //       initialized to an empty collection during construction.
        SetBaseClass (baseClass);
      }
    }

    public new ReflectionBasedClassDefinition BaseClass
    {
      get { return (ReflectionBasedClassDefinition) base.BaseClass; }
    }

    public override bool IsAbstract
    {
      get { return _isAbstract; }
    }

    public override Type ClassType
    {
      get { return _classType; }
    }

    public override bool IsClassTypeResolved
    {
      get { return true; }
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    protected internal override IDomainObjectCreator GetDomainObjectCreator ()
    {
      return FactoryBasedDomainObjectCreator.Instance;
    }

    #region ISerializable Members
    
    protected ReflectionBasedClassDefinition (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
      if (!IsPartOfMappingConfiguration)
      {
        _classType = (Type) info.GetValue ("ClassType", typeof (Type));
      }
    }

    protected override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);
      info.AddValue ("ClassType", _classType);
    }

    #endregion
  }
}