using System;
using System.Collections.Generic;
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
    private const string c_signedAssemblyFileName = "Rubicon.Mixins.Generated.Signed.dll";
    private const string c_unsignedAssemblyFileName = "Rubicon.Mixins.Generated.Unsigned.dll";

    [SetUp]
    public void SetUp ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      _moduleManager = ConcreteTypeBuilder.Current.Scope;
      _moduleManager.SignedAssemblyName = "Rubicon.Mixins.Generated.Signed";
      _moduleManager.UnsignedAssemblyName = "Rubicon.Mixins.Generated.Unsigned";
      _moduleManager.SignedModulePath = c_signedAssemblyFileName;
      _moduleManager.UnsignedModulePath = c_unsignedAssemblyFileName;
      DeleteSavedAssemblies ();
    }

    [TearDown]
    public void TearDown ()
    {
      DeleteSavedAssemblies ();
    }

    private void DeleteSavedAssemblies ()
    {
      if (File.Exists (c_signedAssemblyFileName))
        File.Delete (c_signedAssemblyFileName);
      if (File.Exists (c_unsignedAssemblyFileName))
        File.Delete (c_unsignedAssemblyFileName);
    }

    [Test]
    public void CreateTypeGenerator ()
    {
      BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));

      ITypeGenerator generator = _moduleManager.CreateTypeGenerator (bt1, GuidNameProvider.Instance);
      Assert.IsNotNull (generator);
      Assert.IsTrue (bt1.Type.IsAssignableFrom (generator.GetBuiltType ()));
    }

    [Test]
    public void CreateMixinTypeGenerator()
    {
      BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));

      IMixinTypeGenerator mixinGenerator = _moduleManager.CreateMixinTypeGenerator (bt1.Mixins[0], GuidNameProvider.Instance);
      Assert.IsNotNull (mixinGenerator);
      Assert.IsTrue (bt1.Mixins[0].Type.IsAssignableFrom (mixinGenerator.GetBuiltType ()));
    }

    [Test]
    public void HasAssemblyFromUnsignedType ()
    {
      Assert.IsFalse (_moduleManager.HasUnsignedAssembly);
      Assert.IsFalse (_moduleManager.HasSignedAssembly);
      Assert.IsFalse (_moduleManager.HasAssemblies);
      
      TypeFactory.GetConcreteType (typeof (BaseType1)); // type from unsigned assembly
      
      Assert.IsTrue (_moduleManager.HasUnsignedAssembly);
      Assert.IsFalse (_moduleManager.HasSignedAssembly);
      Assert.IsTrue (_moduleManager.HasAssemblies);
    }

    [Test]
    public void HasAssemblyFromSignedType ()
    {
      Assert.IsFalse (_moduleManager.HasUnsignedAssembly);
      Assert.IsFalse (_moduleManager.HasSignedAssembly);
      Assert.IsFalse (_moduleManager.HasAssemblies);

      TypeFactory.GetConcreteType (typeof (List<int>)); // type from signed assembly

      Assert.IsFalse (_moduleManager.HasUnsignedAssembly);
      Assert.IsTrue (_moduleManager.HasSignedAssembly);
      Assert.IsTrue (_moduleManager.HasAssemblies);
    }

    [Test]
    public void SaveAssemblies ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));
      TypeFactory.GetConcreteType (typeof (List<int>));

      Assert.IsFalse (File.Exists (c_signedAssemblyFileName));
      Assert.IsFalse (File.Exists (c_unsignedAssemblyFileName));
      
      string[] paths = _moduleManager.SaveAssemblies ();

      Assert.AreEqual (2, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, c_signedAssemblyFileName), paths[0]);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, c_unsignedAssemblyFileName), paths[1]);
      
      Assert.IsTrue (File.Exists (c_signedAssemblyFileName));
      Assert.IsTrue (File.Exists (c_unsignedAssemblyFileName));
    }

    [Test]
    public void SaveUnsignedAssemblyWithDifferentNameAndPath ()
    {
      _moduleManager.UnsignedAssemblyName = "Foo";
      string path = Path.GetTempFileName();
      _moduleManager.UnsignedModulePath = path;
      File.Delete (path);

      TypeFactory.GetConcreteType (typeof (BaseType1));

      Assert.IsFalse (File.Exists (path));
      string[] actualPaths = _moduleManager.SaveAssemblies ();
      
      Assert.AreEqual (1, actualPaths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, path), actualPaths[0]);

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

    [Test]
    public void SaveSignedAssemblyWithDifferentNameAndPath ()
    {
      _moduleManager.SignedAssemblyName = "Bar";
      string path = Path.GetTempFileName ();
      _moduleManager.SignedModulePath = path;
      File.Delete (path);

      TypeFactory.GetConcreteType (typeof (List<int>));

      Assert.IsFalse (File.Exists (path));
      string[] actualPaths = _moduleManager.SaveAssemblies ();

      Assert.AreEqual (1, actualPaths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, path), actualPaths[0]);

      Assert.IsTrue (File.Exists (path));

      try
      {
        AssemblyName name = AssemblyName.GetAssemblyName (path);
        Assert.AreEqual ("Bar", name.Name);
      }
      finally
      {
        File.Delete (path);
      }
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The name can only be set before the first type is built.")]
    public void SettingSignedNameThrowsWhenTypeGenerated ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.SignedAssemblyName = "Foo";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The module path can only be set before the first type is built.")]
    public void SettingSignedPathThrowsWhenTypeGenerated ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.SignedModulePath = "Foo.dll";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The name can only be set before the first type is built.")]
    public void SettingUnsignedNameThrowsWhenTypeGenerated ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.UnsignedAssemblyName = "Foo";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The module path can only be set before the first type is built.")]
    public void SettingUnsignedPathThrowsWhenTypeGenerated ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.UnsignedModulePath = "Foo.dll";
    }

    [Test]
    public void SavedSignedAssemblyHasStrongName ()
    {
      TypeFactory.GetConcreteType (typeof (List<int>));

      _moduleManager.SaveAssemblies ();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_signedAssemblyFileName);
      
      Assert.IsNotNull (assemblyName.GetPublicKey ());
      Assert.AreNotEqual (0, assemblyName.GetPublicKey ().Length);
    }

    [Test]
    public void SavedUnsignedAssemblyHasWeakName ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));

      _moduleManager.SaveAssemblies ();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_unsignedAssemblyFileName);

      Assert.IsNull (assemblyName.GetPublicKey ());
    }

    [Test]
    public void SavedUnsignedAssemblyHasMixinAssemblyName ()
    {
      TypeFactory.GetConcreteType (typeof (BaseType1));

      _moduleManager.SaveAssemblies ();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_unsignedAssemblyFileName);

      Assert.AreEqual ("Rubicon.Mixins.Generated.Unsigned", assemblyName.Name);
    }

    [Test]
    public void SavedSignedAssemblyHasMixinAssemblyName ()
    {
      TypeFactory.GetConcreteType (typeof (List<int>));

      _moduleManager.SaveAssemblies ();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_signedAssemblyFileName);

      Assert.AreEqual ("Rubicon.Mixins.Generated.Signed", assemblyName.Name);
    }

    private void CheckForTypeInAssembly (string typeName, AssemblyName assemblyName)
    {
      AppDomainRunner.Run (delegate (object[] args)
      {
        AssemblyName assemblyToLoad = (AssemblyName) args[0];
        string typeToFind = (string) args[1];

        Assembly loadedAssembly = Assembly.Load (assemblyToLoad);
        Assert.IsNotNull (loadedAssembly.GetType (typeToFind));
      }, assemblyName, typeName);
    }

    [Test]
    public void SavedUnsignedAssemblyContainsGeneratedType ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      _moduleManager.SaveAssemblies ();

      CheckForTypeInAssembly (concreteType.FullName, AssemblyName.GetAssemblyName (c_unsignedAssemblyFileName));
    }


    [Test]
    public void SavedSignedAssemblyContainsGeneratedType ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (List<int>));
      _moduleManager.SaveAssemblies ();

      CheckForTypeInAssembly (concreteType.FullName, AssemblyName.GetAssemblyName (c_signedAssemblyFileName));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No types have been built, so no assembly has been generated.")]
    public void SaveThrowsWhenNoTypeCreated ()
    {
      _moduleManager.SaveAssemblies ();
    }
  }
}