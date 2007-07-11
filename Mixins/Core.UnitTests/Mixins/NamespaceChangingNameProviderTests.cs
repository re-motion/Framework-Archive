using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.CodeGeneration;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class NamespaceChangingNameProviderTests
  {
    [Test]
    public void NormalNameGetsExtendedNamespace()
    {
      INameProvider nameProvider = NamespaceChangingNameProvider.Instance;

      BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      string newName = nameProvider.GetNewTypeName (definition);

      Assert.AreEqual (typeof (BaseType1).Namespace + ".MixedTypes.BaseType1", newName);
    }

    [Test]
    public void GenericNameGetsExtendedNamespacePlusCharacterReplacements()
    {
      INameProvider nameProvider = NamespaceChangingNameProvider.Instance;

      BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (GenericBaseClass<int>));
      string newName = nameProvider.GetNewTypeName (definition);

      Assert.AreEqual (typeof (GenericBaseClass<int>).Namespace +
          ".MixedTypes.GenericBaseClass`1{System_Int32/mscorlib/Version=2_0_0_0/Culture=neutral/PublicKeyToken=b77a5c561934e089}",
          newName);
    }

  }
}