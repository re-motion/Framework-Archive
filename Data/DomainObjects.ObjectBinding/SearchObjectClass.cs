using System;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class QueryClass : IBusinessObjectClass
{
  private BusinessObjectClassReflector _classReflector;

	public QueryClass (Type type)
	{
    ArgumentUtility.CheckNotNull ("type", type);

    ReflectionPropertyFactory propertyFactory = new ReflectionPropertyFactory ();
    _classReflector = new BusinessObjectClassReflector (type, propertyFactory);
  }

  #region IBusinessObjectClass Members

  public bool RequiresWriteBack
  {
    get { return false; }
  }

  public string Identifier
  {
    get { return _classReflector.Identifier; }
  }

  public IBusinessObjectProvider BusinessObjectProvider
  {
    get { return QueryProvider.Instance; }
  }

  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    return _classReflector.GetPropertyDefinition (propertyIdentifier);
  }

  public IBusinessObjectProperty[] GetPropertyDefinitions ()
  {
    return _classReflector.GetPropertyDefinitions ();
  }

  #endregion
}
}
