using System;
using System.Collections;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

using System.Drawing.Design;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

public interface IObjectBindingDataSource
{
  bool IsWritable { get; }

  void Register (IBusinessObjectBoundControl control);
  void Unregister (IBusinessObjectBoundControl control);

  /// <summary>
  ///   Load the values of the business object into all registered controls.
  /// </summary>
  /// <remarks>
  ///   On initial loads, all values must be loaded. On interim loads, each control decides whether it keeps its own 
  ///   value (e.g., using view state) or whether it reloads the value (useful for complex structures that need no 
  ///   validation).
  /// </remarks>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  void LoadValues (bool interim);

  /// <summary>
  ///   Save the values of the business object from all registered controls.
  /// </summary>
  /// <remarks>
  ///   On final saves, all values must be saved. (It is assumed that invalid values were already identified using 
  ///   validators.) On interim saves, each control decides whether it saves its values into the business object or
  ///   using an alternate mechanism (e.g. view state).
  /// </remarks>
  /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
  void SaveValues (bool interim);

  IBusinessObjectClass BusinessObjectClass { get; }
  IBusinessObjectProvider BusinessObjectProvider { get; }
}

public interface IBusinessObjectListDataSource
{
  IList BusinessObjectList { get; set; }
}

public interface IBusinessObjectDataSource: IObjectBindingDataSource
{
  IBusinessObject BusinessObject { get; set; }
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

  [Browsable (false)]
  bool IObjectBindingDataSource.IsWritable
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
    if (_boundControls != null)
      _boundControls.Remove (control);
  }

  public void LoadValues (bool interim)
  {
    if (_boundControls != null)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
        control.LoadValue (interim);
    }
  }

  public void SaveValues (bool interim)
  {
    if (_boundControls != null)
    {
      foreach (IBusinessObjectBoundControl control in _boundControls)
      {
        IBusinessObjectBoundModifiableControl writeableControl = control as IBusinessObjectBoundModifiableControl;
        if (writeableControl != null)
          writeableControl.SaveValue (interim);
      }
    }
  }

  [Browsable (false)]
  public abstract IBusinessObject BusinessObject { get; set; }

  [Browsable (false)]
  public abstract IBusinessObjectClass BusinessObjectClass { get; }

  [Browsable (false)]
  public virtual IBusinessObjectProvider BusinessObjectProvider 
  { 
    get { return (BusinessObjectClass == null) ? null : BusinessObjectClass.BusinessObjectProvider; }
  }
}

}