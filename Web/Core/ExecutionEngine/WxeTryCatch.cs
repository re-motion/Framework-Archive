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

public class WxeCatchBlock: WxeStepList
{
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

/// <summary>
///   Try-Catch block.
/// </summary>
public class WxeTryCatch: WxeStep
{
  private int _executingCatchBlock = -1; // index of currently executing catch block, or -1 if the try block is executing.
  private WxeStepList _trySteps;
  private Exception _currentException = null;

  /// <summary> ArrayList&lt;WxeCatchBlock&gt; </summary>
  private ArrayList _catchBlocks;

  public WxeTryCatch (Type tryStepListType, params Type[] catchBlockTypes)
  {
    _trySteps = (WxeStepList) Activator.CreateInstance (tryStepListType);
    _trySteps.ParentStep = this;

    _catchBlocks = new ArrayList();
    foreach (Type catchBlockType in catchBlockTypes)
      Add ((WxeCatchBlock) Activator.CreateInstance (catchBlockType));
  }

  public WxeTryCatch()
  {
    _catchBlocks = new ArrayList();
    _trySteps = new WxeStepList();
    _trySteps.ParentStep = this;
  }

  public override void Execute (WxeContext context)
  {
    if (IsExecutingTryBlock)
    {
      try
      {
        _trySteps.Execute (context);
      }
      catch (Exception e)
      {
        if (e is System.Threading.ThreadAbortException)
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
            break;
          }
        }

        if (_executingCatchBlock == -1)
          throw;
        else
        {
          _currentException = e;
          ExecutingStepList.Execute (context);
        }
      }
    }
    else
    {
      ExecutingStepList.Execute (context);
    }
  }

  public override WxeStep ExecutingStep
  {
    get { return ExecutingStepList.ExecutingStep; }
  }

  private bool IsExecutingTryBlock
  {
    get { return _executingCatchBlock == -1; }
  }

  private WxeStepList ExecutingStepList
  {
    get
    {
      if (IsExecutingTryBlock)
        return _trySteps;
      else
        return (WxeCatchBlock)_catchBlocks[_executingCatchBlock];
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

  public Exception CurrentException 
  {
    get { return _currentException; }
  }
}

}
