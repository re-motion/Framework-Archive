using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins
{
  [AttributeUsage(AttributeTargets.Method)]
  public class OverrideAttribute : Attribute
  {
  }
}
