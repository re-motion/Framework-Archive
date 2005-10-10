using System;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
// TODO Doc: 
public class SearchObjectClass : IBusinessObjectClass
{
  private BusinessObjectClassReflector _classReflector;

	public SearchObjectClass (Type type)
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
    get { return SearchObjectProvider.Instance; }
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
