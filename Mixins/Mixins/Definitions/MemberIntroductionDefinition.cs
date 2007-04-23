using System;
using Rubicon.Utilities;
using System.Reflection;

namespace Mixins.Definitions
{
  [Serializable]
  public class MemberIntroductionDefinition: IVisitableDefinition
  {
    private InterfaceIntroductionDefinition _declaringInterface;
    private MemberInfo _interfaceMember;
    private MemberDefinition _implementingMember;

    public MemberIntroductionDefinition (
        InterfaceIntroductionDefinition declaringInterface, MemberInfo interfaceMember, MemberDefinition implementingMember)
    {
      ArgumentUtility.CheckNotNull ("interfaceMember", interfaceMember);
      ArgumentUtility.CheckNotNull ("declaringInterface", declaringInterface);
      ArgumentUtility.CheckNotNull ("implementingMember", implementingMember);

      _declaringInterface = declaringInterface;
      _implementingMember = implementingMember;
      _interfaceMember = interfaceMember;
    }

    public InterfaceIntroductionDefinition DeclaringInterface
    {
      get { return _declaringInterface; }
    }

    public MemberInfo InterfaceMember
    {
      get { return _interfaceMember; }
    }

    public MemberDefinition ImplementingMember
    {
      get { return _implementingMember; }
    }

    public string FullName
    {
      get { return DeclaringInterface.FullName + "." + InterfaceMember.Name; }
    }

    public IVisitableDefinition Parent
    {
      get { return DeclaringInterface; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}