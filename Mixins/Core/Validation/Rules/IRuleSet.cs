using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Validation.Rules
{
  public interface IRuleSet
  {
    void Install (ValidatingVisitor visitor);
  }
}
