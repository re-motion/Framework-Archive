using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.NullableValueTypes;
using Rubicon.Collections;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

public interface IBusinessObjectDataSource
{
  bool EditMode { get; set; }

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

  IBusinessObject BusinessObject { get; set; }

  IBusinessObjectBoundControl[] BoundControls { get; }
}

public abstract class BusinessObjectDataSource: Component, IBusinessObjectDataSource
{
  private TypedArrayList _boundControls = new TypedArrayList (typeof (IBusinessObjectBoundControl));
  private bool _editMode = true;

  [Category ("Data")]
  public bool EditMode
  {
    get { return _editMode; }
    set { _editMode = value; }
  }

  public void Register (IBusinessObjectBoundControl control)
  {
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
      {
        if (control.IsValid)
          control.LoadValue (interim);
      }
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
        {
          if (writeableControl.IsValid)
            writeableControl.SaveValue (interim);
        }

      }
    }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public abstract IBusinessObjectClass BusinessObjectClass { get; }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public virtual IBusinessObjectProvider BusinessObjectProvider 
  { 
    get { return (BusinessObjectClass == null) ? null : BusinessObjectClass.BusinessObjectProvider; }
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public abstract IBusinessObject BusinessObject { get; set; }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectBoundControl[] BoundControls
  {
    get 
    {
      ArrayList bindableControls = new ArrayList (_boundControls.Count);
      for (int i = 0; i < _boundControls.Count; ++i)
      {
        IBusinessObjectBoundControl control = (IBusinessObjectBoundControl) _boundControls[i];
        if (control.IsValid)
          bindableControls.Add (control);
      }
      return (IBusinessObjectBoundControl[]) bindableControls.ToArray (); 
    }
  }
}

//public abstract class BusinessObjectDataSource: Component, IBusinessObjectDataSource
//{
//  [Browsable (false)]
//  public abstract IBusinessObject BusinessObject { get; set; }
//}

}