using System;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Legacy.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.Mapping
{
  /// <summary><see cref="ClassDefinition"/> used when loading the mappign from an xml-file.</summary>
  [Serializable]
  public class XmlBasedClassDefinition: ClassDefinition
  {
    [NonSerialized]
    private Type _classType;
    [NonSerialized]
    private string _classTypeName;

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType)
        : this (id, entityName, storageProviderID, classType, null)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, Type classType, XmlBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, true)
    {
      ArgumentUtility.CheckNotNull ("classType", classType);

      Initialize (classType, null, false, baseClass);
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName)
        : this (id, entityName, storageProviderID, classTypeName, resolveClassTypeName, null)
    {
    }

    public XmlBasedClassDefinition (string id, string entityName, string storageProviderID, string classTypeName, bool resolveClassTypeName, XmlBasedClassDefinition baseClass)
        : base (id, entityName, storageProviderID, resolveClassTypeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("classTypeName", classTypeName);

      Initialize (null, classTypeName, resolveClassTypeName, baseClass);
    }

    private void Initialize (Type classType, string classTypeName, bool resolveClassTypeName, XmlBasedClassDefinition baseClass)
    {
      if (resolveClassTypeName)
        classType = Type.GetType (classTypeName, true);

      if (classType != null)
      {
        CheckClassType (ID, classType);
        classTypeName = classType.AssemblyQualifiedName;
      }

      _classType = classType;
      _classTypeName = classTypeName;

      if (baseClass != null)
      {
        // Note: CheckBasePropertyDefinitions does not have to be called, because member _propertyDefinitions is
        //       initialized to an empty collection during construction.
        SetBaseClass (baseClass);
      }
    }

    private void CheckClassType (string classID, Type classType)
    {
      if (!classType.IsSubclassOf (typeof (DomainObject)))
        throw CreateMappingException ("Type '{0}' of class '{1}' is not derived from 'Remotion.Data.DomainObjects.DomainObject'.", classType, classID);
    }

    public new XmlBasedClassDefinition BaseClass
    {
      get { return (XmlBasedClassDefinition) base.BaseClass; }
    }

    public override bool IsAbstract
    {
      get
      {
        if (ClassType == null)
          throw CreateInvalidOperationException ("Cannot evaluate IsAbstract for ClassDefinition '{0}' since ResolveTypeNames is false.", ID);

        return ClassType.IsAbstract;
      }
    }

    public override Type ClassType
    {
      get { return _classType; }
    }

    public string ClassTypeName
    {
      get { return _classTypeName; }
    }

    public override bool IsClassTypeResolved
    {
      get { return (_classType != null); }
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (string.Format (message, args));
    }

    protected override IDomainObjectCreator GetDomainObjectCreator ()
    {
      return DirectDomainObjectCreator.Instance;
    }
  }
}