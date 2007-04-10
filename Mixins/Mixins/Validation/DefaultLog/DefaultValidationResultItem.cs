using System;
using Rubicon.Utilities;

namespace Mixins.Validation.DefaultLog
{
  public struct DefaultValidationResultItem : IDefaultValidationResultItem
  {
    private IValidationRule _rule;

    public DefaultValidationResultItem (IValidationRule rule)
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
      get { return Rule.Message; }
    }
  }
}