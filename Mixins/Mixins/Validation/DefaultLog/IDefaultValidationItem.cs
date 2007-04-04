using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Validation.DefaultLog
{
  public interface IDefaultValidationResultItem
  {
    IValidationRule Rule { get; }
    string Message { get; }
  }
}
