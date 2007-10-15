using System;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Validation
{
  [Serializable]
  public struct ValidationResultItem : IDefaultValidationResultItem
  {
    private static string FormatMessage (string message)
    {
      StringBuilder sb = new StringBuilder ();

      for (int i = 0; i < message.Length; ++i)
      {
        if (i > 0 && char.IsUpper (message[i]))
          sb.Append (' ').Append (char.ToLower (message[i]));
        else
          sb.Append (message[i]);
      }
      return sb.ToString ();
    }

    private readonly IValidationRule _rule;

    public ValidationResultItem (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      _rule = rule;
    }

    public IValidationRule Rule
    {
      get { return _rule; }
    }

    public string Message
    {
      get { return FormatMessage (Rule.Message); }
    }
  }
}