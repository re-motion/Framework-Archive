using System;
using Rubicon.Utilities;
using System.Reflection;
using System.Diagnostics;

namespace Mixins.Definitions
{
  [Serializable]
  [DebuggerDisplay ("{_type.FullName} introduced via {_implementer.FullName}")]
  public class InterfaceIntroductionDefinition : IVisitableDefinition
  {
    public readonly DefinitionItemCollection<MethodInfo, MethodIntroductionDefinition> IntroducedMethods =
        new DefinitionItemCollection<MethodInfo, MethodIntroductionDefinition> (delegate (MethodIntroductionDefinition m) { return m.InterfaceMember; });
    public readonly DefinitionItemCollection<PropertyInfo, PropertyIntroductionDefinition> IntroducedProperties =
        new DefinitionItemCollection<PropertyInfo, PropertyIntroductionDefinition> (delegate (PropertyIntroductionDefinition m) { return m.InterfaceMember; });
    public readonly DefinitionItemCollection<EventInfo, EventIntroductionDefinition> IntroducedEvents =
        new DefinitionItemCollection<EventInfo, EventIntroductionDefinition> (delegate (EventIntroductionDefinition m) { return m.InterfaceMember; });

    private Type _type;
    private MixinDefinition _implementer;

    public InterfaceIntroductionDefinition (Type type, MixinDefinition implementer)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      _type = type;
      _implementer = implementer;
    }

    public Type Type
    {
      get { return _type; }
    }

    public MixinDefinition Implementer
    {
      get { return _implementer; }
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }

    public BaseClassDefinition BaseClass
    {
      get { return Implementer.BaseClass; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      IntroducedMethods.Accept (visitor);
      IntroducedProperties.Accept (visitor);
      IntroducedEvents.Accept (visitor);
    }
  }
}
