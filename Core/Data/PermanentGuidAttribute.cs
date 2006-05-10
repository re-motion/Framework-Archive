using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data
{
  /// <summary>
  ///   Supplies an identifier that should remain constant even accross refactorings. Can be applied to reference types and properties.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property)]
  public class PermanentGuidAttribute : Attribute
  {
    private readonly Guid _value;

    /// <summary>
    ///   Initializes a new instance of the <see cref="PermanentGuidAttribute"/> class.
    /// </summary>
    /// <param name="value"> The <see cref="String"/> representation of a <see cref="Guid"/>. </param>
    public PermanentGuidAttribute (string value)
    {
      _value = new Guid (value);
    }

    /// <summary>
    ///   Gets the <see cref="Guid"/> supplied during initialization.
    /// </summary>
    public Guid Value
    {
      get { return _value; }
    }
  }
}