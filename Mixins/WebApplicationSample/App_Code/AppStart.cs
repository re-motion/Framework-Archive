using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Rubicon.Mixins;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;

namespace WebApplicationSample.App_Code
{
  public static class AppStart
  {
    public static void AppInitialize ()
    {
      Trace.WriteLine ("Entering AppInitialize", "INFO");
      Dictionary<string, Type> baseNameToConcreteTypeMap = PrepareMixinAssembly ();
      HostingEnvironment.RegisterVirtualPathProvider (new MixinAwareVirtualPathProvider (baseNameToConcreteTypeMap));
    }

    private static Dictionary<string, Type> PrepareMixinAssembly ()
    {
      INameProvider previousTypeNameProvider = ConcreteTypeBuilder.Current.TypeNameProvider;
      ConcreteTypeBuilder.Current.TypeNameProvider = NamespaceChangingNameProvider.Instance;

      string targetDirectory = GetMixinAssemblyTargetDirectory ();
      ConcreteTypeBuilder.Current.Scope.SignedModulePath =
          Path.Combine (targetDirectory, ConcreteTypeBuilder.Current.Scope.SignedAssemblyName + ".dll");
      ConcreteTypeBuilder.Current.Scope.UnsignedModulePath =
          Path.Combine (targetDirectory, ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName + ".dll");

      string[] paths;
      Dictionary<string, Type> baseNameToConcreteTypeMap;
      try
      {
        baseNameToConcreteTypeMap = GenerateTypesForMixinAssembly ();
        paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      }
      finally
      {
        ConcreteTypeBuilder.Current.TypeNameProvider = previousTypeNameProvider;
      }

      Trace.WriteLine (string.Format ("Generated {0} mixin assemblies", paths.Length), "INFO");
      foreach (string path in paths)
        Trace.WriteLine (path, "INFO");

      return baseNameToConcreteTypeMap;
    }

    private static string GetMixinAssemblyTargetDirectory ()
    {
      Uri currentAssemblyUri = new Uri (Assembly.GetExecutingAssembly ().CodeBase);
      return Path.GetDirectoryName (currentAssemblyUri.LocalPath);
    }

    private static Dictionary<string, Type> GenerateTypesForMixinAssembly ()
    {
      Dictionary<string, Type> baseNameToConcreteTypeMap = new Dictionary<string, Type> ();

      foreach (ClassContext configuredClass in MixinConfiguration.ActiveContext.ClassContexts)
      {
        if (configuredClass.Type.IsGenericTypeDefinition)
        {
          Trace.WriteLine ("Cannot prepare a mixed type for the generic type definition '" + configuredClass.Type.FullName + "'.", "WARN");
        }
        else
        {
          BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (configuredClass.Type);

          // prepare type for class
          Type mixedType = ConcreteTypeBuilder.Current.GetConcreteType (baseClassDefinition);
          baseNameToConcreteTypeMap.Add (baseClassDefinition.FullName, mixedType);

          Trace.WriteLine (string.Format ("{0} -> {1} ({2} mixins)", baseClassDefinition.FullName, mixedType.FullName,
              baseClassDefinition.Mixins.Count), "INFO");

          foreach (MixinDefinition mixinDefinition in baseClassDefinition.Mixins)
          {
            if (mixinDefinition.HasOverriddenMembers ())
            {
              // prepare type for mixin with overridden members
              Type mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
              baseNameToConcreteTypeMap.Add (mixinDefinition.FullName, mixinType);

              Trace.WriteLine (string.Format ("{0} -> {1}", mixinDefinition.FullName, mixinType.FullName));
            }
          }
        }
      }

      return baseNameToConcreteTypeMap;
    }
  }
}
