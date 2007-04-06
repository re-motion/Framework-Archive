using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.Mapping
{
  [Serializable]
  public class ReflectionBasedClassDefinition: ClassDefinition
  {
    private bool _isAbstract;

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType)
        : base (id, entityName, storageProviderID, classType)
    {
      Initialize (null);
    }

    public ReflectionBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, bool isAbstract)
        : base (id, entityName, storageProviderID, classType)
    {
      Initialize (isAbstract);
    }

    public ReflectionBasedClassDefinition (
        string id, string entityName, string storageProviderID, Type classType, bool isAbstract, ReflectionBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classType, (ClassDefinition) baseClass)
    {
      Initialize (isAbstract);
    }

    public ReflectionBasedClassDefinition (
        string id, string entityName, string storageProviderID, Type classType, ReflectionBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, classType, baseClass)
    {
      Initialize (null);
    }

    private void Initialize (bool? isAbstract)
    {
      if (isAbstract.HasValue)
        _isAbstract = isAbstract.Value;
      else
        _isAbstract = ClassType.IsAbstract;
    }

    protected ReflectionBasedClassDefinition (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    public new ReflectionBasedClassDefinition BaseClass
    {
      get { return (ReflectionBasedClassDefinition) base.BaseClass; }
    }

    public override bool IsAbstract
    {
      get { return _isAbstract; }
    }
  }
}