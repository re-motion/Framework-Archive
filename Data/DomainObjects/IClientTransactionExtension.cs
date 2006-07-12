using System;
using Rubicon.Data.DomainObjects.Queries;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Interface for extending the <see cref="ClientTransaction"/> by observing events within the DomainObjects framework.
  /// </summary>
  public interface IClientTransactionExtension
  {
    /// <summary>
    /// This method is invoked, when a new <see cref="DomainObject"/> is created, but not registered yet. 
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="type">The <see cref="System.Type"/> of the new <see cref="DomainObject"/>.</param>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void NewObjectCreating (Type type);

    /// <summary>
    /// This method is invoked, when one or multiple <see cref="DomainObject"/>s were loaded. 
    /// </summary>
    /// <param name="loadedDomainObjects">A collection of all <see cref="DomainObject"/>s that were loaded.</param>
    /// <remarks>
    ///   <see cref="DomainObject.OnLoaded"/> is called before this method is invoked, whereas <see cref="ClientTransaction.Loaded"/> is fired after it.
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception.</note>
    void ObjectsLoaded (DomainObjectCollection loadedDomainObjects);

    /// <summary>
    /// This method is invoked, before a <see cref="DomainObject"/> is deleted. 
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to be deleted.</param>
    /// <remarks>
    ///   <para>
    ///     If the <see cref="DomainObject"/> has related <see cref="DomainObject"/>s then <see cref="RelationChanging"/> is invoked for 
    ///     every one of them right after this method.
    ///   </para>
    ///   <para>
    ///     If the opposite objects were not loaded yet, <see cref="ObjectsLoaded"/> is invoked before this method.
    ///   </para>
    ///   <para>
    ///     The events <see cref="DomainObject.Deleting"/> and <see cref="DomainObject.RelationChanging"/> are fired after this method was invoked.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void ObjectDeleting (DomainObject domainObject);

    /// <summary>
    /// This method is invoked, after a <see cref="DomainObject"/> was deleted. 
    /// It indicates the success of the operation. 
    /// </summary>
    /// <param name="domainObject">
    ///   The <see cref="DomainObject"/> that was deleted. This object might already be discarded.<br/>
    ///   For more information why and when an object is discarded see <see cref="Rubicon.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </param>
    /// <remarks>
    ///   <para>
    ///     If the <see cref="DomainObject"/> has related <see cref="DomainObject"/>s then <see cref="RelationChanging"/> is invoked for 
    ///     every one of them right before this method.
    ///   </para>
    ///   <para>
    ///     The events <see cref="DomainObject.RelationChanged"/> and <see cref="DomainObject.Deleted"/> are fired before this method is invoked.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="ObjectDeleting"/> instead.</note>
    void ObjectDeleted (DomainObject domainObject);

    /// <summary>
    /// This method is invoked, before a value of <paramref name="dataContainer"/> is read. 
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="dataContainer">
    ///   The <see cref="DataContainer"/> holding the value that is being read.
    ///   Use the <see cref="DataContainer.DomainObject"/> property to get the corresponding <see cref="DomainObject"/>.
    /// </param>
    /// <param name="propertyValue">The <see cref="PropertyValue"/> object holding the value that is being read.</param>
    /// <param name="valueAccess">A value indicating whether the current or the original value is being accessed.</param>
    /// <remarks>
    ///   Use this method to cancel the operation, whereas <see cref="PropertyValueRead"/> should be used to perform actions on its successful execution.
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void PropertyValueReading (DataContainer dataContainer, PropertyValue propertyValue, ValueAccess valueAccess);

    /// <summary>
    /// This method is invoked, when a value of <paramref name="dataContainer"/> was read. 
    /// </summary>
    /// <param name="dataContainer">
    ///   The <see cref="DataContainer"/> holding the value that was read.
    ///   Use the <see cref="DataContainer.DomainObject"/> property to get the corresponding <see cref="DomainObject"/>.
    /// </param>
    /// <param name="propertyValue">The <see cref="PropertyValue"/> object holding the value that was read.</param>
    /// <param name="value">The value that was read.</param>
    /// <param name="valueAccess">A value indicating whether the current or the original value was accessed.</param>
    /// <remarks>
    ///   Use this method to perform actions on a successful execution, whereas <see cref="PropertyValueReading"/> should be used to cancel the operation.
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="PropertyValueReading"/> instead.</note>
    void PropertyValueRead (DataContainer dataContainer, PropertyValue propertyValue, object value, ValueAccess valueAccess);

    /// <summary>
    /// This method is invoked before a value of <paramref name="dataContainer"/> is changed.
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="dataContainer">
    ///   The <see cref="DataContainer"/> holding the <paramref name="propertyValue"/> that is being changed.
    ///   Use the <see cref="DataContainer.DomainObject"/> property to get the corresponding <see cref="DomainObject"/>.
    /// </param>
    /// <param name="propertyValue">The <see cref="PropertyValue"/> object holding the value that is being changed.</param>
    /// <param name="oldValue">The value of the property it currently has.</param>
    /// <param name="newValue">The new value to be assigned to the property.</param>
    /// <remarks>
    ///   <para>
    ///     Use this method to cancel the operation, whereas <see cref="PropertyValueChanged"/> should be used to perform actions on its successful execution.
    ///   </para>
    ///   <para>
    ///     The events <see cref="DataContainer.PropertyChanging"/> and <see cref="DomainObject.PropertyChanging"/> are fired after this method was invoked.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void PropertyValueChanging (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);

    /// <summary>
    /// This method is invoked after a value of <paramref name="dataContainer"/> was changed.
    /// </summary>
    /// <param name="dataContainer">
    ///   The <see cref="DataContainer"/> holding the <paramref name="propertyValue"/> that was changed.
    ///   Use the <see cref="DataContainer.DomainObject"/> property to get the corresponding <see cref="DomainObject"/>.
    /// </param>
    /// <param name="propertyValue">The <see cref="PropertyValue"/> object holding the value that was changed.</param>
    /// <param name="oldValue">The old value of the property it had before.</param>
    /// <param name="newValue">The value that was assigned to the property.</param>
    /// <remarks>
    ///   <para>
    ///     Use this method to perform actions on a successful execution, whereas <see cref="PropertyValueReading"/> should be used to cancel the operation.
    ///   </para>
    ///   <para>
    ///     The events <see cref="DataContainer.PropertyChanged"/> and <see cref="DomainObject.PropertyChanged"/> are fired before this method is invoked.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="PropertyValueChanging"/> instead.</note>
    void PropertyValueChanged (DataContainer dataContainer, PropertyValue propertyValue, object oldValue, object newValue);

    /// <summary>
    /// This method is invoked, before a relation property is being read. 
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> whose relation property is being read.</param>
    /// <param name="propertyName">The name of the relation property being read.</param>
    /// <param name="valueAccess">A value indicating whether the current or the original value is being accessed.</param>
    /// <remarks>
    ///   Use this method to cancel the operation, whereas <see cref="RelationRead"/> should be used to perform actions on its successful execution.
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void RelationReading (DomainObject domainObject, string propertyName, ValueAccess valueAccess);

    /// <summary>
    /// This method is invoked, when a relation property with cardinality <see cref="Mapping.CardinalityType.One"/> was read. 
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> whose relation property was read.</param>
    /// <param name="propertyName">The name of the relation property that was read.</param>
    /// <param name="relatedObject">The related <see cref="DomainObject"/> of the relation property.</param>
    /// <param name="valueAccess">A value indicating whether the current or the original value was accessed.</param>
    /// <remarks>
    ///   <para>
    ///     Use this method to perform actions on a successful execution, whereas <see cref="RelationReading"/> should be used to cancel the operation.
    ///   </para>
    ///   <para>
    ///     If the opposite object was not loaded yet, <see cref="ObjectsLoaded"/> is invoked before this method.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="RelationReading"/> instead.</note>
    void RelationRead (DomainObject domainObject, string propertyName, DomainObject relatedObject, ValueAccess valueAccess);

    /// <summary>
    /// This method is invoked, when a relation property with cardinality <see cref="Mapping.CardinalityType.Many"/> was read. 
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> whose relation property was read.</param>
    /// <param name="propertyName">The name of the relation property that was read.</param>
    /// <param name="relatedObjects">
    ///   A read-only <see cref="DomainObjectCollection"/> containing the related <see cref="DomainObject"/>s of the relation property.
    /// </param>
    /// <param name="valueAccess">A value indicating whether the current or the original value was accessed.</param>
    /// <remarks>
    ///   <para>
    ///     Use this method to perform actions on a successful execution, whereas <see cref="RelationReading"/> should be used to cancel the operation.
    ///   </para>
    ///   <para>
    ///     If the opposite objects were not loaded yet, <see cref="ObjectsLoaded"/> is invoked before this method.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="RelationReading"/> instead.</note>
    void RelationRead (DomainObject domainObject, string propertyName, DomainObjectCollection relatedObjects, ValueAccess valueAccess);

    /// <summary>
    /// This method is invoked before a relation is changed.
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> whose relation property is being changed.</param>
    /// <param name="propertyName">The name of the relation property.</param>
    /// <param name="oldRelatedObject">The current related object.</param>
    /// <param name="newRelatedObject">The new related object to be assigned.</param>
    /// <remarks>
    ///   <para>Use this method to cancel the operation, whereas <see cref="RelationChanged"/> should be used to perform actions on its successful execution.</para>
    ///   <para>The following table lists the values of <paramref name="oldRelatedObject"/> and <paramref name="newRelatedObject"/> for operations on 1:n relations:
    ///     <list type="table">
    ///       <listheader>
    ///         <term>Operation</term>
    ///         <description>Values</description>
    ///       </listheader>
    ///       <item>
    ///         <term>Add, Insert</term>
    ///         <description><paramref name="oldRelatedObject"/> is <see langword="null"/>, <paramref name="newRelatedObject"/> is not <see langword="null"/>.</description>
    ///       </item>
    ///       <item>
    ///         <term>Replace</term>
    ///         <description>Neither <paramref name="oldRelatedObject"/> nor <paramref name="newRelatedObject"/> are <see langword="null"/>.</description>
    ///       </item>
    ///       <item>
    ///         <term>Remove</term>
    ///         <description><paramref name="oldRelatedObject"/> is not <see langword="null"/>, <paramref name="newRelatedObject"/> is <see langword="null"/>.</description>
    ///       </item>
    ///     </list>
    ///   </para>
    ///   <para>
    ///     The <see cref="DomainObject.RelationChanging"/> events are fired after this method was invoked.
    ///   </para>
    ///   <para>
    ///     If the opposite object(s) was/were not loaded yet, <see cref="ObjectsLoaded"/> is invoked before this method.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void RelationChanging (DomainObject domainObject, string propertyName, DomainObject oldRelatedObject, DomainObject newRelatedObject);

    /// <summary>
    /// This method is invoked after a relation was changed.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> whose relation property was changed.</param>
    /// <param name="propertyName">The name of the relation property.</param>
    /// <remarks>
    ///   <para>
    ///     Use this method to perform actions on a successful execution, whereas <see cref="RelationChanging"/> should be used to cancel the operation.
    ///   </para>
    ///   <para>
    ///     The <see cref="DomainObject.RelationChanged"/> events are fired before this method is invoked.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="RelationChanging"/> instead.</note>
    void RelationChanged (DomainObject domainObject, string propertyName);

    /// <summary>
    /// This method is invoked after a collection query was executed by <see cref="Queries.QueryManager.GetCollection"/>.
    /// The <see cref="IClientTransactionExtension"/> may change the result at this point.
    /// </summary>
    /// <param name="queryResult">A writable <see cref="DomainObjectCollection"/> holding the result of the query. The collection may be modified.</param>
    /// <param name="query">The query that was executed.</param>
    /// <remarks>
    ///   <para>
    ///     If some objects that were returned by the query were not loaded yet, <see cref="ObjectsLoaded"/> is invoked before this method.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception.</note>
    void FilterQueryResult (DomainObjectCollection queryResult, IQuery query);

    /// <summary>
    /// This method is invoked before a <see cref="ClientTransaction"/> is committed.
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="changedDomainObjects">A read-only <see cref="DomainObjectCollection"/> holding all changed <see cref="DomainObject"/>s that are being committed.</param>
    /// <remarks>
    ///   <para>Use this method to cancel the operation, whereas <see cref="Committed"/> should be used to perform actions on its successful execution.</para>
    ///   <para>
    ///     The <see cref="DomainObject.Committing"/> events are fired before this method is invoked, 
    ///     whereas <see cref="ClientTransaction.Committing"/> is fired after it.
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void Committing (DomainObjectCollection changedDomainObjects);

    /// <summary>
    /// This method is invoked after a <see cref="ClientTransaction"/> was executed.
    /// </summary>
    /// <param name="changedDomainObjects">A read-only <see cref="DomainObjectCollection"/> holding all changed <see cref="DomainObject"/>s that are being committed.</param>
    /// <remarks>
    ///   <para>
    ///     Use this method to perform actions on a successful execution, whereas <see cref="Committing"/> should be used to cancel the operation.
    ///   </para>
    ///   <para>
    ///     The events <see cref="DomainObject.Committed"/> and <see cref="ClientTransaction.Committed"/> are fired before this method is invoked. 
    ///   </para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="Committing"/> instead.</note>
    void Committed (DomainObjectCollection changedDomainObjects);

    /// <summary>
    /// This method is invoked before a <see cref="ClientTransaction"/> is rolled back.
    /// The operation may be cancelled at this point.
    /// </summary>
    /// <param name="changedDomainObjects">A read-only <see cref="DomainObjectCollection"/> holding all changed <see cref="DomainObject"/>s that are being rolled back.</param>
    /// <remarks>
    ///   <para>Use this method to cancel the operation, whereas <see cref="RolledBack"/> should be used to perform actions on its successful execution.</para>
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method should throw an exception if the operation must be cancelled.</note>
    void RollingBack (DomainObjectCollection changedDomainObjects);

    /// <summary>
    /// This method is invoked after a <see cref="ClientTransaction"/> was rolled back.
    /// </summary>
    /// <param name="changedDomainObjects">A read-only <see cref="DomainObjectCollection"/> holding all changed <see cref="DomainObject"/>s that are being rolled back.</param>
    /// <remarks>
    ///   Use this method to perform actions on a successful execution, whereas <see cref="RollingBack"/> should be used to cancel the operation.
    /// </remarks>
    /// <note type="implementnotes">The implementation of this method must not throw an exception. To cancel the operation use <see cref="RollingBack"/> instead.</note>
    void RolledBack (DomainObjectCollection changedDomainObjects);
  }
}
