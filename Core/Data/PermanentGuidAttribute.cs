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

    public PermanentGuidAttribute (string value)
    {
      _value = new Guid (value);
    }

    public Guid Value
    {
      get { return _value; }
    }
  }
}