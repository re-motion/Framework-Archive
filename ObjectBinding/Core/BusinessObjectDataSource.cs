using System;
using System.Collections;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectDataSource
{
  bool IsWritable { get; }

  void Register (IBusinessObjectBoundControl control);
  void Unregister (IBusinessObjectBoundControl control);

  void LoadValues (object controlOrPage);
  void SaveValues (object controlOrPage);

  IBusinessObject BusinessObject { get; set; }
  IBusinessObjectClass BusinessObjectClass { get; }
}

public abstract class BusinessObjectDataSource: Component, IBusinessObjectDataSource
{
  private ArrayList _boundControls = null;
  private bool _editMode = true;
  // TODO: private bool _dataBindCalled = false;

  [Category ("Data")]
  public bool EditMode
  {
    get { return _editMode; }
    set { _editMode = value; }
  }

  bool IBusinessObjectDataSource.IsWritable
  {
    get { return EditMode; }
  }

  public void Register (IBusinessObjectBoundControl control)
  {
    if (_boundControls == null)
      _boundControls = new ArrayList (5);
    if (! _boundControls.Contains (control))
      _boundControls.Add (control);
  }

  public void Unregister (IBusinessObjectBoundControl control)
  {
    _boundControls.Remove (control);
  }

  protected void EnsureDataBind (object parentControl)
  {
    // TODO: remove parentControl altogether?
    // throw new NotSupportedException();
//    if (! _dataBindCalled)
//    {
//      parentControl.DataBind ();
//      _dataBindCalled = true;
//    }
  }

  public void LoadValues (object parentControl)
  {
    EnsureDataBind (parentControl);
    if (_boundControls != null)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
        control.LoadValue ();
    }
  }

  public void SaveValues (object parentControl)
  {
    EnsureDataBind (parentControl);
    if (_boundControls != null)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
      {
        IBusinessObjectBoundModifiableControl writeableControl = control as IBusinessObjectBoundModifiableControl;
        if (writeableControl != null)
          writeableControl.SaveValue ();
      }
    }
  }

  public abstract IBusinessObject BusinessObject { get; set; }
  public abstract IBusinessObjectClass BusinessObjectClass { get; }
}

}