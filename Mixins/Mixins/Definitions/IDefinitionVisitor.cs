using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.Definitions
{
  public interface IDefinitionVisitor
  {
    void Visit (ApplicationDefinition application);
    void Visit (BaseClassDefinition baseClass);
    void Visit (MixinDefinition mixin);
    void Visit (InterfaceIntroductionDefinition interfaceIntroduction);
    void Visit (MethodDefinition method);
    void Visit (RequiredFaceTypeDefinition requiredFaceType);
    void Visit (RequiredBaseCallTypeDefinition requiredBaseCallType);
  }
}
