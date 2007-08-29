using System;
using System.IO;
using Rubicon.Utilities;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Logging;
using Rubicon.Mixins.Context;

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

      ConcreteTypeBuilder.Current.Scope.SignedAssemblyName = _signedAssemblyName;
      ConcreteTypeBuilder.Current.Scope.SignedModulePath = Path.Combine (_assemblyOutputDirectory,
          ConcreteTypeBuilder.Current.Scope.SignedAssemblyName + ".dll");
    }

    private void Generate ()
    {
      ApplicationContext configuration = MixinConfiguration.ActiveContext;
      s_log.InfoFormat ("Generating types for {0} configured mixin targets.", configuration.ClassContextCount);
      foreach (ClassContext context in configuration.ClassContexts)
      {
        if (context.Type.IsGenericTypeDefinition)
          s_log.WarnFormat ("Type {0} is a generic type definition and is thus ignored.", context.Type);
        else
        {
          try
          {
            ClassContextBeingProcessed (this, new ClassContextEventArgs (context));
            Type concreteType = TypeFactory.GetConcreteType (context.Type);
            s_log.InfoFormat ("{0} : {1}", context.ToString (), concreteType.FullName);
          }
          catch (Exception ex)
          {
            s_log.ErrorFormat (ex, "{0} : Error when generating type", context.ToString ());
          }
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