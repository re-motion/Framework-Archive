using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
	/// <summary>
	/// Provides a DataContainer-like interface for a DomainObject without actually accessing the object's DataContainer.
	/// </summary>
	public class DataContainerIndirection
	{
		private readonly DomainObject _domainObject;

		public DataContainerIndirection (DomainObject domainObject)
		{
			ArgumentUtility.CheckNotNull ("domainObject", domainObject);
			_domainObject = domainObject;
		}

    private DataContainer GetDataContainer ()
    {
      _domainObject.CheckIfRightTransaction (ClientTransaction);
      return _domainObject.GetDataContainerForTransaction (ClientTransaction);
    }


		public PropertyValueCollection PropertyValues
		{
			get { return GetDataContainer ().PropertyValues; }
		}

	  public ClassDefinition ClassDefinition
		{
			get { return GetDataContainer ().ClassDefinition; }
		}

		public ClientTransaction ClientTransaction
		{
      get { return ClientTransaction.Current; }
		}

		public DomainObject DomainObject
		{
			get { return _domainObject; }
		}

		public object Timestamp
		{
			get { return GetDataContainer().Timestamp; }
		}

		public object GetValue (string propertyIdentifier)
		{
			return GetDataContainer ().GetValue (propertyIdentifier);
		}

		public void SetValue (string propertyIdentifier, object value)
		{
			GetDataContainer ().SetValue (propertyIdentifier, value);
		}

		public object this[string propertyIdentifier]
		{
			get { return GetDataContainer()[propertyIdentifier]; }
			set { GetDataContainer ()[propertyIdentifier] = value; }
		}
	}
}
