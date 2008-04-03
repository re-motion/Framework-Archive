using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Remotion.Data.DomainObjects.ObjectBinding.Design;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// A DataSource for <see cref="BindableDomainObject"/>s.
/// </summary>
public class DomainObjectDataSource : BusinessObjectDataSource
{
  private string _typeName;
  private IBusinessObject _object;

  /// <summary>
  /// Gets or sets the name of the type of the data source.
  /// </summary>
  [Editor (typeof (ClassPickerEditor), typeof (UITypeEditor))]
  public string TypeName
  {
    get { return _typeName; }
    set { _typeName = value; }
  }

  /// <summary>
  /// Gets the type specified by <see cref="TypeName"/>.
  /// </summary>
  public Type Type
  {
    get 
    {
      if (_typeName == null || _typeName.Length == 0)
        return null;

      if (Site != null && Site.DesignMode)
      {
        IDesignerHost designerHost = (IDesignerHost) Site.GetService (typeof (IDesignerHost));
        Assertion.IsNotNull (designerHost, "No IDesignerHost found.");
        return designerHost.GetType (_typeName);
      }

      return System.Type.GetType (_typeName); 
    }
  }

  /// <summary>
  /// Gets or sets the business object of the data source.
  /// </summary>
  public override IBusinessObject BusinessObject
  {
    get { return _object; }
    set { _object = value; }
  }

  /// <summary>
  /// Gets an instance of <see cref="DomainObjectClass"/> representing the type specified by <see cref="TypeName"/> or <see langword="null"/> if none is specified.
  /// </summary>
  public override IBusinessObjectClass BusinessObjectClass
  {
    get 
    { 
      Type type = Type;
      if (type == null)
        return null;

      if (Site != null && Site.DesignMode)
        return DomainObjectClass.CreateForDesignMode (type);

      return new DomainObjectClass (type); 
    }
  }

}
}
