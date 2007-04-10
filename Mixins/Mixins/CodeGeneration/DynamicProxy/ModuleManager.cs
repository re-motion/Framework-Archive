using System;
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
  }
}
