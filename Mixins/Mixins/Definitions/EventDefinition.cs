using System;
using System.Collections.Generic;
using System.Reflection;
using Mixins.Utilities;
using Rubicon.Utilities;

namespace Mixins.Definitions
{
  [Serializable]
  public class EventDefinition : MemberDefinition, IVisitableDefinition
  {
    private static SignatureChecker s_signatureChecker = new SignatureChecker();

    public readonly DefinitionItemCollection<Type, EventDefinition> Overrides =
        new DefinitionItemCollection<Type, EventDefinition> (delegate (EventDefinition m) { return m.DeclaringClass.Type; });

    private EventDefinition _base;
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

    public override MemberDefinition BaseAsMember
    {
      get { return _base; }
      set
      {
        if (value == null || value is EventDefinition)
        {
          _base = (EventDefinition) value;
          AddMethod.Base = _base == null ? null : _base.AddMethod;
          RemoveMethod.Base = _base == null ? null : _base.RemoveMethod;
        }
        else
          throw new ArgumentException ("Base must be EventDefinition or null.", "value");
      }
    }

    public EventDefinition Base
    {
      get { return _base; }
      set { BaseAsMember = value; }
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
      if (overriderEvent == null)
        return false;
      else
        return IsSignatureCompatibleWithEvent (overriderEvent);
    }

    private bool IsSignatureCompatibleWithEvent (EventDefinition overrider)
    {
      ArgumentUtility.CheckNotNull ("overrider", overrider);
      return s_signatureChecker.EventSignaturesMatch (EventInfo, overrider.EventInfo);
    }

    internal override void AddOverride (MemberDefinition member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      EventDefinition overrider = member as EventDefinition;
      if (overrider == null)
      {
        string message = string.Format ("Member {0} cannot override event {1} - it is not an event.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      Overrides.Add (overrider);

      AddMethod.AddOverride (overrider.AddMethod);
      RemoveMethod.AddOverride (overrider.RemoveMethod);
    }

    public override IEnumerable<MemberDefinition> GetOverridesAsMemberDefinitions()
    {
      foreach (EventDefinition overrider in Overrides)
        yield return overrider;
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      base.AcceptForChildren (visitor);

      AddMethod.Accept (visitor);
      RemoveMethod.Accept (visitor);
    }
  }
}