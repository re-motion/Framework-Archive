using System;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.SampleTypes;
using Remotion.Mixins.CodeGeneration;

namespace Remotion.Mixins.UnitTests.CodeGeneration
{
  [TestFixture]
  public class NamespaceChangingNameProviderTests
  {
    [Test]
    public void NormalNameGetsExtendedNamespace()
    {
      INameProvider nameProvider = NamespaceChangingNameProvider.Instance;

      TargetClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
      string newName = nameProvider.GetNewTypeName (definition);

      Assert.AreEqual (typeof (BaseType1).Namespace + ".MixedTypes.BaseType1", newName);
    }

    [Test]
    public void GenericNameGetsExtendedNamespacePlusCharacterReplacements()
    {
      INameProvider nameProvider = NamespaceChangingNameProvider.Instance;

      TargetClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (GenericTargetClass<int>), GenerationPolicy.ForceGeneration);
      string newName = nameProvider.GetNewTypeName (definition);

      Assert.AreEqual (typeof (GenericTargetClass<int>).Namespace +
          ".MixedTypes.GenericTargetClass`1{System_Int32/mscorlib/Version=2_0_0_0/Culture=neutral/PublicKeyToken=b77a5c561934e089}",
          newName);
    }

  }
}