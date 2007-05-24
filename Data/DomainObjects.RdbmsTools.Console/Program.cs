using System;
using System.IO;
using System.Reflection;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.RdbmsTools.SchemaGeneration;
using Rubicon.Text.CommandLine;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.RdbmsTools.Console
{
  [Serializable]
  public class Program
  {
    private static int Main (string[] args)
    {
      Arguments arguments;
      CommandLineClassParser parser = new CommandLineClassParser (typeof (Arguments));
      try
      {
        arguments = (Arguments) parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine (e.Message);
        System.Console.WriteLine ("Usage:");
        System.Console.WriteLine (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs()[0], System.Console.BufferWidth));
        return 1;
      }

      Program program = new Program (AppDomain.CurrentDomain.SetupInformation, arguments);
      AppDomainSetup appDomainSetup = new AppDomainSetup();
      appDomainSetup.ApplicationName = "RdbmsTools";
      appDomainSetup.ApplicationBase = arguments.BaseDirectory;
      appDomainSetup.DynamicBase = Path.Combine (Path.GetTempPath(), "Rubicon");
      AppDomain appDomain = null;
      try
      {
        appDomain = AppDomain.CreateDomain ("RdbmsTools", AppDomain.CurrentDomain.Evidence, appDomainSetup);
        Directory.CreateDirectory (appDomain.DynamicDirectory);
        CopyAssemblies (appDomain.DynamicDirectory);
        appDomain.AssemblyResolve += new ResolveEventHandler (program.AppDomain_AssemblyResolve);
        appDomain.DoCallBack (program.Run);
      }
      catch (Exception e)
      {
        if (arguments.Verbose)
        {
          System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
          for (; e != null; e = e.InnerException)
            System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace);
        }
        else
        {
          System.Console.Error.WriteLine ("Execution aborted: {0}", e.Message);
        }
        return 1;
      }
      finally
      {
        if (appDomain != null)
        {
          AppDomain.Unload (appDomain);
        }
        if (Directory.Exists (appDomainSetup.DynamicBase))
          Directory.Delete (appDomainSetup.DynamicBase, true);
      }
      return 0;
    }

    private static void CopyAssemblies (string dynamicDirectory)
    {
      CopyAssembly (dynamicDirectory, typeof (Program).Assembly.Location);
    }

    private static void CopyAssembly (string dynamicDirectory, string assemblyLocation)
    {
      string destinationFileName = Path.Combine (dynamicDirectory, Path.GetFileName (assemblyLocation));
      if (File.Exists (destinationFileName))
        File.Delete (destinationFileName);
      File.Copy (assemblyLocation, destinationFileName);
    }

    private readonly AppDomainSetup _parentAppDomainSetup;
    private readonly Arguments _arguments;

    private Program (AppDomainSetup parentAppDomainSetup, Arguments arguments)
    {
      _parentAppDomainSetup = parentAppDomainSetup;
      _arguments = arguments;
    }

    private void Run ()
    {
      PersistenceConfiguration persistenceConfiguration = DomainObjectsConfiguration.Current.Storage;
      if (persistenceConfiguration.StorageProviderDefinition == null)
      {
        ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
        RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Default", typeof (SqlProvider), "Initial Catalog=DatebaseName;");
        storageProviderDefinitionCollection.Add (providerDefinition);

        persistenceConfiguration = new PersistenceConfiguration (storageProviderDefinitionCollection, providerDefinition);
        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (DomainObjectsConfiguration.Current.MappingLoader, persistenceConfiguration));
      }

      MappingConfiguration mappingConfiguration = MappingConfiguration.Current;

      if ((_arguments.Mode & OperationMode.BuildSchema) != 0)
      {
        Type sqlFileBuilderType = TypeUtility.GetType (_arguments.SchemaFileBuilderTypeName, true, false);
        FileBuilderBase.Build (sqlFileBuilderType, mappingConfiguration, persistenceConfiguration, _arguments.SchemaOutputDirectory);
      }
    }

    private Assembly AppDomain_AssemblyResolve (object sender, ResolveEventArgs args)
    {
      AssemblyName assemblyName = new AssemblyName (args.Name);

      string assemblyLocation = Path.Combine (_parentAppDomainSetup.ApplicationBase, assemblyName.Name + ".dll");
      if (File.Exists (assemblyLocation))
      {
        Assembly reflectionOnlyAssembly = Assembly.ReflectionOnlyLoadFrom (assemblyLocation);
        if (reflectionOnlyAssembly.FullName != args.Name)
          return null;
        CopyAssembly (AppDomain.CurrentDomain.DynamicDirectory, assemblyLocation);
      }

      return Assembly.Load (assemblyName);
    }
  }
}