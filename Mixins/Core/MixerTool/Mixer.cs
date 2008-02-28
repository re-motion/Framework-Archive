using System;
using System.IO;
using Rubicon.Mixins.Validation;
using Rubicon.Reflection;
using Rubicon.Utilities;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Logging;
using Rubicon.Mixins.Context;
using System.Collections;

namespace Rubicon.Mixins.MixerTool
{
  public class Mixer
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (Mixer));

    public event EventHandler<ClassContextEventArgs> ClassContextBeingProcessed = delegate {};

    private readonly string _signedAssemblyName;
    private readonly string _unsignedAssemblyName;
    private readonly string _assemblyOutputDirectory;
    private INameProvider _nameProvider = GuidNameProvider.Instance;

    public Mixer (string signedAssemblyName, string unsignedAssemblyName, string assemblyOutputDirectory)
    {
      ArgumentUtility.CheckNotNull ("signedAssemblyName", signedAssemblyName);
      ArgumentUtility.CheckNotNull ("unsignedAssemblyName", unsignedAssemblyName);
      ArgumentUtility.CheckNotNull ("assemblyOutputDirectory", assemblyOutputDirectory);

      _signedAssemblyName = signedAssemblyName;
      _unsignedAssemblyName = unsignedAssemblyName;
      _assemblyOutputDirectory = assemblyOutputDirectory;
    }

    public INameProvider NameProvider
    {
      get { return _nameProvider; }
      set { _nameProvider = value; }
    }

    public void Execute ()
    {
      s_log.InfoFormat ("Base directory is '{0}'.", AppDomain.CurrentDomain.BaseDirectory);
      ConcreteTypeBuilder originalBuilder = ConcreteTypeBuilder.Current;
      try
      {
        Configure();
        Generate ();
        Save();
      }
      finally
      {
        ConcreteTypeBuilder.SetCurrent (originalBuilder);
      }
    }

    private void Configure ()
    {
      ConcreteTypeBuilder.SetCurrent (new ConcreteTypeBuilder ());

      ConcreteTypeBuilder.Current.TypeNameProvider = NameProvider;

      if (!Directory.Exists (_assemblyOutputDirectory))
        Directory.CreateDirectory (_assemblyOutputDirectory);

      ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName = _unsignedAssemblyName;
      ConcreteTypeBuilder.Current.Scope.UnsignedModulePath = Path.Combine (_assemblyOutputDirectory,
          ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName + ".dll");

      if (File.Exists (ConcreteTypeBuilder.Current.Scope.UnsignedModulePath))
        File.Delete (ConcreteTypeBuilder.Current.Scope.UnsignedModulePath);

      if (File.Exists (ConcreteTypeBuilder.Current.Scope.SignedModulePath))
        File.Delete (ConcreteTypeBuilder.Current.Scope.SignedModulePath);

      ConcreteTypeBuilder.Current.Scope.SignedAssemblyName = _signedAssemblyName;
      ConcreteTypeBuilder.Current.Scope.SignedModulePath = Path.Combine (_assemblyOutputDirectory,
          ConcreteTypeBuilder.Current.Scope.SignedAssemblyName + ".dll");
    }

    private void Generate ()
    {
      MixinConfiguration configuration = MixinConfiguration.ActiveConfiguration;
      ICollection typesToCheck = ContextAwareTypeDiscoveryService.GetInstance().GetTypes (null, false);
      
      s_log.InfoFormat ("Generating types for {0} configured mixin targets and {1} loaded types.", configuration.ClassContexts.Count, typesToCheck.Count);
      GenerateForConfiguredContexts(configuration);
      GenerateForInheritedContexts(configuration, typesToCheck);
    }

    private void GenerateForConfiguredContexts (MixinConfiguration configuration)
    {
      foreach (ClassContext classContext in configuration.ClassContexts)
      {
        if (classContext.Type.IsGenericTypeDefinition)
          s_log.WarnFormat ("Type {0} is a generic type definition and is thus ignored.", classContext.Type);
        else
          GenerateForClassContext (classContext);
      }
    }

    private void GenerateForInheritedContexts (MixinConfiguration configuration, ICollection typesToCheck)
    {
      foreach (Type type in typesToCheck)
      {
        if (type.IsGenericTypeDefinition)
          s_log.WarnFormat ("Type {0} is a generic type definition and is thus ignored.", type);
        else
        {
          ClassContext contextWithoutInheritance = configuration.ClassContexts.GetExact (type);
          ClassContext contextWithInheritance = configuration.ClassContexts.GetWithInheritance (type);
          if (contextWithoutInheritance == null && contextWithInheritance != null)
            GenerateForClassContext (contextWithInheritance);
        }
      }
    }

    private void GenerateForClassContext (ClassContext context)
    {
      try
      {
        ClassContextBeingProcessed (this, new ClassContextEventArgs (context));
        Type concreteType = TypeFactory.GetConcreteType (context.Type);
        s_log.InfoFormat ("{0} : {1}", context.ToString(), concreteType.FullName);
      }
      catch (ValidationException validationException)
      {
        s_log.ErrorFormat (validationException, "{0} : Validation error when generating type", context.ToString());
        ConsoleDumper.DumpValidationResults (validationException.ValidationLog.GetResults());
      }
      catch (Exception ex)
      {
        s_log.ErrorFormat (ex, "{0} : Unexpected error when generating type", context.ToString());
        using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
        {
          Console.WriteLine (ex.ToString());
        }
      }
    }

    private void Save ()
    {
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope();
      if (paths.Length == 0)
        s_log.Info ("No assemblies generated.");
      else
      {
        foreach (string path in paths)
        {
          s_log.InfoFormat ("Generated assembly file {0}.", path);
        }
      }
    }
  }
}