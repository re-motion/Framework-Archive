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

    void Succeed (string message);
    void Warn (string message);
    void Fail (string message);

    void UnexpectedException (Exception ex);
  }
}
