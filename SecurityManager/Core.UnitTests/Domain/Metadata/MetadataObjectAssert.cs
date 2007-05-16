using System;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  public static class MetadataObjectAssert
  {
    public static void AreEqual (EnumValueDefinition expected, EnumValueDefinition actual)
    {
      AreEqual (expected, actual, string.Empty);
    }

    public static void AreEqual (EnumValueDefinition expected, EnumValueDefinition actual, string message)
    {
      Assert.AreEqual (expected.MetadataItemID, actual.MetadataItemID, message);
      Assert.AreEqual (expected.Name, actual.Name, message);
      Assert.AreEqual (expected.Index, actual.Index, message);
      Assert.AreEqual (expected.Value, actual.Value, message);
    }

    public static void AreEqual (AccessTypeDefinition expected, AccessTypeDefinition actual)
    {
      AreEqual (expected, actual, string.Empty);
    }

    public static void AreEqual (AccessTypeDefinition expected, AccessTypeDefinition actual, string message)
    {
      AreEqual ((EnumValueDefinition) expected, (EnumValueDefinition) actual, message);
    }


    public static void AreEqual (StateDefinition expected, StateDefinition actual)
    {
      AreEqual (expected, actual, string.Empty);
    }

    public static void AreEqual (StateDefinition expected, StateDefinition actual, string message)
    {
      Assert.AreEqual (expected.Name, actual.Name, message);
      Assert.AreEqual (expected.Index, actual.Index, message);
      Assert.AreEqual (expected.Value, actual.Value, message);
    }

    public static void AreEqual (StatePropertyDefinition expected, StatePropertyDefinition actual, string message)
    {
      Assert.AreEqual (expected.MetadataItemID, actual.MetadataItemID, message);
      Assert.AreEqual (expected.Name, actual.Name, message);
      Assert.AreEqual (expected.Index, actual.Index, message);

      Assert.AreEqual (expected.DefinedStates.Count, actual.DefinedStates.Count, message);

      for (int i = 0; i < expected.DefinedStates.Count; i++)
        AreEqual ((StateDefinition) expected.DefinedStates[i], (StateDefinition) actual.DefinedStates[i], message);
    }

    public static void AreEqual (SecurableClassDefinition expected, SecurableClassDefinition actual)
    {
      AreEqual (expected, actual, string.Empty, null);
    }

    public static void AreEqual (SecurableClassDefinition expected, SecurableClassDefinition actual, string message)
    {
      AreEqual (expected, actual, message, null);
    }

    public static void AreEqual (SecurableClassDefinition expected, SecurableClassDefinition actual, string message, params object[] args)
    {
      Assert.AreEqual (expected.Name, actual.Name, message, args);
      Assert.AreEqual (expected.Index, actual.Index, message);
    }
  }
}
