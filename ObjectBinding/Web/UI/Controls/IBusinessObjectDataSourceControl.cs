using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///  The <see cref="IBusinessObjectDataSourceControl"/> interface defines the methods and 
///  properties required to implement a control that provides an <see cref="IBusinessObjectDataSource"/>
///  to controls of type <see cref="IBusinessObjectBoundWebControl"/> inside an <b>ASPX Web Form</b> 
///  or <b>ASCX User Control</b>.
/// </summary>
/// <remarks>
///   It is usually sufficient to use the abstract default implementation 
///   (<see cref="BusinessObjectDataSourceControl"/>) as base for creating a new 
///   <b>IBusinessObjectDataSourceControl</b>.
/// </remarks>
public interface IBusinessObjectDataSourceControl: IBusinessObjectDataSource, IControl
{
  /// <summary> Validates all bound controls implementing <see cref="IValidatableControl"/>. </summary>
  /// <returns> <see langword="true"/> if no validation errors where found. </returns>
  bool Validate ();
}

/// <summary>
///   <see cref="BusinessObjectDataSourceControl"/> is the default implementation of
///   the <see cref="IBusinessObjectDataSourceControl"/> interface. Derive from this class
///   if you want to create an invisible control only providing an object of type
///   <see cref="IBusinessObjectDataSource"/>.
/// </summary>
/// <remarks>
///   <para>
///     When creating a specialized implementation of this class, override the abstract <see cref="GetDataSource"/> 
///     method. It is recommended to create the instance to be returned by <see cref="GetDataSource"/> during 
///     the construction phase of the <b>BusinessObjectDataSourceControl</b>.
///   </para><para>
///     In addition, an identifier for the <see cref="BusinessObjectClass"/> must be provided in form of a 
///     property. See the remarks section of the <see cref="IBusinessObjectDataSource"/> for details on implementing 
///     this property.
///   </para>
/// </remarks>
#if net20
[NonVisualControl]
#endif
public abstract class BusinessObjectDataSourceControl: Control, IBusinessObjectDataSourceControl
{
  /// <summary>
  ///   Returns the <see cref="IBusinessObjectDataSource"/> encapsulated in this 
  ///   <see cref="BusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <returns> An <see cref="IBusinessObjectDataSource"/>. </returns>
  /// <remarks>
  ///   For details on overriding this method, see <see cref="BusinessObjectDataSourceControl"/>'s remarks section.
  /// </remarks>
  protected abstract IBusinessObjectDataSource GetDataSource();

  /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
  /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
  /// <remarks> 
  ///   Executes the <see cref="IBusinessObjectDataSource.LoadValues"/> method of the encapsulated data source.
  /// </remarks>
  public virtual void LoadValues(bool interim)
  {
    GetDataSource().LoadValues (interim);
  }

  /// <summary> 
  ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing
  ///   <see cref="IBusinessObjectBoundModifiableControl"/>.
  /// </summary>
  /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
  /// <remarks> 
  ///   Executes the <see cref="IBusinessObjectDataSource.SaveValues"/> method of the encapsulated data source.
  /// </remarks>
  public virtual void SaveValues (bool interim)
  {
    GetDataSource().SaveValues (interim);
  }

  /// <summary>
  ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this
  ///   <see cref="BusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <param name="control"> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to be registered with this
  ///   <see cref="BusinessObjectDataSourceControl"/>.
  /// </param>
  /// <remarks> 
  ///   Executes the <see cref="IBusinessObjectDataSource.Register"/> method of the encapsulated data source.
  /// </remarks>
  public virtual void Register (IBusinessObjectBoundControl control)
  {
    GetDataSource().Register (control);
  }

  /// <summary>
  ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this
  ///   <see cref="BusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <param name="control"> 
  ///   The <see cref="IBusinessObjectBoundControl"/> to be unregistered from this 
  ///   <see cref="BusinessObjectDataSourceControl"/>.
  /// </param>
  /// <remarks> 
  ///   Executes the <see cref="IBusinessObjectDataSource.Unregister"/> method of the encapsulated data source.
  /// </remarks>
  public virtual void Unregister (IBusinessObjectBoundControl control)
  {
    GetDataSource().Unregister (control);
  }

  /// <summary>
  ///   Gets or sets the current <see cref="DataSourceMode"/> of this <see cref="BusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
  /// <remarks> 
  ///   Gets or sets the <see cref="IBusinessObjectDataSource.Mode"/> property of the encapsulated data source.
  /// </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue(DataSourceMode.Edit)]
  public virtual DataSourceMode Mode
  {
    get { return GetDataSource().Mode; }
    set { GetDataSource().Mode = value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> connected to this <see cref="BusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <value>
  ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with
  ///   <see cref="BusinessObjectClass"/>.
  /// </value>
  /// <remarks> 
  ///   Gets or sets the <see cref="IBusinessObjectDataSource.BusinessObject"/> property of the encapsulated data 
  ///   source.
  /// </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObject BusinessObject
  {
    get { return GetDataSource().BusinessObject; }
    set { GetDataSource().BusinessObject = value; }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/>
  ///   connected to this <see cref="BusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <value> The <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>. </value>
  /// <remarks>
  ///   <para>
  ///     Usually set before the <see cref="IBusinessObject"/> is connected to the 
  ///     <see cref="IBusinessObjectDataSource"/> by utilizing Visual Studio .NET Designer. 
  ///   </para><para>
  ///     Gets the <see cref="IBusinessObjectDataSource.BusinessObjectClass"/> property of the encapsulated 
  ///     data source.
  ///   </para>
  /// </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectClass BusinessObjectClass
  {
    get { return GetDataSource().BusinessObjectClass; }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectProvider"/> of this <see cref="BusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <value>
  ///   The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>.
  ///   Must not return <see langword="null"/>.
  /// </value>
  /// <remarks> 
  ///   Gets the <see cref="IBusinessObjectDataSource.BusinessObjectProvider"/> property of the encapsulated
  ///   data source.
  /// </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectProvider BusinessObjectProvider
  {
    get { return GetDataSource().BusinessObjectProvider; }
  }

  /// <summary>
  ///   Gets an array of <see cref="IBusinessObjectBoundControl"/> objects bound to this 
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <value> An array of <see cref="IBusinessObjectBoundControl"/> objects. </value>
  /// <remarks> 
  ///   Gets the <see cref="IBusinessObjectDataSource.BoundControls"/> property of the encapsulated data source.
  /// </remarks>
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectBoundControl[] BoundControls
  {
    get { return GetDataSource().BoundControls; }
  }

  /// <summary> Validates all bound controls implementing <see cref="IValidatableControl"/>. </summary>
  /// <returns> <see langword="true"/> if no validation errors where found. </returns>
  public bool Validate()
  {
    bool isValid = true;
    foreach (IBusinessObjectBoundControl control in BoundControls)
    {
      IValidatableControl validateableControl = control as IValidatableControl;
      if (validateableControl != null)
        isValid &= validateableControl.Validate();
    }
    return isValid;
  }

  /// <summary>
  ///   Overrides the implementation of <see cref="Control.Render">Control.Render</see>. Does not render any output.
  /// </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
  protected override void Render (HtmlTextWriter writer)
  {
    //  No output, control is invisible
  }
}

}
