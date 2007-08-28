using System;
using System.IO;
using Rubicon.Utilities;
using Rubicon.Logging;

namespace Rubicon.Mixins.MixerTool
{
  [Serializable]
  public class MixerRunner : AppDomainRunnerBase
  {
    private static AppDomainSetup CreateAppDomainSetup (MixerParameters parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      AppDomainSetup setup = new AppDomainSetup ();
      setup.ApplicationName = "Mixer";
      setup.ApplicationBase = parameters.BaseDirectory;
      setup.DynamicBase = Path.Combine (Path.GetTempPath (), "Rubicon"); // necessary for AppDomainRunnerBase and AppDomainRunnerBase

      if (!string.IsNullOrEmpty (parameters.ConfigFile))
      {
        setup.ConfigurationFile = parameters.ConfigFile;
        if (!File.Exists (setup.ConfigurationFile))
        {
          throw new FileNotFoundException (
              string.Format (
                  "The configuration file supplied by the 'config' parameter was not found.\r\nFile: {0}",
                  setup.ConfigurationFile),
              setup.ConfigurationFile);
        }
      }
      return setup;
    }

    private readonly MixerParameters _parameters;

    public MixerRunner (MixerParameters parameters)
        : base (CreateAppDomainSetup (ArgumentUtility.CheckNotNull ("parameters", parameters)))
    {
      _parameters = parameters;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      if (_parameters.Verbose)
        LogManager.InitializeConsole ();

      Mixer mixer = new Mixer (_parameters.SignedAssemblyName, _parameters.UnsignedAssemblyName, _parameters.AssemblyOutputDirectory);
      try
      {
        mixer.Execute ();
      }
      catch (Exception ex)
      {
        Console.WriteLine (ex.Message);
      }
    }
  }
}