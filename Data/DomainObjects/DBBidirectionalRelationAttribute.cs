using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Declares a relation as bidirectional. Use <see cref="ContainsForeignKey"/> to indicate the the foreign key side in a one-to-one relation.
  /// </summary>
  public class DBBidirectionalRelationAttribute : BidirectionalRelationAttribute
  {
    private bool _containsForeignKey = false;

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
  }
}