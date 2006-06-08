using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
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
      Assert.AreEqual (expected.Value, actual.Value, message);
    }

    public static void AreEqual (StateDefinition expected, StateDefinition actual)
    {
      AreEqual (expected, actual, string.Empty);
    }

    public static void AreEqual (StateDefinition expected, StateDefinition actual, string message)
    {
      Assert.AreEqual (expected.Name, actual.Name, message);
      Assert.AreEqual (expected.Value, actual.Value, message);
    }

    public static void AreEqual (StatePropertyDefinition expected, StatePropertyDefinition actual, string message)
    {
      Assert.AreEqual (expected.MetadataItemID, actual.MetadataItemID, message);
      Assert.AreEqual (expected.Name, actual.Name, message);

      Assert.AreEqual (expected.DefinedStates.Count, actual.DefinedStates.Count, message);

      for (int i = 0; i < expected.DefinedStates.Count; i++)
        MetadataObjectAssert.AreEqual ((StateDefinition) expected.DefinedStates[i], (StateDefinition) actual.DefinedStates[i], message);
    }
  }
}
