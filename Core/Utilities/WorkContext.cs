using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Rubicon 
{

/// <summary>
///   Provides context information for error messages.
/// </summary>
/// <remarks>
///   <para>
///   Exceptions contain a stack trace of the time the exception was thrown, so it is easy to identify the code that 
///   caused the exception. However, there is no information as to which data this code was processing, or which iteration
///   of a certain loop caused a problem. For programs that process large amount of data and fail for specific data, it 
///   can be tedious to find the state or data that caused an error. 
///   <para></para>
///   WorkContexts provide an easy way to specify which data is currently being processed. In an exception handling block,
///   the current context stack can be used to get diagnostic information about the state of the application when the
///   exception occured. 
///   </para>
/// </remarks>
/// <example>
/// The following example demonstrates the use of WorkContexts to provide information about the data that is currently being
/// processed. Note the <c>using</c> statement and the call to <see cref="WorkContext.Done"/>. 
/// <code><![CDATA[
/// void f (string[] document)
/// {
///   try
///   {
///     for (int i = 0; i < document.Lenght; ++i)
///     {
///       using (WorkContext ctxLine = WorkContext.EnterNew ("Processing line {0}: \"{1}\".", i, line[i]))
///       {
///         Console.WriteLine (line[i].Trim()); // this causes a NullReferenceException if line[i] is a null reference
///         ctxLine.Done();
///       }
///     }
/// }
/// catch (Exception e)
/// {
///   Console.WriteLine ("Error \"{0}\" occured during:\n{1}", e.Message, WorkContext.Stack);
/// }
/// ]]></code>
/// </example>
public class WorkContext: IDisposable
{
  public class ContextStack
  {
    /// <summary> ArrayList &lt;WorkContext&gt; </summary>
    private ArrayList _stack = new ArrayList();
    private WorkContext _lastLeft = null;

    /// <summary>
    ///   The last WorkContext on the stack that was left by calling <see cref="Leave"/>.
    /// </summary>
    public WorkContext LastLeft
    {
      get { return _lastLeft; }
    }

    /// <summary>
    ///   Returns the context stack as an array of WorkContext objects.
    /// </summary>
    /// <returns>
    ///   The items on the context thread, with the top-level stack items first.
    /// </returns>
    public WorkContext[] ToArray()
    {
      return (WorkContext[]) _stack.ToArray (typeof (WorkContext)); 
    }

    /// <summary>
    ///   Returns a string representation of the context stack.
    /// </summary>
    /// <returns>
    ///   An string with the description of each context on the stack on a seperate line. Top-level contexts appear first.
    ///   Contexts that are already left, but not done, are marked with a question mark. See <see cref="Leave"/> and 
    ///   <see cref="Done"/>.
    /// </returns>
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

    internal void Push (WorkContext context)
    {
      if (_lastLeft != null)
      {
        PopIncluding (_lastLeft);
        _lastLeft = null;
      }

      _stack.Add (context);
    }

    internal void PopIncluding (WorkContext context)
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

    internal void Leave (WorkContext context)
    {
      _lastLeft = context;
    }
  }

  // static members

  /// <summary> Stack&lt;WorkContext&gt; </summary>
  [ThreadStatic]
  private static ContextStack _stack; // defaults to null for each new thread

  /// <summary>
  ///   Gets the work context stack of the current thread.
  /// </summary>
  /// <remarks>
  ///   The stack provides diagnostic information about the current execution context.
  /// </remarks>
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

  /// <summary>
  /// Creates a new context and puts it on the stack.
  /// </summary>
  /// <param name="text">The description of the context.</param>
  public static WorkContext EnterNew (string text)
  {
    WorkContext context = new WorkContext ();
    context.Enter (text);
    return context;
  }

  /// <summary>
  /// Creates a new context and puts it on the stack.
  /// </summary>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  public static WorkContext EnterNew (string format, params object[] args)
  {
    return EnterNew (string.Format (format, args));
  }

  // member fields

  private string _text;
  private bool _entered = false;

  // construction and disposal

  public WorkContext ()
  {
  }

  // methods and properties

  /// <summary>
  /// Enters a context.
  /// </summary>
  /// <param name="text">The description of the context.</param>
  [Conditional("TRACE")]
  void Enter (string text)
  {
    _text = text;
    _entered = true;
    Debug.WriteLine ("Enter Context: " + text);
    Debug.Indent();
    Stack.Push (this);
  }

  /// <summary>
  /// Enters a context.
  /// </summary>
  /// <param name="format">A string containing zero or more format items for the description of the context.</param>
  /// <param name="args">An array containing zero or more objects to format.</param>
  [Conditional("TRACE")]
  void Enter (string format, params object[] args)
  {
    Enter (string.Format (format, args));
  }

  /// <summary>
  /// Leaves the context.
  /// <seealso cref="Leave"/>.
  /// </summary>
  void IDisposable.Dispose ()
  {
    Leave();
  }

  /// <summary>
  ///   Leaves the context.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   The context is left, but remains on the stack until the method <see cref="Done"/> is called, or a new context is 
  ///   entered. Therefore, the left context is still available for inspection (e.g. in an exception handling block).
  ///   </para><para>
  ///   A context that is left, but not done, is prefixed with a question mark in the context stack output. Use this 
  ///   information if you are not sure whether all calls to <see cref="Done"/> were coded correctly.
  ///   </para><para>
  ///   In C# it is generally recommended to use a <c>using</c> statement rather than calling <c>Leave</c> explicitly.
  ///   </para>
  /// </remarks>
  [Conditional("TRACE")]
  public void Leave ()
  {
    if (_entered)
    {
      Debug.Unindent();
      Debug.WriteLine ("Leave Context: " + _text);
      Stack.Leave (this);
      _entered = false;
    }
  }

  /// <summary>
  ///   Marks the context as done.
  /// </summary>
  /// <remarks>
  ///   Call this method if the work within a context has been processed successfully (i.e., no uncaught exception has been 
  ///   raised).
  /// </remarks>
  [Conditional("TRACE")]
  public void Done()
  {
    if (_entered)
    {
      Debug.WriteLine ("Work done in Context: " + _text);
      Stack.PopIncluding (this);
    }
  }

  /// <summary>
  /// The description of the context.
  /// </summary>
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
    using (WorkContext ctxMain = WorkContext.EnterNew ("main"))
    {
      try
      {
        using (WorkContext ctxSub1 = WorkContext.EnterNew ("sub 1"))
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
        using (WorkContext ctxSub2 = WorkContext.EnterNew ("sub 2"))
        {
          using (WorkContext ctxSub2_1 = WorkContext.EnterNew ("sub 2.1"))
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
