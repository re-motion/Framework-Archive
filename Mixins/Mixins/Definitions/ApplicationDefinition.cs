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
    public readonly DefinitionItemCollection<Type, BaseClassDefinition> BaseClasses =
        new DefinitionItemCollection<Type, BaseClassDefinition> (delegate (BaseClassDefinition b) { return b.Type; });
  }
}
