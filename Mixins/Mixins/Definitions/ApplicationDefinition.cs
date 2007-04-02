using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Definitions
{
  /// <summary>
  /// Set of base class definitions for configuríng a CodeBuilder.
  /// </summary>
  public class ApplicationDefinition
  {
    private Dictionary<Type, BaseClassDefinition> _baseClasses = new Dictionary<Type, BaseClassDefinition>();

    public IEnumerable<BaseClassDefinition> BaseClasses
    {
      get { return _baseClasses.Values; }
    }

    public void AddBaseClass (BaseClassDefinition baseClass)
    {
      if (HasBaseClass(baseClass.Type))
      {
        string message = string.Format("Definition for type {0} already exists:", baseClass.Type.FullName);
        throw new InvalidOperationException(message);
      }
      _baseClasses.Add (baseClass.Type, baseClass);
    }

    public BaseClassDefinition GetBaseClass (Type type)
    {
      return HasBaseClass (type) ? _baseClasses[type] : null;
    }

    public bool HasBaseClass (Type type)
    {
      return _baseClasses.ContainsKey (type);
    }
  }
}
