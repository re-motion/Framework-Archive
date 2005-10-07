using System;
using Rubicon.Data;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[Serializable]
public class TestTransaction: ITransaction
{
  [ThreadStatic]
  private static TestTransaction _current;

  public static TestTransaction Current
  {
    get { return _current; }
    set { _current = value; }
  }

  private bool _isRolledBack;
  private bool _isCommitted;
  private bool _isReleased;
  private bool _canCreateChild;
  private TestTransaction _child;
  private TestTransaction _parent;
  private bool _isChild;

  public event EventHandler Committed;
  public event EventHandler RolledBack;

  public TestTransaction()
  {
  }

  public void Rollback()
  {
    if (Current != this)
      throw new InvalidOperationException ("Current transaction is not this transaction.");
    _isRolledBack = true;
    if (RolledBack != null)
      RolledBack (this, EventArgs.Empty);
  }

  public void Commit()
  {
    if (Current != this)
      throw new InvalidOperationException ("Current transaction is not this transaction.");
    _isCommitted = true;
    if (Committed != null)
      Committed (this, EventArgs.Empty);
  }

  public ITransaction CreateChild()
  {
    _child = new TestTransaction();
    _child._isChild = true;
    _child._parent = this;
    return _child;
  }

  public bool IsChild
  {
    get { return _isChild; }
  }

  public bool CanCreateChild
  {
    get { return _canCreateChild; }
    set { _canCreateChild = value; }
  }

  public void Release()
  {
    if (_child != null)
    { 
      _child._parent = null;
      _child = null;
    }
    _isReleased = true;
  }

  public ITransaction Parent
  {
    get { return _parent; }
  }

  public bool IsRolledBack
  {
    get { return _isRolledBack; }
  }

  public bool IsCommitted
  {
    get { return _isCommitted; }
  }

  public bool IsReleased
  {
    get { return _isReleased; }
  }
}

}