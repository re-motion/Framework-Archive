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
/// Summary description for BocPropertyPathWrapper.
/// </summary>
public class BocPropertyPathWrapper
{
  private bool _isPopertyPathEvaluated;
  private IBusinessObjectDataSource _dataSource;
  private BusinessObjectPropertyPath _propertyPath;
  private string _propertyPathIdentifier;

  public BocPropertyPathWrapper (BusinessObjectPropertyPath propertyPath)
  {
    ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
    _propertyPath = propertyPath;
  }

  public BocPropertyPathWrapper (string propertyPathIdentifier)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
    _propertyPathIdentifier = propertyPathIdentifier;
  }

  public BocPropertyPathWrapper()
  {}

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
      _propertyPath = value; 
      _propertyPathIdentifier = (value == null) ? string.Empty : value.ToString();
    }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string PropertyPathIdentifier
  {
    get 
    {
      return _propertyPathIdentifier; 
    }
    set 
    { 
      _propertyPathIdentifier = value;
      _propertyPath = null;
      _isPopertyPathEvaluated = false;
    }
  }
}
}
