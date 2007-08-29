using System;
using Rubicon.Mixins.MixerTool;
using Rubicon.Text.CommandLine;

namespace Rubicon.Mixins.MixerTool.Console
{
  class Program
  {
    static int Main (string[] args)
    {
      MixerParameters parameters;
      CommandLineClassParser<MixerParameters> parser = new CommandLineClassParser<MixerParameters> ();
      try
      {
        parameters = parser.Parse (args);
      }
      catch (CommandLineArgumentException e)
      {
        System.Console.WriteLine (e.Message);
        System.Console.WriteLine ("Usage:");
        System.Console.WriteLine (parser.GetAsciiSynopsis (Environment.GetCommandLineArgs ()[0], System.Console.BufferWidth));
        return 1;
      }

      try
      {
        MixerRunner mixerRunner = new MixerRunner (parameters);
        mixerRunner.Run ();
      }
      catch (Exception e)
      {
        System.Console.Error.WriteLine ("Execution aborted. Exception stack:");
        for (; e != null; e = e.InnerException)
          System.Console.Error.WriteLine ("{0}: {1}\n{2}", e.GetType ().FullName, e.Message, e.StackTrace);
        return 1;
      }
      return 0;
    }
  }
}
