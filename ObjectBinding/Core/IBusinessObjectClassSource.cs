using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary>
/// Summary description for PropertyPathBinding.
/// </summary>
public class PropertyPathBinding
{
  /// <summary> <see langword="true"/> once the <see cref="PropertyPath"/> has been set. </summary>
  private bool _isPopertyPathEvaluated;
  /// <summary> 
  ///   The <see cref="IBusinessObjectDataSource"/> used to evaluate the 
  ///   <see cref="PropertyPathIdentifier"/>. 
  /// </summary>
  private IBusinessObjectDataSource _dataSource;
  /// <summary> 
  ///   The <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="PropertyPathBinding"/>.
  /// </summary>
  private BusinessObjectPropertyPath _propertyPath;
  /// <summary> 
  ///   The <see cref="string"/> representing the <see cref="BusinessObjectPropertyPath"/> mananged 
  ///   by this <see cref="PropertyPathBinding"/>.
  /// </summary>
  private string _propertyPathIdentifier;

  /// <summary> Simple Constructor. </summary>
  /// <param name="propertyPath">
  ///   The <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="PropertyPathBinding"/>.
  /// </param>
  public PropertyPathBinding (BusinessObjectPropertyPath propertyPath)
  {
    ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
    PropertyPath = propertyPath;
  }

  /// <summary> Simple Constructor. </summary>
  /// <param name="propertyPathIdentifier">
  ///   The <see cref="string"/> representing the <see cref="BusinessObjectPropertyPath"/> mananged 
  ///   by this <see cref="PropertyPathBinding"/>.
  /// </param>
  public PropertyPathBinding (string propertyPathIdentifier)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
    PropertyPathIdentifier = propertyPathIdentifier;
  }

  /// <summary> Simple Constructor. </summary>
  public PropertyPathBinding()
  {}

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <returns>
  ///   Returns the class name of the instance.
  /// </returns>
  public override string ToString()
  {
    return GetType().Name;
  }

  /// <summary> 
  ///   The <see cref="IBusinessObjectDataSource"/> used to evaluate the 
  ///   <see cref="PropertyPathIdentifier"/>. 
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectDataSource DataSource
  {
    get
    {
      return _dataSource; 
    }
    set 
    {
      _dataSource = value; 
    }
  }

  /// <summary> 
  ///   The <see cref="BusinessObjectPropertyPath"/> mananged by this 
  ///   <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <value>
  ///   A <see cref="BusinessObjectPropertyPath"/> or <see langword="null"/> if the 
  ///   <see cref="PropertyPathIdentifier"/> has not been evaluated.
  ///   Must not be assigned <see langword="null"/>.
  /// </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BusinessObjectPropertyPath PropertyPath
  {
    get 
    {
      if (! _isPopertyPathEvaluated)
      {
        if (_dataSource == null)
          throw new InvalidOperationException ("PropertyPath could not be resolved because the DataSource is not set.");

        _propertyPath = BusinessObjectPropertyPath.Parse (_dataSource, _propertyPathIdentifier);
        _isPopertyPathEvaluated = true;
      }

      return _propertyPath;
    }
    set 
    {
      ArgumentUtility.CheckNotNull ("PropertyPath", value);
      _propertyPath = value; 
      _propertyPathIdentifier = (value == null) ? string.Empty : value.ToString();
    }
  }

  /// <summary> 
  ///   The <see cref="string"/> representing the <see cref="BusinessObjectPropertyPath"/> mananged 
  ///   by this <see cref="PropertyPathBinding"/>.
  /// </summary>
  /// <value> 
  ///   A <see cref="string"/> formatted as a valid property path. 
  ///   Must not be assigned <see langword="null"/> or emtpy.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  //  No default value
  public string PropertyPathIdentifier
  {
    get 
    {
      return _propertyPathIdentifier; 
    }
    set 
    { 
      ArgumentUtility.CheckNotNullOrEmpty ("PropertyPathIdentifier", value);
      _propertyPathIdentifier = value;
      _propertyPath = null;
      _isPopertyPathEvaluated = false;
    }
  }
}
}
