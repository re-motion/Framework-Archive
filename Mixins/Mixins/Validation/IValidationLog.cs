using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation
{
  public interface IValidationLog
  {
    void ValidationStartsFor (IVisitableDefinition definition);
    void ValidationEndsFor (IVisitableDefinition definition);

    void Succeed (IValidationRule rule);
    void Warn (IValidationRule rule);
    void Fail (IValidationRule rule);

    void UnexpectedException<TDefinition> (IValidationRule<TDefinition> rule, Exception ex) where TDefinition : IVisitableDefinition;
  }
}
