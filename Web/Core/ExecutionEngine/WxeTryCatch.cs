using System;
using System.Collections;
using System.Reflection;

namespace Rubicon.Web.ExecutionEngine
{

[AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class WxeExceptionAttribute: Attribute
{
  public static WxeExceptionAttribute GetAttribute (Type type)
  {
    WxeExceptionAttribute[] attributes = (WxeExceptionAttribute[]) type.GetCustomAttributes (typeof (WxeExceptionAttribute), false);
    if (attributes == null || attributes.Length == 0)
      return null;
    return attributes[0];
  }

  private Type _exceptionType;

  public WxeExceptionAttribute (Type exceptionType)
  {
    _exceptionType = exceptionType;
  }

  public Type ExceptionType
  {
    get { return _exceptionType; }
  }
}


/// <summary>
///   Try-Catch block.
/// </summary>
public class WxeTryCatch: WxeStep
{
  /// <summary>
  /// index of currently executing catch block, or -1 if the try block is executing, -2 if finally block is executing
  /// </summary>
  private int _executingCatchBlock = -1; 

  private WxeStepList _trySteps;

  /// <summary> ArrayList&lt;WxeCatchBlock&gt; </summary>
  private ArrayList _catchBlocks;

  private WxeStepList _finallySteps;

  public WxeTryCatch (Type tryStepListType, Type finallyStepListType, params Type[] catchBlockTypes)
  {
    _trySteps = (WxeStepList) Activator.CreateInstance (tryStepListType);
    _trySteps.ParentStep = this;

    _catchBlocks = new ArrayList();
    if (catchBlockTypes != null)
    {
      foreach (Type catchBlockType in catchBlockTypes)
        Add ((WxeCatchBlock) Activator.CreateInstance (catchBlockType));
    }

    if (finallyStepListType != null)
    {
      _finallySteps = (WxeStepList) Activator.CreateInstance (finallyStepListType);
      _finallySteps.ParentStep = this;
    }
    else
    {
      _finallySteps = null;
    }
  }

  public WxeTryCatch()
  {
    InitializeSteps();
  }

  private void InitializeSteps ()
  {
    Type type = this.GetType();

    Type tryBlockType = type.GetNestedType ("Try", BindingFlags.Public | BindingFlags.NonPublic);
    if (tryBlockType == null)
      throw new ApplicationException ("Try/catch block type " + type.FullName + " has no nested type named \"Try\".");
    _trySteps = (WxeStepList) Activator.CreateInstance (tryBlockType);
    _trySteps.ParentStep = this;

    Type finallyBlockType = type.GetNestedType ("Finally", BindingFlags.Public | BindingFlags.NonPublic);
    if (finallyBlockType != null)
    {
      _finallySteps = (WxeStepList) Activator.CreateInstance (finallyBlockType);
      _finallySteps.ParentStep = this;
    }
    else
    {
      _finallySteps = null;
    }

    MemberInfo[] catchBlockTypes = NumberedMemberFinder.FindMembers (
        type, 
        "Catch",
        MemberTypes.NestedType,
        BindingFlags.Public | BindingFlags.NonPublic);

    _catchBlocks = new ArrayList();
    foreach (Type catchBlockType in catchBlockTypes)
      Add ((WxeCatchBlock) Activator.CreateInstance (catchBlockType));
  }

  public override void Execute (WxeContext context)
  {
    if (_executingCatchBlock == -1) // tryBlock
    {
      try
      {
        _trySteps.Execute (context);
      }
      catch (Exception e)
      {
        if (e is System.Threading.ThreadAbortException || e is WxeExecuteNextStepException)
          throw;

        // if the exception was wrapped by the framework, get the original exception
        if (e is System.Web.HttpException)
        {
          e = e.InnerException;
          if (e is System.Web.HttpUnhandledException)
            e = e.InnerException;
        }

        for (int i = 0; i < _catchBlocks.Count; ++i)
        {
          WxeCatchBlock catchBlock = (WxeCatchBlock) _catchBlocks[i];
          if (catchBlock.ExceptionType.IsAssignableFrom (e.GetType()))
          {
            _executingCatchBlock = i;
            catchBlock.Exception = e;
            break;
          }
        }

        if (_executingCatchBlock == -1)
          throw;

        ExecutingStepList.Execute (context);
      }

      if (_finallySteps != null)
      {
        _executingCatchBlock = -2;
        ExecutingStepList.Execute (context);
      }
    }
    else if (_executingCatchBlock == -2) // finallyBlock
    {
      _finallySteps.Execute (context);
    }
    else
    {
      ExecutingStepList.Execute (context);
      if (_finallySteps != null)
      {
        _executingCatchBlock = -2;
        _finallySteps.Execute (context);
      }
    }
  }

  public override WxeStep ExecutingStep
  {
    get { return ExecutingStepList.ExecutingStep; }
  }

  private WxeStepList ExecutingStepList
  {
    get
    {
      switch (_executingCatchBlock)
      {
        case -1:
          return _trySteps;
        case -2:
          return _finallySteps;
        default:
          return (WxeCatchBlock)_catchBlocks[_executingCatchBlock];
      }
    }
  }

  public void Add (WxeCatchBlock catchBlock)
  {
    _catchBlocks.Add (catchBlock);
    catchBlock.ParentStep = this;
  }

  public WxeStepList TrySteps
  {
    get { return _trySteps; }
  }

  public WxeCatchBlock[] CatchBlocks
  {
    get { return (WxeCatchBlock[]) _catchBlocks.ToArray (typeof (WxeCatchBlock)); }
  }

  protected override void Dispose (bool disposing)
  {
    _trySteps.Dispose ();

    if (_catchBlocks != null)
    {
      foreach (WxeStepList catchBlock in _catchBlocks)
        catchBlock.Dispose();
    }

    if (_finallySteps != null)
      _finallySteps.Dispose();
  }
}

public class WxeTryBlock: WxeStepList
{
}

public class WxeFinallyBlock: WxeStepList
{
}

public class WxeCatchBlock: WxeStepList
{
  private Exception _exception = null;

  public Exception Exception 
  {
    get { return _exception; }
    set { _exception = value; }
  }

  public virtual Type ExceptionType 
  { 
    get 
    { 
      WxeExceptionAttribute exceptionAttribute = WxeExceptionAttribute.GetAttribute (this.GetType());
      if (exceptionAttribute == null)
        return typeof (Exception); 
      else
        return exceptionAttribute.ExceptionType;
    }
  }
}

}
