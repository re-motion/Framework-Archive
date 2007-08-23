using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using System.Collections;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
  /// <summary>
  /// A <see cref="WxeFunction"/> that will always have a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
  /// </summary>
  /// <remarks>
  /// <para>A <b>WxeTransactedFunction</b> always creates a new <see cref="ClientTransaction"/>, unless <see cref="WxeTransactionMode.None"/>
  /// is passed to the constructor. <see cref="DomainObject">DomainObjects</see> passed to a <see cref="WxeTransactedFunction"/> as In parameters are
  /// automatically enlisted in the new transaction; <see cref="DomainObject">DomainObjects</see> returned as Out parameters are automatically
  /// enlisted in the surrounding transaction (if any).
  /// </para>
  /// <para>
  /// Override <see cref="CreateRootTransaction"/> if you wish to replace the default behavior of creating new <see cref="ClientTransaction"/>
  /// instances.
  /// </para>
  /// <para>A <see cref="WxeTransactedFunction"/> has <see cref="AutoCommit"/> set to <see langword="true"/> by default. <br />
  /// To change this behavior for a function you can overwrite this property.</para>
  /// </remarks>
  [Serializable]
  public class WxeTransactedFunction : WxeTransactedFunctionBase<ClientTransaction>
  {
    private WxeTransactionMode _transactionMode;

    /// <summary>
    /// Creates a new <b>WxeTransactedFunction</b> that has a new <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeTransactedFunction (params object[] actualParameters)
        : this (WxeTransactionMode.CreateRoot, actualParameters)
    {
    }

    /// <summary>
    /// Creates a new <b>WxeTransactedFunction</b>
    /// </summary>
    /// <param name="transactionMode">A value indicating the behavior of the WxeTransactedFunction.</param>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeTransactedFunction (WxeTransactionMode transactionMode, params object[] actualParameters)
        : base (actualParameters)
    {
      ArgumentUtility.CheckValidEnumValue ("transactionMode", transactionMode);

      _transactionMode = transactionMode;
    }

    /// <summary> Gets the underlying <see cref="ClientTransaction"/> owned by this 
    /// <see cref="WxeTransactedFunctionBase{TTransaction}"/>.</summary>
    public new ClientTransaction MyTransaction
    {
      get { return base.MyTransaction; }
    }

    /// <summary> Gets the underlying <see cref="ClientTransaction"/> used when this <see cref="WxeTransactedFunctionBase{TTransaction}"/>
    /// is executed. This is either <see cref="MyTransaction"/> or, when this function does not have an own transaction, the
    /// <see cref="Transaction"/> of this function's parent function.</summary>
    public new ClientTransaction Transaction
    {
      get { return base.Transaction; }
    }

    /// <summary>
    /// Gets or sets the <see cref="WxeTransactionMode"/> of the <see cref="WxeTransactedFunction"/>.
    /// </summary>
    /// <remarks>The property must not be set after execution of the function has started.</remarks>
    /// <exception cref="System.InvalidOperationException">An attempt to set the property is performed after execution of the function has started.</exception>
    public WxeTransactionMode TransactionMode
    {
      get { return _transactionMode; }
      set
      {
        ArgumentUtility.CheckValidEnumValue ("transactionMode", value);

        if (ExecutionStarted)
          throw new InvalidOperationException ("CreateTransactionMode must not be set after execution of this function has started.");

        _transactionMode = value;
      }
    }

    /// <summary>
    /// Creates a new <see cref="WxeTransaction"/> depending on the value of <see cref="WxeTransactionMode"/>. 
    /// </summary>
    /// <returns>A new WxeTransaction, if <see cref="WxeTransactionMode"/> has a value of <b>CreateRoot</b>; otherwise <see langword="null"/>.</returns>
    protected override sealed WxeTransactionBase<ClientTransaction> CreateWxeTransaction ()
    {
      switch (_transactionMode)
      {
        case WxeTransactionMode.CreateRoot:
          return CreateWxeTransaction (AutoCommit, true);
        case WxeTransactionMode.CreateChildIfParent:
          return CreateWxeTransaction (AutoCommit, false);
        default:
          return null;
      }
    }

    /// <summary>
    /// Creates a new <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> object.
    /// </summary>
    /// <returns>A new <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.</returns>
    /// <remarks>Derived class should override this method to provide specific implemenations of <see cref="ClientTransaction"/>s.</remarks>
    protected override ClientTransaction CreateRootTransaction ()
    {
      return ClientTransaction.NewTransaction ();
    }

    /// <summary>
    /// Creates a new <see cref="WxeTransaction"/>. Derived classes should override this method to use their own <see cref="WxeTransaction"/>.
    /// </summary>
    /// <param name="autoCommit"><b>autoCommit</b> is forwarded to <see cref="WxeTransaction"/>'s constructor. For further information see <see cref="WxeTransaction"/>.</param>
    /// <param name="forceRoot"><b>forceRoot</b> is forwarded to <see cref="WxeTransaction"/>'s constructor. For further information see <see cref="WxeTransaction"/>.</param>
    /// <returns>A new WxeTransaction.</returns>
    protected virtual WxeTransaction CreateWxeTransaction (bool autoCommit, bool forceRoot)
    {
      return new WxeTransaction (autoCommit, forceRoot);
    }

    /// <summary>
    /// Gets a value indicating if the function performs an automatic commit on the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <note type="inheritinfo">Overwrite this property to change the behavior of the function.</note>
    protected virtual bool AutoCommit
    {
      get { return true; }
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been created.
    /// </summary>
    /// <param name="transaction">The transaction that has been created.</param>
    protected override void OnTransactionCreated (ClientTransaction transaction)
    {
      Assertion.IsNotNull (transaction);
      EnlistInParameters (transaction);
      base.OnTransactionCreated (transaction);
    }

    /// <summary>
    /// Executes the current function.
    /// </summary>
    /// <param name="context">The execution context.</param>
    public override void Execute (WxeContext context)
    {
      base.Execute (context);
      if (ClientTransactionScope.HasCurrentTransaction)
        EnlistOutParameters (ClientTransactionScope.CurrentTransaction);
    }

    private void EnlistInParameters (ClientTransaction transaction)
    {
      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        if (parameterDeclaration.IsIn)
        {
          object parameter = parameterDeclaration.GetValue (Variables);
          EnlistParameter (parameterDeclaration, parameter, transaction);
        }
      }
    }

    private void EnlistOutParameters (ClientTransaction transaction)
    {
      WxeParameterDeclaration[] parameterDeclarations = ParameterDeclarations;
      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        WxeParameterDeclaration parameterDeclaration = parameterDeclarations[i];
        if (parameterDeclaration.IsOut)
        {
          object parameter = parameterDeclaration.GetValue (Variables);
          EnlistParameter (parameterDeclaration, parameter, transaction);
        }
      }
    }

    private void EnlistParameter (WxeParameterDeclaration parameterDeclaration, object parameter, ClientTransaction transaction)
    {
      if (!TryEnlistAsDomainObject (parameterDeclaration, parameter as DomainObject, transaction))
        TryEnlistAsEnumerable (parameterDeclaration, parameter as IEnumerable, transaction);
    }

    private bool TryEnlistAsDomainObject (WxeParameterDeclaration parameterDeclaration, DomainObject domainObject, ClientTransaction transaction)
    {
      if (domainObject != null)
      {
        EnlistDomainObject (transaction, domainObject, parameterDeclaration);
        return true;
      }
      else
        return false;
    }

    private bool TryEnlistAsEnumerable (WxeParameterDeclaration parameterDeclaration, IEnumerable enumerable, ClientTransaction transaction)
    {
      if (enumerable != null)
      {
        foreach (object innerParameter in enumerable)
          EnlistParameter (parameterDeclaration, innerParameter, transaction);
        return true;
      }
      else
        return false;
    }

    private void EnlistDomainObject (ClientTransaction transaction, DomainObject domainObject, WxeParameterDeclaration parameterDeclaration)
    {
      try
      {
        transaction.EnlistDomainObject (domainObject);
      }
      catch (Exception ex)
      {
        string message = string.Format (
            "The domain object '{0}' cannot be enlisted in the function's transaction. Maybe it was newly created "
            + "and has not yet been committed, or it was deleted.",
            domainObject.ID);
        throw new ArgumentException (message, parameterDeclaration.Name, ex);
      }
    }
  }
}