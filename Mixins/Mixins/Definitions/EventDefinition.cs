using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class EventDefinition : MemberDefinition, IVisitableDefinition
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker();

    public readonly DefinitionItemCollection<Type, EventDefinition> Overrides =
        new DefinitionItemCollection<Type, EventDefinition> (delegate (EventDefinition m) { return m.DeclaringClass.Type; });

    private MethodDefinition _addMethod;
    private MethodDefinition _removeMethod;

    public EventDefinition (EventInfo memberInfo, ClassDefinition declaringClass, MethodDefinition addMethod, MethodDefinition removeMethod)
        : base (memberInfo, declaringClass)
    {
      ArgumentUtility.CheckNotNull ("addMethod", addMethod);
      ArgumentUtility.CheckNotNull ("removeMethod", removeMethod);

      _addMethod = addMethod;
      _removeMethod = removeMethod;
    }

    public EventInfo EventInfo
    {
      get { return (EventInfo) MemberInfo; }
    }

    public MethodDefinition AddMethod
    {
      get { return _addMethod; }
    }

    public MethodDefinition RemoveMethod
    {
      get { return _removeMethod; }
    }

    protected override bool IsSignatureCompatibleWith (MemberDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      EventDefinition overriderEvent = overrider as EventDefinition;
      if (overrider == null)
      {
        return false;
      }
      else
      {
        return IsSignatureCompatibleWithEvent (overriderEvent);
      }
    }

    private bool IsSignatureCompatibleWithEvent (EventDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.EventSignaturesMatch (EventInfo, overrider.EventInfo);
    }

    public override void AddOverride (MemberDefinition member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      EventDefinition Event = member as EventDefinition;
      if (Event == null)
      {
        string message = string.Format ("Member {0} cannot override Event {1} - it is not a Event.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      Overrides.Add (Event);
    }

    public override IEnumerable<MemberDefinition> GetOverridesAsMemberDefinitions ()
    {
      foreach (EventDefinition overrider in Overrides)
      {
        yield return overrider;
      }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      base.AcceptForChildren (visitor);

      if (AddMethod != null)
      {
        AddMethod.Accept (visitor);
      }
      if (RemoveMethod != null)
      {
        RemoveMethod.Accept (visitor);
      }
    }
  }
}