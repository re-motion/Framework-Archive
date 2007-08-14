using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Rubicon.Utilities;
using Rubicon.CodeGeneration.DPExtensions;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  internal class ModuleManager
  {
    private const string _strongAssemblyName = "Rubicon.Data.DomainObjects.Generated.Signed";
    private const string _weakAssemblyName = "Rubicon.Data.DomainObjects.Generated.Unsigned";

    private ModuleScope _scope;

    public ModuleManager ()
    {
      _scope = CreateModuleScope();
    }

    private ModuleScope CreateModuleScope ()
    {
      return new ModuleScope (true, _strongAssemblyName, _strongAssemblyName + ".dll", _weakAssemblyName, _weakAssemblyName + ".dll");
    }

    public TypeGenerator CreateTypeGenerator (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      return new TypeGenerator (baseType, _scope);
    }

    public string[] SaveAssemblies ()
    {
      string[] paths = AssemblySaver.SaveAssemblies (_scope);
      _scope = CreateModuleScope ();
      return paths;
    }
  }
}