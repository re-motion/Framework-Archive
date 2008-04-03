using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Reflection.Emit;

namespace Remotion.CodeGeneration
{
  public interface IAttributableEmitter
  {
    void AddCustomAttribute (CustomAttributeBuilder customAttribute);
  }
}