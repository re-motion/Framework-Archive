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

		public PropertyValueCollection PropertyValues
		{
			get { return _domainObject.GetDataContainer ().PropertyValues; }
		}

		public ClassDefinition ClassDefinition
		{
			get { return _domainObject.GetDataContainer ().ClassDefinition; }
		}

		public ClientTransaction ClientTransaction
		{
      get { return _domainObject.GetDataContainer ().ClientTransaction; }
		}

		public DomainObject DomainObject
		{
			get { return _domainObject; }
		}

		public object Timestamp
		{
			get { return _domainObject.GetDataContainer().Timestamp; }
		}

		public object GetValue (string propertyIdentifier)
		{
			return _domainObject.GetDataContainer ().GetValue (propertyIdentifier);
		}

		public void SetValue (string propertyIdentifier, object value)
		{
			_domainObject.GetDataContainer ().SetValue (propertyIdentifier, value);
		}

		public object this[string propertyIdentifier]
		{
			get { return _domainObject.GetDataContainer()[propertyIdentifier]; }
			set { _domainObject.GetDataContainer ()[propertyIdentifier] = value; }
		}
	}
}
