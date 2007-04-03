using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
  //TODO: Doc, Tests
  /// <summary>
  /// Declares a relation as bidirectional. Use <see cref="ContainsForeignKey"/> to indicate the the foreign key side in a one-to-one relation.
  /// </summary>
  public class DBBidirectionalRelationAttribute: BidirectionalRelationAttribute
  {
    private bool _containsForeignKey = false;
    private string _sortExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="DBBidirectionalRelationAttribute"/> class with the name of the oppsite property
    /// and the <see cref="ContainsForeignKey"/> value.
    /// </summary>
    /// <param name="oppositeProperty">The name of the opposite property. Must not be <see langword="null" /> or empty.</param>
    public DBBidirectionalRelationAttribute (string oppositeProperty)
        : base (oppositeProperty)
    {
    }

    /// <summary>Gets or sets a flag that indicates the foreign key side in a one-to-one relation.</summary>
    public bool ContainsForeignKey
    {
      get { return _containsForeignKey; }
      set { _containsForeignKey = value; }
    }

    public string SortExpression
    {
      get { return _sortExpression; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        value = value.Trim();
        ArgumentUtility.CheckNotNullOrEmpty ("value", value);
        _sortExpression = StringUtility.EmptyToNull (value);
      }
    }
  }
}