using System;
using System.ComponentModel;
using System.Drawing.Design;

using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
/// <summary>
/// A DataSource for <see cref="BindableSearchObject"/>s.
/// </summary>
public class SearchObjectDataSource : BusinessObjectDataSource
{
  private string _typeName;
  private IBusinessObject _object;

  /// <summary>
  /// Gets or sets the name of the type of the data source.
  /// </summary>
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

      return System.Type.GetType (_typeName, true); 
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
  /// Gets an instance of <see cref="SearchObjectClass"/> representing the type specified by <see cref="TypeName"/>.
  /// </summary>
  public override IBusinessObjectClass BusinessObjectClass
  {
    get 
    { 
      //TODO: eliminate this line ?
      Type type = Type;
      //TODO: Change behaviour if type is null (analog to DomainObjectDataSource) ?
      return new SearchObjectClass (type); 
    }
  }
}
}
