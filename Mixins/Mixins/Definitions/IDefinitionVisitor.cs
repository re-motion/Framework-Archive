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
    void Visit (MemberIntroductionDefinition definition);
    void Visit (MethodDefinition method);
    void Visit (PropertyDefinition property);
    void Visit (EventDefinition eventDefintion);
    void Visit (RequiredFaceTypeDefinition requiredFaceType);
    void Visit (RequiredBaseCallTypeDefinition requiredBaseCallType);
    void Visit (ThisDependencyDefinition thisDependency);
    void Visit (BaseDependencyDefinition baseDependency);
  }
}
