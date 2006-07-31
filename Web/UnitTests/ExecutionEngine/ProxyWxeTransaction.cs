using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Development.UnitTesting;
using Rubicon.Data;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

  /// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactionBase{TTransaction}"/> type. </summary>
  [Serializable]
  public abstract class ProxyWxeTransaction : WxeTransactionBase<ITransaction>
  {
    public ProxyWxeTransaction ()
      : base (null, true, false)
    {
    }

    protected override ITransaction CreateRootTransaction ()
    {
      return Proxy_CreateRootTransaction ();
    }

    public abstract ITransaction Proxy_CreateRootTransaction ();

    protected override ITransaction CurrentTransaction
    {
      get { return Proxy_CurrentTransaction; }
    }

    public abstract ITransaction Proxy_CurrentTransaction { get;}

    protected override void SetCurrentTransaction (ITransaction transaction)
    {
      Proxy_SetCurrentTransaction (transaction);
    }

    public abstract void Proxy_SetCurrentTransaction (ITransaction transaction);

    public override void Execute (WxeContext context)
    {
      base.Execute (context);
    }

    protected override void OnTransactionCreating ()
    {
      Proxy_OnTransactionCreating ();
    }

    public virtual void Proxy_OnTransactionCreating ()
    {
      base.OnTransactionCreating ();
    }

    protected override void OnTransactionCreated (ITransaction transaction)
    {
      Proxy_OnTransactionCreated (transaction);
    }

    public virtual void Proxy_OnTransactionCreated (ITransaction transaction)
    {
      base.OnTransactionCreated (transaction);
    }

    protected override void OnTransactionCommitting ()
    {
      Proxy_OnTransactionCommitting ();
    }

    public virtual void Proxy_OnTransactionCommitting ()
    {
      base.OnTransactionCommitting ();
    }

    protected override void OnTransactionCommitted ()
    {
      Proxy_OnTransactionCommitted ();
    }

    public virtual void Proxy_OnTransactionCommitted ()
    {
      base.OnTransactionCommitted ();
    }

    protected override void OnTransactionRollingBack ()
    {
      Proxy_OnTransactionRollingBack ();
    }

    public virtual void Proxy_OnTransactionRollingBack ()
    {
      base.OnTransactionRollingBack ();
    }

    protected override void OnTransactionRolledBack ()
    {
      Proxy_OnTransactionRolledBack ();
    }

    public virtual void Proxy_OnTransactionRolledBack ()
    {
      base.OnTransactionRolledBack ();
    }
  }

}