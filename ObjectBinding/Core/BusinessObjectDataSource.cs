using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.NullableValueTypes;
using Rubicon.Collections;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding
{

public enum DataSourceMode
{
  Read,
  Edit,
  Search
}

/// <summary>
///   This interface provides funtionality for binding an <see cref="IBusinessObject"/> to an 
///   <see cref="IBusinessObjectBoundControl"/>
/// </summary>
public interface IBusinessObjectDataSource
{
  DataSourceMode Mode { get; set; }

  /// <summary>
  ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls populated by this
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <param name="control"> The <see cref="IBusinessObjectBoundControl"/> to be added to the list. </param>
  void Register (IBusinessObjectBoundControl control);

  /// <summary>
  ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls populated by this
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <param name="control"> The <see cref="IBusinessObjectBoundControl"/> to be removed to the list. </param>
  void Unregister (IBusinessObjectBoundControl control);

  /// <summary>
  ///   Loads the values of the <see cref="BusinessObject"/> into all registered controls.
  /// </summary>
  /// <remarks>
  ///   On initial loads, all values must be loaded. On interim loads, each control decides whether it keeps its own 
  ///   value (e.g., using view state) or whether it reloads the value (useful for complex structures that need no 
  ///   validation).
  /// </remarks>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  void LoadValues (bool interim);

  /// <summary>
  ///   Saves the values of the <see cref="BusinessObject"/> from all registered controls.
  /// </summary>
  /// <remarks>
  ///   On final saves, all values must be saved. (It is assumed that invalid values were already identified using 
  ///   validators.) On interim saves, each control decides whether it saves its values into the business object or
  ///   using an alternate mechanism (e.g. view state).
  /// </remarks>
  /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
  void SaveValues (bool interim);

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of the bound <see cref="IBusinessObject"/>.
  /// </summary>
  IBusinessObjectClass BusinessObjectClass { get; }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectProvider"/> used for accessing supplementary information on the bound
  ///   <see cref="IBusinessObject"/>.
  /// </summary>
  IBusinessObjectProvider BusinessObjectProvider { get; }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> who's properties will be loaded into the registered controls.
  /// </summary>
  IBusinessObject BusinessObject { get; set; }

  /// <summary>
  ///   Gets an array of <see cref="IBusinessObjectBoundControl"/> objects registered with this 
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  IBusinessObjectBoundControl[] BoundControls { get; }
}

/// <summary>
///   The abstract default implementation of the <see cref="IBusinessObjectDataSource"/>.
/// </summary>
/// <remarks>
///   <para>
///     Override this class when creating a new business object model.
///   </para><para>
///     The data source usually provides a way of specifying a type identifier string. This identifier is then used to
///     get or instantiate the matching <see cref="IBusinessObjectClass"/> from the object model.
///     <note>
///       It is important to use a string or simpilar value type as the identifier. Otherwise it would not be possible
///       to specify the <see cref="IBusinessObjectClass"/> in the Visual Studio .net Designer, preventing any
///       further design time features from working.
///     </note>
///   </para>
/// </remarks>
public abstract class BusinessObjectDataSource: Component, IBusinessObjectDataSource
{
  private TypedArrayList _boundControls = new TypedArrayList (typeof (IBusinessObjectBoundControl));
  private DataSourceMode _mode = DataSourceMode.Edit;

  [Category ("Data")]
  public DataSourceMode Mode
  {
    get { return _mode; }
    set { _mode = value; }
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
      return (IBusinessObjectBoundControl[]) bindableControls.ToArray (typeof(IBusinessObjectBoundControl)); 
    }
  }
}

//public abstract class BusinessObjectDataSource: Component, IBusinessObjectDataSource
//{
//  [Browsable (false)]
//  public abstract IBusinessObject BusinessObject { get; set; }
//}

}