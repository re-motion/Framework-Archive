using System;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Mixins.Definitions;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  // This class is not safe for multi-threaded usage. When using it, ensure that its methods and properties are not used from multiple
  // threads at the same time.
  public class ModuleManager : IModuleManager
  {
    public const string DefaultWeakModulePath = "Rubicon.Mixins.Generated.Unsigned.dll";
    public const string DefaultStrongModulePath = "Rubicon.Mixins.Generated.Signed.dll";

    private string _weakAssemblyName = "Rubicon.Mixins.Generated.Unsigned";
    private string _weakModulePath = DefaultWeakModulePath;

    private string _strongAssemblyName = "Rubicon.Mixins.Generated.Signed";
    private string _strongModulePath = DefaultStrongModulePath;

    private ModuleScope _scope;

    public ITypeGenerator CreateTypeGenerator (TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("nameProvider", nameProvider);
      ArgumentUtility.CheckNotNull ("mixinNameProvider", mixinNameProvider);

      return new TypeGenerator (this, configuration, nameProvider, mixinNameProvider);
    }

    public string SignedAssemblyName
    {
      get { return _strongAssemblyName;  }
      set
      {
        if (HasSignedAssembly)
          throw new InvalidOperationException ("The name can only be set before the first type is built.");
        _strongAssemblyName = value;
      }
    }

    public string UnsignedAssemblyName
    {
      get { return _weakAssemblyName; }
      set
      {
        if (HasUnsignedAssembly)
          throw new InvalidOperationException ("The name can only be set before the first type is built.");
        _weakAssemblyName = value;
      }
    }

    public string SignedModulePath
    {
      get { return _strongModulePath; }
      set
      {
        if (HasSignedAssembly)
          throw new InvalidOperationException ("The module path can only be set before the first type is built.");
        _strongModulePath = value;
      }
    }

    public string UnsignedModulePath
    {
      get { return _weakModulePath; }
      set
      {
        if (HasUnsignedAssembly)
          throw new InvalidOperationException ("The module path can only be set before the first type is built.");
        _weakModulePath = value;
      }
    }

    internal ModuleScope Scope
    {
      get
      {
        if (_scope == null)
          _scope = new ModuleScope (true, _strongAssemblyName, _strongModulePath, _weakAssemblyName, _weakModulePath);
        return _scope;
      }
    }

    public bool HasSignedAssembly
    {
      get { return _scope != null && _scope.StrongNamedModule != null; }
    }

    public bool HasUnsignedAssembly
    {
      get { return _scope != null && _scope.WeakNamedModule != null; }
    }

    public bool HasAssemblies
    {
      get { return HasSignedAssembly || HasUnsignedAssembly; }
    }

    public string[] SaveAssemblies ()
    {
      if (!HasSignedAssembly && !HasUnsignedAssembly)
        throw new InvalidOperationException ("No types have been built, so no assembly has been generated.");
      else
        return AssemblySaver.SaveAssemblies (_scope);
    }

    public void InitializeMixinTarget (IMixinTarget target)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      GeneratedClassInstanceInitializer.InitializeMixinTarget (target);
    }

    public void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("mixinInstances", mixinInstances);

      GeneratedClassInstanceInitializer.InitializeDeserializedMixinTarget (instance, mixinInstances);
    }

    public IObjectReference BeginDeserialization (Type concreteDeserializedType, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("concreteDeserializedType", concreteDeserializedType);
      ArgumentUtility.CheckNotNull ("info", info);

      return new SerializationHelper (concreteDeserializedType, info, context);
    }

    public void FinishDeserialization (IObjectReference objectReference)
    {
      ArgumentUtility.CheckNotNull ("objectReference", objectReference);
      ArgumentUtility.CheckTypeIsAssignableFrom ("objectReference", objectReference.GetType (), typeof (IDeserializationCallback));

      ((IDeserializationCallback) objectReference).OnDeserialization (this);
    }
  }
}
