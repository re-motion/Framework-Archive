namespace Rubicon.ObjectBinding
{
  /// <summary>
  ///   The <b>IBusinessObjectWithIdentity</b> interface provides functionality to uniquely identify a business object 
  ///   within its business object domain.
  /// </summary>
  /// <remarks>
  ///   With the help of the <b>IBusinessObjectWithIdentity</b> interface it is possible to persist and later restore 
  ///   a reference to the business object. 
  /// </remarks>
  public interface IBusinessObjectWithIdentity : IBusinessObject
  {
    /// <summary> Gets the human readable <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/>. </summary>
    /// <value> A <see cref="string"/> identifying this object to the user. </value>
    /// <remarks> This value does not have to be unqiue within its business object domain. </remarks>
    string DisplayName { get; }

    /// <summary> Gets the human readable <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/> in an exception-safe manner. </summary>
    /// <remarks> Accessing this property must not fail during normal operations. </remarks>
    string DisplayNameSafe { get; }

    /// <summary> Gets the programmatic <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/> </summary>
    /// <value> A <see cref="string"/> uniquely identifying this object. </value>
    /// <remarks> This value must be be unqiue within its business object domain. </remarks>
    string UniqueIdentifier { get; }
  }
}