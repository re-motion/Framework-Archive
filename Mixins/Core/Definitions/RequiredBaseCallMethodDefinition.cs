using System;
using System.Diagnostics;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{InterfaceMethod}")]
  public class RequiredBaseCallMethodDefinition : IVisitableDefinition
  {
    private RequiredBaseCallTypeDefinition _declaringType;
    private readonly MethodInfo _interfaceMethod;
    private readonly MethodDefinition _implementingMethod;

    public RequiredBaseCallMethodDefinition (RequiredBaseCallTypeDefinition declaringType, MethodInfo interfaceMethod,
        MethodDefinition implementingMethod)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("implementingMethod", implementingMethod);
      ArgumentUtility.CheckNotNull ("interfaceMethod", interfaceMethod);

      _declaringType = declaringType;
      _interfaceMethod = interfaceMethod;
      _implementingMethod = implementingMethod;
    }

    public RequiredBaseCallTypeDefinition DeclaringType
    {
      get { return _declaringType; }
    }

    public MethodInfo InterfaceMethod
    {
      get { return _interfaceMethod; }
    }

    public MethodDefinition ImplementingMethod
    {
      get { return _implementingMethod; }
    }

    public string FullName
    {
      get { return _declaringType.FullName + "." + _interfaceMethod.Name; }
    }

    public IVisitableDefinition Parent
    {
      get { return _declaringType; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
