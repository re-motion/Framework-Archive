using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Mixins.Definitions;
using System.IO;

namespace Mixins.CodeGeneration.DynamicProxy
{
  public class ModuleManager : IModuleManager
  {
    private ModuleScope _scope = new ModuleScope ();

    public ITypeGenerator CreateTypeGenerator (BaseClassDefinition configuration)
    {
      return new TypeGenerator (this, configuration);
    }

    public IMixinTypeGenerator CreateMixinTypeGenerator (MixinDefinition configuration)
    {
      return new MixinTypeGenerator (this, configuration);
    }

    internal ModuleScope Scope
    {
      get { return _scope; }
    }

    public string SaveAssembly ()
    {
      try
      {
        _scope.SaveAssembly ();
      }
      catch (NullReferenceException)
      {
        return null;
      }
      return Path.Combine (Environment.CurrentDirectory, ModuleScope.FILE_NAME);
    }

    public void InitializeInstance (object instance)
    {
      GeneratedClassInstanceInitializer.InitializeInstanceFields (instance);
    }

    public void InitializeInstance (object instance, object[] extensions)
    {
      GeneratedClassInstanceInitializer.InitializeInstanceFields (instance, extensions);
    }

    public void InitializeInstanceWithMixins (object instance, object[] mixinInstances)
    {
      GeneratedClassInstanceInitializer.InitializeInstanceFieldsWithMixins (instance, mixinInstances);
    }

    public void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, object targetInstance)
    {
      GeneratedClassInstanceInitializer.InitializeMixinInstance (mixinDefinition, mixinInstance, targetInstance);
    }
  }
}
