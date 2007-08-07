using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.Definitions
{
  public interface IDefinitionVisitor
  {
    void Visit (TargetClassDefinition targetClass);
    void Visit (MixinDefinition mixin);
    void Visit (InterfaceIntroductionDefinition interfaceIntroduction);
    void Visit (SuppressedInterfaceIntroductionDefinition definition);
    void Visit (MethodIntroductionDefinition methodIntroduction);
    void Visit (PropertyIntroductionDefinition propertyIntroduction);
    void Visit (EventIntroductionDefinition eventIntroduction);
    void Visit (MethodDefinition method);
    void Visit (PropertyDefinition property);
    void Visit (EventDefinition eventDefintion);
    void Visit (RequiredFaceTypeDefinition requiredFaceType);
    void Visit (RequiredBaseCallTypeDefinition requiredBaseCallType);
    void Visit (RequiredMethodDefinition definition);
    void Visit (ThisDependencyDefinition thisDependency);
    void Visit (BaseDependencyDefinition baseDependency);
    void Visit (AttributeDefinition attribute);
    void Visit (AttributeIntroductionDefinition attributeIntroduction);
  }
}
