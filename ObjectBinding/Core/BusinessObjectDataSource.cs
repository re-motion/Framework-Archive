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
///   <see cref="IBusinessObjectBoundControl"/>. Each business object model requires a specialized 
///   implementation of this interface.
/// </summary>
/// <remarks>
///   <para>
///     For most situations, the default implementation provided by <see cref="BusinessObjectDataSource"/> can be
///     used as a base for the implementation.
///   </para><para>
///     The data source usually provides a way of specifying a type identifier. This identifier is used to
///     get or instantiate the matching <see cref="IBusinessObjectClass"/> from the object model.
///     <note type="inheritinfo">
///       It is important to use an identifier that can be persisted as a string. Otherwise it would not be possible to 
///       specify and later persist the <see cref="IBusinessObjectClass"/> in the Visual Studio .NET Designer, 
///       preventing any further design time features from working.
///     </note>
///   </para>
///   <para>
///     See the <see href="Rubicon.ObjectBinding.html">Rubicon.ObjectBinding</see> namespace documentation for general 
///     information on the data binding process.
///   </para>
/// </remarks>
/// <seealso cref="IBusinessObjectBoundControl"/>
/// <seealso cref="IBusinessObjectBoundModifiableControl"/>
public interface IBusinessObjectDataSource
{
  /// <summary> Gets or sets the current <see cref="DataSourceMode"/>. </summary>
  /// <remarks> The behavior of the bound controls depends on the current <see cref="DataSourceMode"/>. </remarks>
  /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
  DataSourceMode Mode { get; set; }

  /// <summary>
  ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <remarks>
  ///   <b>Register</b> is usually called by the <see cref="IBusinessObjectBoundControl"/> when it's 
  ///   <see cref="IBusinessObjectBoundControl.DataSource"/> is set.
  /// </remarks>
  /// <param name="control"> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to be added to <see cref="BoundControls"/>. 
  /// </param>
  void Register (IBusinessObjectBoundControl control);

  /// <summary>
  ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <remarks>
  ///   <b>Unregister</b> is usually called by the <see cref="IBusinessObjectBoundControl"/> when it's 
  ///   <see cref="IBusinessObjectBoundControl.DataSource"/> is set to a new <see cref="IBusinessObjectDataSource"/>
  ///   or <see langword="null"/>.
  /// </remarks>
  /// <param name="control">
  ///   The <see cref="IBusinessObjectBoundControl"/> to be removed from <see cref="BoundControls"/>. 
  /// </param>
  void Unregister (IBusinessObjectBoundControl control);

  /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  /// <remarks>
  ///   On initial loads, all values must be loaded. On interim loads, each <see cref="IBusinessObjectBoundControl"/>
  ///   decides whether it keeps its own value (e.g., using view state) or reloads the value (useful for complex 
  ///   structures that need no validation).
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundControl.LoadValue">IBusinessObjectBoundControl.LoadValue</seealso>
  void LoadValues (bool interim);

  /// <summary> 
  ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing
  ///   <see cref="IBusinessObjectBoundModifiableControl"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  /// <remarks>
  ///   On final saves, all values must be saved. (It is assumed that invalid values were already identified using 
  ///   validators.) On interim saves, each <see cref="IBusinessObjectBoundModifiableControl"/> decides whether it 
  ///   saves its values into the <see cref="BusinessObject"/> or uses an alternate mechanism (e.g. view state).
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundModifiableControl.SaveValue">IBusinessObjectBoundModifiableControl.SaveValue</seealso>
  void SaveValues (bool interim);

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> connected to this 
  ///   <see cref="IBusinessObjectDataSource"/>. 
  /// </summary>
  /// <value> The <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>. </value>
  /// <remarks>
  ///   Usually set before the an <see cref="IBusinessObject"/> is connected to the 
  ///   <see cref="IBusinessObjectDataSource"/>. 
  /// </remarks>
  IBusinessObjectClass BusinessObjectClass { get; }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectProvider"/> used for accessing supplementary information on the connected
  ///   <see cref="IBusinessObject"/> and assigned <see cref="IBusinessObjectClass"/>.
  /// </summary>
  /// <value>
  ///   The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>.
  ///   Must not return <see langword="null"/>.
  /// </value>
  IBusinessObjectProvider BusinessObjectProvider { get; }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> connected to this <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <value>
  ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with
  ///   the <see cref="BusinessObjectClass"/> assigned to this <see cref="IBusinessObjectDataSource"/>.
  /// </value>
  IBusinessObject BusinessObject { get; set; }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this 
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <value> An array of <see cref="IBusinessObjectBoundControl"/> objects. </value>
  IBusinessObjectBoundControl[] BoundControls { get; }
}

