using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Rubicon;
using Rubicon.Utilities;

namespace Mixins.Utilities.Singleton
{
  public class CallContextSingleton<T>
  {
    private readonly string _callContextKey;
    private Func<T> _creator;

    public CallContextSingleton(string callContextKey, Func<T> creator)
    {
      ArgumentUtility.CheckNotNull ("callContextKey", callContextKey);
      ArgumentUtility.CheckNotNull ("creator", creator);

      _callContextKey = callContextKey;
      _creator = creator;
    }

    public bool HasCurrent
    {
      get { return GetCurrentInternal () != null; }
    }

    public T Current
    {
      get
      {
        if (!HasCurrent)
          SetCurrent (_creator());

        return GetCurrentInternal ();
      }
    }

    public void SetCurrent (T value)
    {
      CallContext.SetData (_callContextKey, value);
    }

    private T GetCurrentInternal ()
    {
      return (T) CallContext.GetData (_callContextKey);
    }
  }
}
