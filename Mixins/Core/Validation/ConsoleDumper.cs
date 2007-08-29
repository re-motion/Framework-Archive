using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Validation;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Validation
{
  public static class ConsoleDumper
  {
    public static void DumpValidationResults (IEnumerable<ValidationResult> results)
    {
      ArgumentUtility.CheckNotNull ("results", results);

      foreach (ValidationResult result in results)
      {
        if (result.TotalRulesExecuted == 0)
        {
          //Console.ForegroundColor = ConsoleColor.DarkGray;
          //Console.WriteLine ("No rules found for {0} '{1}'", result.Definition.GetType ().Name, result.Definition.FullName);
        }
        else if (result.TotalRulesExecuted != result.Successes.Count)
        {
          Console.ForegroundColor = ConsoleColor.Gray;
          Console.WriteLine ("{0} '{1}', {2} rules executed", result.Definition.GetType().Name, result.Definition.FullName, result.TotalRulesExecuted);
          DumpContext (result);
        }
        DumpResultList ("unexpected exceptions", result.Exceptions, ConsoleColor.White, ConsoleColor.DarkRed);
        // DumpResultList ("successes", result.Successes, ConsoleColor.Green, ConsoleColor.Black);
        DumpResultList ("warnings", result.Warnings, ConsoleColor.Yellow, ConsoleColor.Black);
        DumpResultList ("failures", result.Failures, ConsoleColor.Red, ConsoleColor.Black);
        Console.ForegroundColor = ConsoleColor.Gray;
      }
    }

    private static void DumpContext (ValidationResult result)
    {
      string contextString = result.GetParentDefinitionString();
      if (contextString.Length > 0)
        Console.WriteLine ("Context: " + contextString);
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