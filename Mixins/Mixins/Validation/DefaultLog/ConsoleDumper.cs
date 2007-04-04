using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Validation.DefaultLog
{
  public static class ConsoleDumper
  {
    public static void DumpLog (DefaultValidationLog log)
    {
      foreach (DefaultValidationResult result in log.Results)
      {
        if (result.TotalRulesExecuted == 0)
        {
          Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Gray;
        }
        Console.WriteLine ("{0} for {1}, {2} rules executed", result.Definition.GetType ().Name, result.Definition.FullName, result.TotalRulesExecuted);
        DumpResultList ("unexpected exceptions", result.Exceptions, ConsoleColor.White, ConsoleColor.DarkRed);
        DumpResultList ("successes", result.Successes, ConsoleColor.Green, ConsoleColor.Black);
        DumpResultList ("warnings", result.Failures, ConsoleColor.Yellow, ConsoleColor.Black);
        DumpResultList ("failures", result.Failures, ConsoleColor.Red, ConsoleColor.Black);
        Console.WriteLine ();
        Console.ForegroundColor = ConsoleColor.Gray;
      }
    }

    private static void DumpResultList<T> (string title, List<T> resultList, ConsoleColor foreColor, ConsoleColor backColor) where T : IDefaultValidationResultItem
    {
      if (resultList.Count > 0)
      {
        Console.ForegroundColor = foreColor;
        Console.BackgroundColor = backColor;

        Console.WriteLine ("  {0} - {1}", title, resultList.Count);
        foreach (T resultItem in resultList)
        {
          Console.WriteLine ("    {0} ({1})", resultItem.Message, resultItem.Rule.RuleName);
        }
      }
    }
  }
}
