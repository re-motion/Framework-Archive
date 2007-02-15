using System;
using System.ComponentModel;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> Performs a single operation in a web application as part of a <see cref="WxeFunction"/>. </summary>
/// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/Class/*' />
[Serializable]
public abstract class WxeStep
{
  /// <summary> Gets the <see cref="WxeFunction"/> for the passed <see cref="WxeStep"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/GetFunction/*' />
  public static WxeFunction GetFunction (WxeStep step)
  {
    return (WxeFunction) WxeStep.GetStepByType (step, typeof (WxeFunction));
  }

  /// <summary> Gets the first step of the specified <paramref name="type"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/GetStepByType/*' />
  protected static WxeStep GetStepByType (WxeStep step, Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    Type expectedType = typeof (WxeStep);
    if (! expectedType.IsAssignableFrom (type))
      throw new ArgumentTypeException ("type", expectedType, type);

    for (; 
          step != null; 
          step = step.ParentStep)
    {
      if (type.IsAssignableFrom (step.GetType()))
        return step;
    }
    return null;
  }

  /// <summary> Used to pass a variable by reference to a <see cref="WxeFunction"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/varref/*' />
  protected static WxeVariableReference varref (string localVariable)
  {
    return new WxeVariableReference (localVariable);
  }

  private WxeStep _parentStep = null;
  private bool _isAborted = false;
  /// <summary> 
  ///   <see langword="true"/> during the execution of <see cref="Abort"/>. Used to prevent circular aborting.
  /// </summary>
  [NonSerialized]
  private bool _isAborting = false;

  /// <overloads>
  /// <summary> Executes the <see cref="WxeStep"/>. </summary>
  /// <remarks> This method should only be invoked by the WXE infrastructure. </remarks>
  /// </overloads>
  /// <summary> Executes the <see cref="WxeStep"/>. </summary>
  /// <remarks> 
  ///   Invokes <see cref="M:Rubicon.Web.ExecutionEngine.WxeStep.Execute(Rubicon.Web.ExecutionEngine.WxeContext">WxeContext</see>,
  ///   passing the <see cref="WxeContext.Current"/> <see cref="WxeContext"/> as argument.
  ///   <note>
  ///     This method should only be invoked by the WXE infrastructure.
  ///   </note>
  /// </remarks>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void Execute ()
  {
    Execute (WxeContext.Current);
  }

  /// <summary> Executes the <see cref="WxeStep"/>. </summary>
  /// <param name="context"> The <see cref="WxeContext"/> containing the information about the execution. </param>
  /// <remarks> 
  ///   Override this method to implement your execution logic. 
  ///   <note>
  ///     This method should only be invoked by the WXE infrastructure.
  ///   </note>
  /// </remarks>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public abstract void Execute (WxeContext context);

  /// <summary> Gets the scope's variables collection. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/Variables/*' />
  public virtual NameObjectCollection Variables
  {
    get { return (_parentStep == null) ? null : _parentStep.Variables; }
  }

  /// <summary> Gets the parent step of the the <see cref="WxeStep"/>. </summary>
  /// <value> The <see cref="WxeStep"/> assigned using <see cref="SetParentStep"/>. </value>
  public WxeStep ParentStep
  {
    get { return _parentStep; }
  }

  /// <summary> Sets the parent step of this <see cref="WxeStep"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/SetParentStep/*' />
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void SetParentStep (WxeStep parentStep)
  {
    ArgumentUtility.CheckNotNull ("parentStep", parentStep);
    _parentStep = parentStep;
  }

  /// <summary> Gets the step currently being executed. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/ExecutingStep/*' />
  public virtual WxeStep ExecutingStep 
  {
    get { return this; }
  }

  /// <summary> Gets the root <see cref="WxeFunction"/> of the execution hierarchy. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/RootFunction/*' />
  public WxeFunction RootFunction
  {
    get
    {
      WxeStep step = this;
      while (step.ParentStep != null)
        step = step.ParentStep;
      return step as WxeFunction;
    }
  }

  /// <summary> Gets the parent <see cref="WxeFunction"/> for this <see cref="WxeStep"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/ParentFunction/*' />
  public WxeFunction ParentFunction
  {
    get { return WxeStep.GetFunction (ParentStep); }
  }

  /// <summary> 
  ///   Gets the <see cref="Exception"/> caught by the <see cref="WxeTryCatch"/> block encapsulating this 
  ///   <see cref="WxeStep"/>.
  /// </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/CurrentException/*' />
  protected Exception CurrentException
  {
    get 
    {
      for (WxeStep step = this;
           step != null;
           step = step.ParentStep)
      {
        if (step is WxeCatchBlock)
          return ((WxeCatchBlock) step).Exception;
      }

      return null;
    }
  }

  /// <summary> Gets a flag describing whether the <see cref="WxeStep"/> has been aborted. </summary>
  /// <value> <see langword="true"/> once <see cref="AbortRecursive"/> as finished executing. </value>
  public bool IsAborted
  {
    get { return _isAborted; }
  }
  
  /// <summary> Aborts the <b>WxeStep</b> by calling <see cref="AbortRecursive"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/Abort/*' />
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void Abort()
  {
    if (! _isAborted && ! _isAborting)
    {
      _isAborting = true;
      AbortRecursive();
      _isAborted = true;
      _isAborting = false;
    }
  }

  /// <summary> Contains the aborting logic for the <see cref="WxeStep"/>. </summary>
  /// <include file='doc\include\ExecutionEngine\WxeStep.xml' path='WxeStep/AbortRecursive/*' />
  protected virtual void AbortRecursive()
  {
  }
}

}
