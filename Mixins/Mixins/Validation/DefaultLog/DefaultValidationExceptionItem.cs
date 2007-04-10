using System;
using Rubicon.Utilities;

namespace Mixins.Validation.DefaultLog
{
  public struct DefaultValidationExceptionItem : IDefaultValidationResultItem
  {
    private IValidationRule _rule;
    private Exception _exception;

    public DefaultValidationExceptionItem (IValidationRule rule, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      ArgumentUtility.CheckNotNull ("exception", exception);

      _rule = rule;
      _exception = exception;
    }

    public IValidationRule Rule
    {
      get { return _rule; }
    }

    public string Message
    {
      get { return _exception.ToString (); }
    }

    public Exception Exception
    {
      get { return _exception; }
    }
  }
}