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
	  private readonly ClientTransaction _clientTransaction;

		public DataContainerIndirection (DomainObject domainObject, ClientTransaction clientTransaction)
		{
			ArgumentUtility.CheckNotNull ("domainObject", domainObject);
		  ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

			_domainObject = domainObject;
		  _clientTransaction = clientTransaction;
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
      get { return _clientTransaction; }
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
