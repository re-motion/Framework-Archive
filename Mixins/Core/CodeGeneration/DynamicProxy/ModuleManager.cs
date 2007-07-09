using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Rubicon.Mixins.Definitions;
using System.IO;
using Rubicon.Utilities;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  public class ModuleManager : IModuleManager
  {
    private string _weakAssemblyName = "Rubicon.Mixins.Generated.Unsigned";
    private string _weakModulePath = "Rubicon.Mixins.Generated.Unsigned.dll";

    private string _strongAssemblyName = "Rubicon.Mixins.Generated.Signed";
    private string _strongModulePath = "Rubicon.Mixins.Generated.Signed.dll";

    private ModuleScope _scope;

    public ITypeGenerator CreateTypeGenerator (BaseClassDefinition configuration)
    {
      return new TypeGenerator (this, configuration);
    }

    public IMixinTypeGenerator CreateMixinTypeGenerator (MixinDefinition configuration)
    {
      return new MixinTypeGenerator (this, configuration);
    }

    public string AssemblyName
    {
      get { return _strongAssemblyName;  }
      set
      {
        if (_scope != null && HasAssembly)
          throw new InvalidOperationException ("The name can only be set before the first type is built.");
        _strongAssemblyName = value;
      }
    }

    public string ModulePath
    {
      get { return _strongModulePath; }
      set
      {
        if (_scope != null && HasAssembly)
          throw new InvalidOperationException ("The module path can only be set before the first type is built.");
        _strongModulePath = value;
      }
    }

    internal ModuleScope Scope
    {
      get
      {
        if (_scope == null)
        {
          _scope = new ModuleScope (true, _strongAssemblyName, _strongModulePath, _weakAssemblyName, _weakModulePath);
        }
        return _scope;
      }
    }

    public bool HasAssembly
    {
      get
      {
        Assertion.Assert (Scope.WeakNamedModule == null, "Because all generated types implement the interface IMixinTarget, which stems from "
            + " an assembly with a strong name, DynamicProxy will also always generate strongly named assemblies.");
        return Scope.StrongNamedModule != null;
      }
    }

    public string SaveAssembly ()
    {
      Assertion.Assert (Scope.WeakNamedModule == null, "Because all generated types implement the interface IMixinTarget, which stems from "
          + " an assembly with a strong name, DynamicProxy will also always generate strongly named assemblies.");

      try
      {
        Scope.SaveAssembly (true);
        return Scope.StrongNamedModule.FullyQualifiedName;
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException ("No types have been built, so no assembly has been generated.", ex);
      }
    }

    public void InitializeMixinTarget (IMixinTarget instance)
    {
      GeneratedClassInstanceInitializer.InitializeMixinTarget (instance);
    }

    public void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances)
    {
      GeneratedClassInstanceInitializer.InitializeDeserializedMixinTarget (instance, mixinInstances);
    }
  }
}
