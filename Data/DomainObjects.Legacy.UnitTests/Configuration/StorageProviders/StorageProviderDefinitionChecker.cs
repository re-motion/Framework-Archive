using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.Configuration.StorageProviders
{
  public class StorageProviderDefinitionChecker
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public StorageProviderDefinitionChecker ()
    {
    }

    // methods and properties

    public void Check (
        StorageProviderDefinitionCollection expectedProviders,
        StorageProviderDefinitionCollection actualProviders)
    {
      Assert.AreEqual (expectedProviders.Count, actualProviders.Count,
          string.Format ("Number of storage providers does not match. Expected: {0}, actual: {1}",
          expectedProviders.Count, actualProviders.Count));

      foreach (StorageProviderDefinition expectedProvider in expectedProviders)
      {
        StorageProviderDefinition actualProvider = actualProviders[expectedProvider.ID];
        CheckProvider (expectedProvider, actualProvider);
      }
    }

    private void CheckProvider (
        StorageProviderDefinition expectedProvider,
        StorageProviderDefinition actualProvider)
    {
      Assert.AreEqual (expectedProvider.ID, actualProvider.ID,
          string.Format ("ID of provider definitions does not match. Expected: {0}, actual: {1}",
          expectedProvider.ID,
          expectedProvider.ID,
          actualProvider.ID));

      Assert.AreEqual (expectedProvider.GetType (), actualProvider.GetType (),
          string.Format ("Providers (ID: '{0}') are not of same type. Expected: {1}, actual: {2}",
          expectedProvider.ID,
          expectedProvider.GetType (),
          actualProvider.GetType ()));

      Assert.AreEqual (expectedProvider.StorageProviderType, actualProvider.StorageProviderType,
          string.Format ("ProviderType of provider definitions (ID: '{0}') does not match. Expected: {1}, actual: {2}",
          expectedProvider.ID,
          expectedProvider.StorageProviderType,
          actualProvider.StorageProviderType));

      if (expectedProvider as RdbmsProviderDefinition != null)
      {
        CheckRdbmsProvider ((RdbmsProviderDefinition) expectedProvider, (RdbmsProviderDefinition) actualProvider);
      }
    }

    private void CheckRdbmsProvider (
        RdbmsProviderDefinition expectedProvider,
        RdbmsProviderDefinition actualProvider)
    {
      Assert.AreEqual (expectedProvider.ConnectionString, actualProvider.ConnectionString,
          string.Format ("ConnectionString of provider definitions (ID: '{0}') does not match. Expected: {1}, actual: {2}",
          expectedProvider.ID,
          expectedProvider.ConnectionString,
          actualProvider.ConnectionString));
    }
  }
}
