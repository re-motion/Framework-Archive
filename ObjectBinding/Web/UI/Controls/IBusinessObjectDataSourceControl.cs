using System;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///  The <see cref="IBusinessObjectDataSourceControl"/> interface defines the methods and 
///  properties required to implement a control that provides an object of type
///  <see cref="IBusinessObjectDataSource"/> to the other controls inside an <c>APSX WebForm</c>.
/// </summary>
public interface IBusinessObjectDataSourceControl: IBusinessObjectDataSource, IControl
{
}

/// <summary>
///   <see cref="BusinessObjectDataSourceControl"/> is the default implementation of
///   the interface <see cref="IBusinessObjectDataSourceControl"/>. Derive from this class
///   if you want to create an invisible control only providing an object of type
///   <see cref="IBusinessObjectDataSource"/>
/// </summary>
public abstract class BusinessObjectDataSourceControl: Control, IBusinessObjectDataSourceControl
{
  /// <summary>
  ///   Overrides the implementation of <see cref="Control.Render"/>. Does not render any output.
  /// </summary>
  /// <param name="writer">
  ///   The <see cref="HtmlTextWriter"/> object that receives the server control content. 
  /// </param>
  protected override void Render (HtmlTextWriter writer)
  {
    //  No output, control is invisible
  }

  /// <summary>
  ///   Load the values of the business object into all registered controls.
  /// </summary>
  /// <remarks> Implementation of <see cref="IBusinessObjectDataSource.LoadValues"/>. </remarks>
  public virtual void LoadValues(bool interim)
  {
    GetDataSource().LoadValues (interim);
  }

  /// <summary>
  ///   Save the values of the business object into all registered controls.
  /// </summary>
  /// <remarks> Implementation of <see cref="IBusinessObjectDataSource.SaveValues"/>. </remarks>
  public virtual void SaveValues (bool interim)
  {
    GetDataSource().SaveValues (interim);
  }

  /// <summary>
  ///   Registers a <see cref="IBusinessObjectBoundControl"/> with the 
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <remarks> Implementation of <see cref="IBusinessObjectDataSource.Register"/>. </remarks>
  public virtual void Register (IBusinessObjectBoundControl control)
  {
    GetDataSource().Register (control);
  }

  /// <summary>
  ///   Unregisters a <see cref="IBusinessObjectBoundControl"/> with the 
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <remarks> Implementation of <see cref="IBusinessObjectDataSource.Unregister"/>. </remarks>
  public virtual void Unregister (IBusinessObjectBoundControl control)
  {
    GetDataSource().Unregister (control);
  }

  /// <summary>
  ///   Gets or sets the whether the <see cref="IBusinessObjectDataSourceControl"/> is in edit mode.
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [DefaultValue(true)]
  public virtual bool EditMode
  {
    get { return GetDataSource().EditMode; }
    set { GetDataSource().EditMode = value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObject"/> to which the 
  ///   <see cref="IBusinessObjectDataSource"/> connects.
  /// </summary>
  /// <remarks> Implementation of <see cref="IBusinessObjectDataSource.BusinessObject"/>. </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObject BusinessObject
  {
    get { return GetDataSource().BusinessObject; }
    set { GetDataSource().BusinessObject = value; }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/>
  ///   connected to the <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <remarks> Implementation of <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>. </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectClass BusinessObjectClass
  {
    get { return GetDataSource().BusinessObjectClass; }
  }

  /// <summary>
  ///   Gets the <see cref="IBusinessObjectProvider"/> of <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <remarks> Implementation of <see cref="IBusinessObjectDataSource.BusinessObjectProvider"/>. </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual IBusinessObjectProvider BusinessObjectProvider
  {
    get { return GetDataSource().BusinessObjectProvider; }
  }

  /// <summary>
  ///   Returns the <see cref="IBusinessObjectDataSource"/> encapsulated in this 
  ///   <see cref="IBusinessObjectDataSourceControl"/>.
  /// </summary>
  /// <returns> 
  ///   The <see cref="IBusinessObjectDataSource"/> encapsulated in this 
  ///   <see cref="IBusinessObjectDataSourceControl"/>.
  /// </returns>
  protected abstract IBusinessObjectDataSource GetDataSource();

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public IBusinessObjectBoundControl[] BoundControls
  {
    get { return GetDataSource().BoundControls; }
  }
}

}
