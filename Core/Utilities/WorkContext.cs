using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Rubicon 
{

/// <summary>
/// Provides context information for error messages.
/// </summary>
public class WorkContext: IDisposable
{
  public class ContextStack
  {
    /// <summary> ArrayList &lt;WorkContext&gt; </summary>
    private ArrayList _stack = new ArrayList();
    private WorkContext _lastLeft = null;

    public WorkContext[] Stack
    {
      get { return (WorkContext[]) _stack.ToArray (typeof (WorkContext)); }
    }

    public override string ToString()
    {
      bool pastLast = false;
      StringBuilder sb = new StringBuilder();
      foreach (WorkContext context in _stack)
      {
        if (context == _lastLeft)
          pastLast = true;
        if (sb.Length > 0)
          sb.Append ('\n');
        if (pastLast)
          sb.Append ("? ");
        sb.Append (context.Text);
      }
      return sb.ToString();
    }

    public void Push (WorkContext context)
    {
      if (_lastLeft != null)
      {
        PopIncluding (_lastLeft);
        _lastLeft = null;
      }

      _stack.Add (context);
    }

    public void PopIncluding (WorkContext context)
    {
      int contextIndex = -1;
      for (int i = _stack.Count - 1; i >= 0; --i)
      {
        if (_stack[i] == context)
        {
          contextIndex = i;
          break;
        }
      }
      if (contextIndex == -1)
        return;

      object[] newStack = new object[contextIndex];
      _stack.CopyTo (0, newStack, 0, contextIndex);
      _stack = new ArrayList (newStack);
    }

    public void Leave (WorkContext context)
    {
      _lastLeft = context;
    }

  }

  // static members

  /// <summary> Stack&lt;WorkContext&gt; </summary>
  [ThreadStatic]
  private static ContextStack _stack; // defaults to null for each new thread

  /// <summary>
  /// Gets the current work context. (Stack&lt;WorkContext&gt;)
  /// </summary>
  public static ContextStack Stack
  {
    get 
    {
      ContextStack stack = _stack;
      if (stack == null)
      {
        stack = new ContextStack();
        _stack = stack;
      }
      return stack;
    }    
  }

  // member fields

  private string _text;

  // construction and disposal

	public WorkContext (string text)
	{
    Debug.WriteLine ("Enter Context: " + text);
    Debug.Indent();
    _text = text;
    Stack.Push (this);
	}

  // methods and properties

  void IDisposable.Dispose ()
  {
    Debug.Unindent();
    Debug.WriteLine ("Leave Context: " + _text);
    Stack.Leave (this);
    GC.SuppressFinalize (this);
  }

  public void Done()
  {
    Debug.WriteLine ("Work done in Context: " + _text);
    Stack.PopIncluding (this);
  }

  public string Text
  {
    get { return _text; }
    set { _text = value; }
  }
}

public class TestWorkContext
{
  public static void Test()
  {
    using (WorkContext ctxMain = new WorkContext ("main"))
    {
      try
      {
        using (WorkContext ctxSub1 = new WorkContext ("sub 1"))
        {
          throw new Exception ("text exception 1");

          ctxSub1.Done();
        }
      }
      catch (Exception e)
      {
        string context = WorkContext.Stack.ToString();
        Console.WriteLine (e.Message + "\n" + e.StackTrace + "\n\nContext:\n" + context);
      }
      try
      {
        using (WorkContext ctxSub2 = new WorkContext ("sub 2"))
        {
          using (WorkContext ctxSub2_1 = new WorkContext ("sub 2.1"))
          {
            //ctxSub2_1.Done();
          }
            throw new Exception ("text exception 2");
          ctxSub2.Done();
        }
      }
      catch (Exception e)
      {
        string context = WorkContext.Stack.ToString();
        Console.WriteLine (e.Message + "\n" + e.StackTrace + "\n\nContext:\n" + context);
      }

      ctxMain.Done();
    }
  }
}

}
