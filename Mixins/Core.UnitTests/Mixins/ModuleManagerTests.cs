using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection.Emit;
using System.IO;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ModuleManagerTests
  {
    private IModuleManager _moduleManager;
    private const string c_assemblyFileName = "Rubicon.Mixins.Generated.Signed.dll";

    [SetUp]
    public void SetUp ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      _moduleManager = ConcreteTypeBuilder.Current.Scope;
      _moduleManager.AssemblyName = "Rubicon.Mixins.Generated";
      _moduleManager.ModulePath = c_assemblyFileName;
      DeleteSavedAssembly ();
    }

    [TearDown]
    public void TearDown ()
    {
      DeleteSavedAssembly ();
    }

    private void DeleteSavedAssembly ()
    {
      if (File.Exists (c_assemblyFileName))
        File.Delete (c_assemblyFileName);
    }

    [Test]
    public void CreateTypeGenerator ()
    {
      BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));

      ITypeGenerator generator = _moduleManager.CreateTypeGenerator (bt1);
      Assert.IsNotNull (generator);
      Assert.IsTrue (bt1.Type.IsAssignableFrom (generator.GetBuiltType ()));
    }

    [Test]
    public void CreateMixinTypeGenerator()
    {
      BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));

      IMixinTypeGenerator mixinGenerator = _moduleManager.CreateMixinTypeGenerator (bt1.Mixins[0]);
      Assert.IsNotNull (mixinGenerator);
      Assert.IsTrue (bt1.Mixins[0].Type.IsAssignableFrom (mixinGenerator.GetBuiltType ()));
    }

    [Test]
    public void HasAssembly ()
    {
      Assert.IsFalse (_moduleManager.HasAssembly);
      TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.IsTrue (_moduleManager.HasAssembly);
    }

    [Test]
    public void SaveAssembly ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));

      Assert.IsFalse (File.Exists (c_assemblyFileName));
      string path = _moduleManager.SaveAssembly ();
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, c_assemblyFileName), path);
      Assert.IsTrue (File.Exists (c_assemblyFileName));
    }

    [Test]
    public void SaveAssemblyWithDifferentNameAndPath ()
    {
      _moduleManager.AssemblyName = "Foo";
      string path = Path.GetTempFileName();
      _moduleManager.ModulePath = path;
      File.Delete (path);

      TypeFactory.GetConcreteType (typeof (BaseType1));

      Assert.IsFalse (File.Exists (path));
      _moduleManager.SaveAssembly ();
      Assert.IsTrue (File.Exists (path));

      try
      {
        AssemblyName name = AssemblyName.GetAssemblyName (path);
        Assert.AreEqual ("Foo", name.Name);
      }
      finally
      {
        File.Delete (path);
      }
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The name can only be set before the first type is built.")]
    public void SettingNameThrowsWhenTypeGenerated ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.AssemblyName = "Foo";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The module path can only be set before the first type is built.")]
    public void SettingPathThrowsWhenTypeGenerated ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.ModulePath = "Foo.dll";
    }

    [Test]
    public void SavedAssemblyHasStrongName ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));

      _moduleManager.SaveAssembly ();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_assemblyFileName);
      
      Assert.IsNotNull (assemblyName.GetPublicKey ());
      Assert.AreNotEqual (0, assemblyName.GetPublicKey ().Length);
    }

    [Test]
    public void SavedAssemblyHasMixinAssemblyName ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));

      _moduleManager.SaveAssembly ();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_assemblyFileName);

      Assert.AreEqual ("Rubicon.Mixins.Generated", assemblyName.Name);
    }

    [Serializable]
    private class CrossApDomainTypeChecker
    {
      public static void CheckForType (string typeName, AssemblyName assemblyToLoad)
      {
        AppDomain checkDomain = AppDomain.CreateDomain ("Test domain", null, AppDomain.CurrentDomain.SetupInformation);
        CrossApDomainTypeChecker checker = new CrossApDomainTypeChecker (typeName, assemblyToLoad);
        Serializer.SerializeAndDeserialize (checker);

        try
        {
          checkDomain.DoCallBack (checker.PerformCheck);
        }
        finally
        {
          AppDomain.Unload (checkDomain);
        }
      }

      private string _typeNameToFind;
      private AssemblyName _assemblyToLoad;

      public CrossApDomainTypeChecker (string typeNameToFind, AssemblyName assemblyToLoad)
      {
        ArgumentUtility.CheckNotNull ("typeNameToFind", typeNameToFind);
        ArgumentUtility.CheckNotNull ("assemblyToLoad", assemblyToLoad);

        _typeNameToFind = typeNameToFind;
        _assemblyToLoad = assemblyToLoad;
      }

      public void PerformCheck ()
      {
        Assembly reloadedAssembly = Assembly.Load (_assemblyToLoad);
        Assert.IsNotNull (reloadedAssembly.GetType (_typeNameToFind));
      }
    }

    [Test]
    public void SavedAssemblyContainsGeneratedType ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.SaveAssembly ();

      CrossApDomainTypeChecker.CheckForType (concreteType.FullName, AssemblyName.GetAssemblyName (c_assemblyFileName));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No types have been built, so no assembly has been generated.")]
    public void SaveThrowsWhenNoTypeCreated ()
    {
      _moduleManager.SaveAssembly ();
    }
  }
}