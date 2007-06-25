using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Rubicon.Mixins.Definitions;
using System.IO;

namespace Rubicon.Mixins.CodeGeneration.DynamicProxy
{
  public class ModuleManager : IModuleManager
  {
    private ModuleScope _scope = new ModuleScope (true);

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

    public void InitializeInstance (IMixinTarget instance)
    {
      GeneratedClassInstanceInitializer.InitializeInstanceFields (instance);
    }

    public void InitializeInstance (IMixinTarget instance, object[] extensions)
    {
      GeneratedClassInstanceInitializer.InitializeInstanceFields (instance, extensions);
    }

    public void InitializeInstanceWithMixins (IMixinTarget instance, object[] mixinInstances)
    {
      GeneratedClassInstanceInitializer.InitializeInstanceFieldsWithMixins (instance, mixinInstances);
    }

    public void InitializeMixinInstance (MixinDefinition mixinDefinition, object mixinInstance, IMixinTarget targetInstance)
    {
      GeneratedClassInstanceInitializer.InitializeMixinInstance (mixinDefinition, mixinInstance, targetInstance);
    }
  }
}
