using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Definitions
{
  /// <summary>
  /// Set of base class definitions for configuríng a CodeBuilder.
  /// </summary>
  public class ApplicationDefinition : IVisitableDefinition
  {
    public readonly DefinitionItemCollection<Type, BaseClassDefinition> BaseClasses =
        new DefinitionItemCollection<Type, BaseClassDefinition> (delegate (BaseClassDefinition b) { return b.Type; });

    public void Accept (IDefinitionVisitor visitor)
    {
      visitor.Visit (this);
      
      BaseClasses.Accept (visitor);
    }

    public string FullName
    {
      get { return "<application>"; }
    }
  }
}
