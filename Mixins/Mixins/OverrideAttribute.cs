using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
  public class OverrideAttribute : Attribute
  {
  }
}
