using System;
using System.Collections.Specialized;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Configuration
{
  public abstract class StorageProviderDefinition: ExtendedProviderBase
  {
    // types

    // static members and constants

    // member fields

    private Type _storageProviderType;
    private TypeConversionProvider _typeConversionProvider;
    private TypeProvider _typeProvider;

    // construction and disposing

    protected StorageProviderDefinition (string name, NameValueCollection config)
        : base (name, config)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("config", config);

      string storageProviderTypeName = GetAndRemoveNonEmptyStringAttribute (config, "providerType", name, true);
      Initialize (TypeUtility.GetType (storageProviderTypeName, true, false));
    }

    protected StorageProviderDefinition (string name, Type storageProviderType)
        : base (name, new NameValueCollection())
    {
      ArgumentUtility.CheckNotNull ("storageProviderType", storageProviderType);

      Initialize (storageProviderType);
    }

    private void Initialize(Type storageProviderType)
    {
      _storageProviderType = storageProviderType;
      _typeConversionProvider = TypeConversionProvider.Create ();
      _typeProvider = new TypeProvider();
    }

    // abstract methods and properties

    public abstract bool IsIdentityTypeSupported (Type identityType);

    // methods and properties

    public void CheckIdentityType (Type identityType)
    {
      if (!IsIdentityTypeSupported (identityType))
        throw new IdentityTypeNotSupportedException (_storageProviderType, identityType);
    }

    [Obsolete ("Use property Name instead. (Version: 1.7.42)")]
    public string ID
    {
      get { return Name; }
    }

    public Type StorageProviderType
    {
      get { return _storageProviderType; }
    }

    public TypeConversionProvider TypeConversionProvider
    {
      get { return _typeConversionProvider; }
    }

    public TypeProvider TypeProvider
    {
      get { return _typeProvider; }
    }
  }
}