/// <summary> The abstract default implementation of the <see cref="IBusinessObjectDataSource"/>. </summary>
/// <remarks>
///   Any specialized version of the <b>BusinessObjectDataSource</b> requires an override for the
///   <see cref="BusinessObjectClass"/> property. It is also necessary to provide a way for specifying which 
///   <see cref="IBusinessObjectClass"/> will be returned by this property. See the remarks section of the
///   <see cref="IBusinessObjectDataSource"/> interface documentation for details on how to implement this feature.
/// </remarks>
public abstract class BusinessObjectDataSource: Component, IBusinessObjectDataSource
{
  private TypedArrayList _boundControls = new TypedArrayList (typeof (IBusinessObjectBoundControl));
  private DataSourceMode _mode = DataSourceMode.Edit;

  /// <summary> Gets or sets the current <see cref="DataSourceMode"/>. </summary>
  /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
  [Category ("Data")]
  [DefaultValue (DataSourceMode.Edit)]
  public DataSourceMode Mode
  {
    get { return _mode; }
    set { _mode = value; }
  }

  /// <summary>
  ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this
  ///   <see cref="BusinessObjectDataSource"/>.
  /// </summary>
  /// <param name="control"> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to be added to <see cref="BoundControls"/>.
  /// </param>
  public void Register (IBusinessObjectBoundControl control)
  {
    if (! _boundControls.Contains (control))
      _boundControls.Add (control);
  }

  /// <summary>
  ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this
  ///   <see cref="BusinessObjectDataSource"/>.
  /// </summary>
  /// <param name="control"> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to be removed from <see cref="BoundControls"/>.
  /// </param>
  public void Unregister (IBusinessObjectBoundControl control)
  {
    if (_boundControls != null)
      _boundControls.Remove (control);
  }

  /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  public void LoadValues (bool interim)
  {
    if (_boundControls != null)
    {
      for (int i = 0; i < _boundControls.Count; i++)
      {
        IBusinessObjectBoundControl control = (IBusinessObjectBoundControl) _boundControls[i];
        if (control.IsValid)
          control.LoadValue (interim);
      }
    }
  }

  /// <summary> 
  ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing
  ///   <see cref="IBusinessObjectBoundModifiableControl"/>.
  /// </summary>
  /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
  public void SaveValues (bool interim)
  {
    if (_boundControls != null)
    {
      for (int i = 0; i < _boundControls.Count; i++)
      {
        IBusinessObjectBoundModifiableControl writeableControl = _boundControls[i] as IBusinessObjectBoundModifiableControl;
        if (writeableControl != null)
        {
          if (writeableControl.IsValid)
            writeableControl.SaveValue (interim);
        }
      }
    }
  }

  /// <summary> 
  ///   Gets the <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> connected to this 
  ///   <see cref="BusinessObjectDataSource"/>. 
  /// </summary>
  /// <value> The <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>. </value>
  /// <remarks>
  ///   Usually set before the <see cref="IBusinessObject"/> is connected to the 
  ///   <see cref="BusinessObjectDataSource"/>. 
  /// </remarks>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public abstract IBusinessObjectClass BusinessObjectClass { get; }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectProvider"/> used for accessing supplementary information on the connected
  ///   <see cref="IBusinessObject"/> and assigned <see cref="IBusinessObjectClass"/>.
  /// </summary>
  /// <value>
  ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with
  ///   the <see cref="BusinessObjectClass"/> assigned to this <see cref="BusinessObjectDataSource"/>.
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public virtual IBusinessObjectProvider BusinessObjectProvider 
  { 
    get { return (BusinessObjectClass == null) ? null : BusinessObjectClass.BusinessObjectProvider; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> connected to this <see cref="IBusinessObjectDataSource"/>
  /// </summary>
  /// <value>
  ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with
  ///   the <see cref="BusinessObjectClass"/> assigned to this <see cref="BusinessObjectDataSource"/>.
  /// </value>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public abstract IBusinessObject BusinessObject { get; set; }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this 
  ///   <see cref="BusinessObjectDataSource"/>.
  /// </summary>
  /// <value> An array of <see cref="IBusinessObjectBoundControl"/> objects. </value>
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