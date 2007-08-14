using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Mixins.Definitions;
using System.IO;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  // This class is not safe for multi-threaded usage. When using it, ensure that its methods and properties are not used from multiple
  // threads at the same time.
  public class ModuleManager : IModuleManager
  {
    private string _weakAssemblyName = "Rubicon.Mixins.Generated.Unsigned";
    private string _weakModulePath = "Rubicon.Mixins.Generated.Unsigned.dll";

    private string _strongAssemblyName = "Rubicon.Mixins.Generated.Signed";
    private string _strongModulePath = "Rubicon.Mixins.Generated.Signed.dll";

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

    public void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances)
    {
      GeneratedClassInstanceInitializer.InitializeDeserializedMixinTarget (instance, mixinInstances);
    }
  }
}
