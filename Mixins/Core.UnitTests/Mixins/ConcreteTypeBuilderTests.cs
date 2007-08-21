using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.CodeGeneration;
using System.Reflection;
using Rubicon.Development.UnitTesting;
using System.IO;
using System.Threading;
using Rubicon.Mixins.Definitions;
using System.Runtime.Serialization;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ConcreteTypeBuilderTests : MixinTestBase
  {
    [Test]
    public void TypesAreCached()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreSame (t1, t2);
    }

    [Test]
    public void CacheIsBoundToConcreteTypeBuilder ()
    {
      Type t1 = TypeFactory.GetConcreteType (typeof (BaseType1));
      ConcreteTypeBuilder.SetCurrent (null);
      Type t2 = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreNotSame (t1, t2);
    }

    [Test]
    public void CacheEvenWorksForSerialization ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1>().With();
      BaseType1 bt2 = ObjectFactory.Create<BaseType1> ().With ();

      Assert.AreSame (bt1.GetType(), bt2.GetType());

      BaseType1[] array = Serializer.SerializeAndDeserialize (new BaseType1[] {bt1, bt2});
      Assert.AreSame (bt1.GetType(), array[0].GetType());
      Assert.AreSame (array[0].GetType(), array[1].GetType());
    }

    [Test]
    public void GeneratedMixinTypesAreCached()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers>().With();
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType(), Mixin.Get<MixinWithAbstractMembers> (c2).GetType());
    }

    [Test]
    public void MixinTypeCacheIsBoundToConcreteTypeBuilder ()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      ConcreteTypeBuilder.SetCurrent (null);
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreNotSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType (), Mixin.Get<MixinWithAbstractMembers> (c2).GetType ());
    }

    [Test]
    public void MixinTypeCacheEvenWorksForSerialization ()
    {
      ClassOverridingMixinMembers c1 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();
      ClassOverridingMixinMembers c2 = ObjectFactory.Create<ClassOverridingMixinMembers> ().With ();

      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (c1).GetType (), Mixin.Get<MixinWithAbstractMembers> (c2).GetType ());

      ClassOverridingMixinMembers[] array = Serializer.SerializeAndDeserialize (new ClassOverridingMixinMembers[] { c1, c2 });
      Assert.AreSame (Mixin.Get<MixinWithAbstractMembers> (array[0]).GetType (), Mixin.Get<MixinWithAbstractMembers> (array[1]).GetType ());
    }

    [Test]
    public void CurrentIsGlobalSingleton ()
    {
      ConcreteTypeBuilder newBuilder = new ConcreteTypeBuilder ();
      Assert.IsFalse (ConcreteTypeBuilder.HasCurrent);
      Thread setterThread = new Thread ((ThreadStart) delegate { ConcreteTypeBuilder.SetCurrent (newBuilder); });
      setterThread.Start ();
      setterThread.Join ();

      Assert.IsTrue (ConcreteTypeBuilder.HasCurrent);
      Assert.AreSame (newBuilder, ConcreteTypeBuilder.Current);
    }

    [Test]
    public void LockAndAccessScope ()
    {
      IModuleManager scope = ConcreteTypeBuilder.Current.Scope;
      ConcreteTypeBuilder.Current.LockAndAccessScope(delegate (IModuleManager lockedScope)
      {
        Assert.AreSame (scope, lockedScope);
      });
    }

    [Test]
    public void DefaultNameProviderIsGuid ()
    {
      Assert.AreSame (GuidNameProvider.Instance, ConcreteTypeBuilder.Current.TypeNameProvider);
      Assert.AreSame (GuidNameProvider.Instance, ConcreteTypeBuilder.Current.MixinTypeNameProvider);
    }

    [Test]
    public void CanSaveAndResetScope ()
    {
      MockRepository repository = new MockRepository();
      IModuleManager managerMock = repository.CreateMock<IModuleManager>();

      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = managerMock;

      string[] paths = new string[] { "Foos", "Bars", "Stripes" };

      using (repository.Ordered ())
      {
        Expect.Call (managerMock.HasSignedAssembly).Return (false);
        Expect.Call (managerMock.HasUnsignedAssembly).Return (true);
        Expect.Call (managerMock.SaveAssemblies ()).Return (paths);
      }

      repository.ReplayAll ();

      builder.SaveAndResetDynamicScope ();

      repository.VerifyAll ();
    }

    [Test]
    public void HandlesSaveWithoutGeneratedTypesGracefully ()
    {
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      Assert.AreEqual (0, paths.Length);
    }

    [Test]
    public void ResetsScopeWhenSaving ()
    {
      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      IModuleManager scopeBefore = builder.Scope;
      builder.SaveAndResetDynamicScope ();
      Assert.AreNotSame (scopeBefore, builder.Scope);
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSaving ()
    {
      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      Assert.IsNotNull (builder.GetConcreteType (TypeFactory.GetActiveConfiguration (typeof (BaseType1), GenerationPolicy.ForceGeneration)));
      Assert.IsNotNull (builder.GetConcreteType (TypeFactory.GetActiveConfiguration (typeof (BaseType2), GenerationPolicy.ForceGeneration)));
      builder.SaveAndResetDynamicScope ();
      Assert.IsNotNull (builder.GetConcreteType (TypeFactory.GetActiveConfiguration (typeof (BaseType3), GenerationPolicy.ForceGeneration)));
    }

    [Test]
    public void SavingGeneratingCachingIntegration ()
    {
      Type concreteType1 = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      Assert.IsNotEmpty (paths);
      Type concreteType2 = TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration);
      paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      Assert.IsNotEmpty (paths);
      Type concreteType3 = TypeFactory.GetConcreteType (typeof (BaseType3), GenerationPolicy.ForceGeneration);

      Assert.AreSame (concreteType1, TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration));
      Assert.AreSame (concreteType2, TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration));
      Assert.AreSame (concreteType3, TypeFactory.GetConcreteType (typeof (BaseType3), GenerationPolicy.ForceGeneration));
    }

    [Test]
    public void LoadAddsLoadedBaseTypesToTheCache ()
    {
      string concreteTypeName = TypeFactory.GetConcreteType (typeof (BaseType1)).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      
      AppDomainRunner.Run (delegate (object[] args)
          {
            string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;

            MockRepository repository = new MockRepository ();
            IModuleManager moduleManagerMock = repository.CreateMock<IModuleManager> ();
            ConcreteTypeBuilder.Current.Scope = moduleManagerMock;

            // expecting _no_ actions on the scope when loading and accessing types from saved module

            repository.ReplayAll ();

            Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
            ConcreteTypeBuilder.Current.LoadScopeIntoCache (assembly);
            Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
            string expectedTypeName = (string) args[0];
            Assert.AreEqual (expectedTypeName, concreteType.FullName);
            Assert.AreSame (assembly.GetType (expectedTypeName), concreteType);

            repository.VerifyAll ();
          }, concreteTypeName);
    }

    [Test]
    public void LoadDoesntReplaceTypes ()
    {
      string concreteTypeName = TypeFactory.GetConcreteType (typeof (BaseType1)).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
          {
            Type concreteType1 = TypeFactory.GetConcreteType (typeof (BaseType1));

            string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;
            Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
            ConcreteTypeBuilder.Current.LoadScopeIntoCache (assembly);

            Type concreteType1b = TypeFactory.GetConcreteType (typeof (BaseType1));
            Assert.AreSame (concreteType1, concreteType1b);
            
            string outerTypeName = (string) args[0];
            Assert.AreNotSame (assembly.GetType (outerTypeName), concreteType1b);

          }, concreteTypeName);
    }

    [Test]
    public void LoadAddsLoadedMixinTypesToTheCache ()
    {
      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      string concreteTypeName = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
          {
            string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;

            MockRepository repository = new MockRepository ();
            IModuleManager moduleManagerMock = repository.CreateMock<IModuleManager> ();
            ConcreteTypeBuilder.Current.Scope = moduleManagerMock;

            // expecting _no_ actions on the scope when loading and accessing types from saved module

            repository.ReplayAll ();

            Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
            ConcreteTypeBuilder.Current.LoadScopeIntoCache (assembly);

            MixinDefinition innerMixinDefinition =
              TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
            Assert.IsNotNull (innerMixinDefinition);

            Type concreteType = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

            string expectedTypeName = (string) args[0];
            Assert.AreEqual (expectedTypeName, concreteType.FullName);
            Assert.AreSame (assembly.GetType (expectedTypeName), concreteType);

            repository.VerifyAll ();
          }, concreteTypeName);
    }

    [Test]
    public void LoadDoesntReplaceMixinTypes ()
    {
      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      string concreteTypeName = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).FullName;
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate (object[] args)
          {
            string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;

            MixinDefinition innerMixinDefinition =
              TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];

            Type concreteType1a = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

            Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
            ConcreteTypeBuilder.Current.LoadScopeIntoCache (assembly);

            Type concreteType1b = ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

            Assert.AreSame (concreteType1a, concreteType1b);

            string outerTypeName = (string) args[0];
            Assert.AreNotSame (assembly.GetType (outerTypeName), concreteType1b);
          }, concreteTypeName);
    }

    [Test]
    public void LoadStillAllowsGeneration ()
    {
      TypeFactory.GetConcreteType (typeof (ClassOverridingMixinMembers));
      MixinDefinition mixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
      ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();

      AppDomainRunner.Run (delegate
      {
        string modulePath = ConcreteTypeBuilder.Current.Scope.UnsignedModulePath;
        Assembly assembly = Assembly.Load (AssemblyName.GetAssemblyName (modulePath));
        ConcreteTypeBuilder.Current.LoadScopeIntoCache (assembly);

        MockRepository repository = new MockRepository ();

        IModuleManager moduleManagerMock = repository.CreateMock<IModuleManager> ();
        IModuleManager realScope = ConcreteTypeBuilder.Current.Scope;
        ConcreteTypeBuilder.Current.Scope = moduleManagerMock;

        TargetClassDefinition innerClassDefinition = TypeFactory.GetActiveConfiguration (typeof (BaseType2), GenerationPolicy.ForceGeneration);
        MixinDefinition innerMixinDefinition =
          TypeFactory.GetActiveConfiguration (typeof (ClassOverridingSingleMixinMethod)).Mixins[typeof (MixinWithSingleAbstractMethod)];

        using (repository.Ordered ())
        {
          Expect.Call (moduleManagerMock.CreateTypeGenerator (innerClassDefinition, GuidNameProvider.Instance, GuidNameProvider.Instance))
              .Return (realScope.CreateTypeGenerator (innerClassDefinition, GuidNameProvider.Instance, GuidNameProvider.Instance));
          Expect.Call (moduleManagerMock.CreateTypeGenerator (innerMixinDefinition.TargetClass, GuidNameProvider.Instance, GuidNameProvider.Instance))
              .Return (realScope.CreateTypeGenerator (innerMixinDefinition.TargetClass, GuidNameProvider.Instance, GuidNameProvider.Instance));
        }

        repository.ReplayAll ();

        // causes CreateTypeGenerator
        TypeFactory.GetConcreteType (typeof (BaseType2), GenerationPolicy.ForceGeneration);
        // causes CreateMixinTypeGenerator
        ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);

        // causes nothing, was loaded
        TypeFactory.GetConcreteType (typeof (ClassOverridingMixinMembers));
        // causes nothing, was loaded
        innerMixinDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
        ConcreteTypeBuilder.Current.GetConcreteMixinType (innerMixinDefinition);
        
        repository.VerifyAll ();
      });
    }

    public class BaseOverridingMixinMember
    {
      [Override]
      protected void Foo ()
      {
      }
    }

    public class MixinWithOverridableMember : Mixin<object>
    {
      protected virtual void Foo ()
      {
      }
    }

    [Test]
    public void GetConcreteMixinTypeBeforeGetConcreteTypeWorks ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseOverridingMixinMember), typeof (MixinWithOverridableMember)))
      {
        Type t = ConcreteTypeBuilder.Current.GetConcreteMixinType (TypeFactory.GetActiveConfiguration (typeof (BaseOverridingMixinMember)).Mixins[0]);
        Assert.IsNotNull (t);
        Assert.IsTrue (typeof (MixinWithOverridableMember).IsAssignableFrom (t));

        BaseOverridingMixinMember instance = ObjectFactory.Create<BaseOverridingMixinMember> ().With ();
        Assert.AreSame (t, Mixin.Get<MixinWithOverridableMember> (instance).GetType ());
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "No concrete mixin type is required for the given configuration "
        + "(mixin Rubicon.Mixins.UnitTests.Mixins.ConcreteTypeBuilderTests+MixinWithOverridableMember and target class System.Object).",
        MatchType = MessageMatch.Contains)]
    public void GetConcreteMixinTypeThrowsIfNoMixinTypeGenerated ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (MixinWithOverridableMember)))
      {
        ConcreteTypeBuilder.Current.GetConcreteMixinType (TypeFactory.GetActiveConfiguration (typeof (object)).Mixins[0]);
      }
    }

    [Test]
    public void InitializeUnconstructedInstanceDelegatesToScope ()
    {
      MockRepository mockRepository = new MockRepository();
      IMixinTarget mockMixinTarget = mockRepository.CreateMock<IMixinTarget>();
      IModuleManager mockScope = mockRepository.CreateMock<IModuleManager> ();
      
      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = mockScope;

      //expect
      mockScope.InitializeMixinTarget (mockMixinTarget);

      mockRepository.ReplayAll ();

      builder.InitializeUnconstructedInstance (mockMixinTarget);

      mockRepository.VerifyAll ();
    }

    [Test]
    public void BeginDeserializationDelegatesToScope ()
    {
      MockRepository mockRepository = new MockRepository ();

      Type deserializedType = typeof (object);
      IObjectReference objectReference = mockRepository.CreateMock<IObjectReference> ();
      SerializationInfo info = new SerializationInfo (deserializedType, new FormatterConverter ());
      StreamingContext context = new StreamingContext ();

      IModuleManager mockScope = mockRepository.CreateMock<IModuleManager> ();

      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = mockScope;

      Expect.Call (mockScope.BeginDeserialization (deserializedType, info, context)).Return (objectReference);

      mockRepository.ReplayAll ();

      Assert.AreSame (objectReference, builder.BeginDeserialization (deserializedType, info, context));

      mockRepository.VerifyAll ();
    }

    [Test]
    public void FinishDeserializationDelegatesToScope ()
    {
      MockRepository mockRepository = new MockRepository ();

      IObjectReference objectReference = mockRepository.CreateMock<IObjectReference> ();
      IModuleManager mockScope = mockRepository.CreateMock<IModuleManager> ();

      ConcreteTypeBuilder builder = new ConcreteTypeBuilder ();
      builder.Scope = mockScope;

      //expect
      mockScope.FinishDeserialization (objectReference);

      mockRepository.ReplayAll ();

      builder.FinishDeserialization (objectReference);

      mockRepository.VerifyAll ();
    }
  }
}
