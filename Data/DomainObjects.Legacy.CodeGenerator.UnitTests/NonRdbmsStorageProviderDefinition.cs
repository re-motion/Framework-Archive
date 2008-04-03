using System;
using System.Xml;

using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests
{
  public class NonRdbmsStorageProviderDefinition : StorageProviderDefinition
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public NonRdbmsStorageProviderDefinition (
        string storageProviderID,
        Type storageProviderType)
      : base (storageProviderID, storageProviderType)
    {
    }

    public NonRdbmsStorageProviderDefinition (
        string storageProviderID,
        Type storageProviderType,
        XmlNode configurationNode)
      : base (storageProviderID, storageProviderType)
    {
    }

    // methods and properties

    public override bool IsIdentityTypeSupported (Type identityType)
    {
      ArgumentUtility.CheckNotNull ("identityType", identityType);

      // Note: UnitTestStorageProviderStubDefinition supports all identity types for testing purposes.
      return true;
    }

  }
}
