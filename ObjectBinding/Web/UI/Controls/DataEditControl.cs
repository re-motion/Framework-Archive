using System;
using System.Web.UI;
using Rubicon.ObjectBinding;
using Rubicon.Web.UI;

namespace Rubicon.ObjectBinding.Web.Controls
{

public interface IDataEditControl: IControl
{
  /// <summary>
  ///   Gets or sets the BusinessObject used by this control. 
  /// </summary>
  /// <remarks>
  ///   If the control uses multiple business objects, only one can be exposed using this property.
  /// </remarks>
  IBusinessObject BusinessObject { get; set; }

  /// <summary>
  ///   Load the values into bound controls. 
  /// </summary>
  /// <remarks>
  ///   If the control uses multiple data sources, all data sources will be loaded using this method.
  /// </remarks>
  /// <param name="interim"> Indicates whether loading is initial (all values must be loaded) or interim (preserving values between requests).  </param>
  void LoadValues (bool interim);

  /// <summary>
  ///   Saves the values from bound controls. 
  /// </summary>
  /// <remarks>
  ///   If the control uses multiple data sources, all data sources will be saved using this method.
  /// </remarks>
  /// <param name="interim"> 
  ///   Indicates whether saving is interim (preserving values between requests) or final (all values must be saved). Before final saving,
  ///   <see cref="Validate"/> must be called and succeeded. 
  /// </param>
  void SaveValues (bool interim);

  /// <summary>
  ///   Notifies the control that editing is cancelled. 
  /// </summary>
  /// <remarks>
  ///   This method can be used to release locks. If the control is not in <see cref="EditMode"/>, this will usually be ignored.
  ///   If the control uses multiple data sources, all data sources will be affected by this method.
  /// </remarks>
  void CancelEdit ();

  /// <summary>
  ///   Gets or sets a value indicating whether this control is in edit mode or read-only mode.
  /// </summary>
  /// <remarks>
  ///   If the control uses multiple data sources, all data sources will be modified by this property.
  /// </remarks>
  bool EditMode { get; set; }

  /// <summary>
  ///   Validates all bound controls and displays error hints if the validation failed.
  /// </summary>
  /// <returns> True if validation succeeded, false if validation failed. </returns>
  bool Validate ();

  /// <summary>
  /// Provides access to the data source object. For common operations, use the methods of <see cref="IDataEditControl"/> instead.
  /// </summary>
  IBusinessObjectDataSource DataSource { get; }
}

public class DataEditUserControl: UserControl, IDataEditControl
{
  public IBusinessObject BusinessObject
  {
    get { return DataSource.BusinessObject; }
    set { DataSource.BusinessObject = value; }
  }

  public virtual void LoadValues (bool interim)
  {
    DataSource.LoadValues (interim);
  }

  public virtual void SaveValues (bool interim)
  {
    DataSource.SaveValues (interim);
  }

  public virtual void CancelEdit()
  {
  }

  public virtual bool EditMode
  {
    get { return DataSource.EditMode; } 
    set { DataSource.EditMode = value; }
  }

  public virtual bool Validate()
  {
    return true;
  }

  /// <summary>
  ///   Gets the control's data source. This method must be overridden in derived classes.
  /// </summary>
  /// <remarks>
  ///   This method should be abstract, but abstract base classes are not supported by VS.NET designer.
  /// </remarks>
  public virtual IBusinessObjectDataSource DataSource
  {
    get { throw new NotImplementedException ("Property DataSource must be overridden by derived classes to return a non-null value."); }
  }
}

}
