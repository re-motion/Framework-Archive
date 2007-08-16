using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Rubicon.Utilities;
using Rubicon.CodeGeneration.DPExtensions;
using System.IO;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  public class ModuleManager
  {
    public const string StrongAssemblyName = "Rubicon.Data.DomainObjects.Generated.Signed";
    public const string WeakAssemblyName = "Rubicon.Data.DomainObjects.Generated.Unsigned";

    private readonly string _directory;
    private ModuleScope _scope;

    public ModuleManager (string directory)
    {
      ArgumentUtility.CheckNotNull ("directory", directory);

      _directory = directory;
      _scope = CreateModuleScope();
    }

    private ModuleScope CreateModuleScope ()
    {
      return new ModuleScope (true, StrongAssemblyName, Path.Combine (_directory, StrongAssemblyName + ".dll"),
        WeakAssemblyName, Path.Combine (_directory, WeakAssemblyName + ".dll"));
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