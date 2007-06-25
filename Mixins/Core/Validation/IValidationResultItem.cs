using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.Validation
{
  public interface IDefaultValidationResultItem
  {
    IValidationRule Rule { get; }
    string Message { get; }
  }
}
