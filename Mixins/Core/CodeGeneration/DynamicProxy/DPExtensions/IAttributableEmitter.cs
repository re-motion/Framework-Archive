using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;

namespace Mixins.CodeGeneration.DynamicProxy.DPExtensions
{
  public interface IAttributableEmitter
  {
    void AddCustomAttribute (CustomAttributeBuilder customAttribute);
  }
}
