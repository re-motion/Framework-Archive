using System;
using log4net;
using Rubicon.Data;

namespace Rubicon.Data
{
  public interface ITransaction
  {
    void Commit();
    void Rollback();
    bool CanCreateChild { get; }
    ITransaction CreateChild();
    ITransaction CreateAndRegisterChild();
    ITransaction UnregisterChild();
  }
}
namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
/// Creates a scope for a transaction.
/// </summary>
[Serializable]
public abstract class WxeTransactionBase: WxeStepList
{
  /// <summary>
  ///   Finds out wheter the specified step is part of a WxeTransactionBase.
  /// </summary>
  /// <returns> True, if one of the parents of the specified Step is a WxeTransactionBase, false otherwise. </returns>
  public static bool HasWxeTransaction (WxeStep step)
  {
    for (;
         step != null;
         step = step.ParentStep)
    {
      if (step is WxeTransactionBase)
        return true;
    }
    return false;
  }

  private static ILog s_log = LogManager.GetLogger (typeof (WxeTransactionBase));

  private ITransaction _transaction = null;
  private ITransaction _previousTransaction = null;
  private bool _autoCommit;
  private bool _forceRoot;

  /// <summary> Creates a new instance. </summary>
  /// <param name="steps"> Initial step list. Can be <see langword="null"/>. </param>
  /// <param name="autoCommit">
  ///   If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back. 
  /// </param>
  /// <param name="forceRoot"> 
  ///   If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists. 
  /// </param>
  /// <remarks>
  ///   If the <see cref="ITransaction"/> implementation does not support child transactions 
  /// </remarks>
  public WxeTransactionBase (WxeStepList steps, bool autoCommit, bool forceRoot)
  {
    _autoCommit = autoCommit;
    _forceRoot = forceRoot;
    if (steps != null)
      AddStepList (steps);
  }

  protected abstract ITransaction CreateTransaction();
  protected abstract ITransaction GetCurrentTransaction();
  protected abstract void SetCurrentTransaction (ITransaction transaction);

  public override void Execute (WxeContext context)
  {
    s_log.Debug ("Starting Execute of " + this.GetType().Name);
    
    if (_transaction == null)
    {
      WxeTransactionBase parentTransaction = ParentTransaction;
      
      if (   ! _forceRoot 
          && parentTransaction != null 
          && parentTransaction.Transaction != null)
      {
        _previousTransaction = parentTransaction.Transaction;
        if (parentTransaction.Transaction.CanCreateChild)
          _transaction = parentTransaction.Transaction.CreateChild();
        else
          _transaction = CreateTransaction();

        s_log.Debug ("Created child " + this.GetType().Name + ".");
      }
      else
      {
        _previousTransaction = GetCurrentTransaction();
        _transaction = CreateTransaction();

        s_log.Debug ("Created root " + this.GetType().Name + ".");
      }
    }

    SetCurrentTransaction (_transaction);

    try
    {
      base.Execute (context);
    }
    catch (Exception e)
    {
      if (e is System.Threading.ThreadAbortException)
        throw;

      RollbackTransaction();
      s_log.Debug ("Aborted Execute of " + this.GetType().Name + " because of exception: \"" + e.Message + "\" (" + e.GetType().FullName + ").");
  
      throw;
    }

    if (_autoCommit)
      CommitTransaction();
    else
      RollbackTransaction();

    // TODO: Abort entire function?
    
    s_log.Debug ("Finished Execute of " + this.GetType().Name);
  }

  /// <summary> Gets the first parent of type <see cref="WxeTransactionBase"/>. </summary>
  /// <value> 
  ///   A <see cref="WxeTransactionBase"/> object or <see langword="null"/> if the current transaction is the 
  ///   topmost transaction.
  /// </value>
  protected WxeTransactionBase ParentTransaction
  {
    get { return (WxeTransactionBase) WxeStep.GetStepByType (this, typeof (WxeTransactionBase)); }
  }

  public ITransaction Transaction
  {
    get { return _transaction; } 
  }

  protected override void AbortRecursive()
  {
    s_log.Debug ("Aborting " + this.GetType().Name);
    base.AbortRecursive();
    RollbackTransaction();
  }

  private void CommitTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Committing " + _transaction.GetType().Name + ".");
      _transaction.Commit();
      _transaction = null;
      SetCurrentTransaction (_previousTransaction);
    }
  }

  private void RollbackTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Rolling back " + _transaction.GetType().Name + ".");
      _transaction.Rollback();
      _transaction = null;
      SetCurrentTransaction (_previousTransaction);
    }
  }
}

}
