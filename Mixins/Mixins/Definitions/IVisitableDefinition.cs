using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Definitions
{
  public interface IVisitableDefinition
  {
    void Accept (IDefinitionVisitor visitor);
    string FullName { get; }
  }
}
