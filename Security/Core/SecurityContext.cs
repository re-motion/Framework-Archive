using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Security
{
  public sealed class SecurityContext
  {
    private readonly string _class;
    private readonly string _owner;
    private readonly string _ownerGroup;
    private readonly string _ownerClient;
    private IDictionary<string, EnumWrapper> _states;
    private EnumWrapper[] _abstractRoles;

    public SecurityContext (Type classType)
      : this (classType, null, null, null, null, null)
    {
    }

    public SecurityContext (
        Type classType,
        string owner,
        string ownerGroup,
        string ownerClient,
        IDictionary<string, Enum> states,
        ICollection<Enum> abstractRoles)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("classType", classType, typeof (ISecurableType));

      List<EnumWrapper> abstractRoleList = new List<EnumWrapper> ();

      if (abstractRoles != null)
      {
        foreach (Enum abstractRole in abstractRoles)
        {
          Type roleType = abstractRole.GetType ();
          if (!Attribute.IsDefined (roleType, typeof (AbstractRoleAttribute), false))
          {
            string message = string.Format ("Enumerated Type '{0}' cannot be used as an abstract role. Valid abstract roles must have the {1} applied.",
                roleType, typeof (AbstractRoleAttribute).FullName);

            throw new ArgumentException (message, "abstractRoles");
          }

          abstractRoleList.Add (new EnumWrapper (abstractRole));
        }
      }

      _abstractRoles = abstractRoleList.ToArray ();

      Dictionary<string, EnumWrapper> securityStates = new Dictionary<string, EnumWrapper> ();

      if (states != null)
      {
        foreach (KeyValuePair<string, Enum> valuePair in states)
        {
          Type stateType = valuePair.Value.GetType ();
          if (!Attribute.IsDefined (stateType, typeof (SecurityStateAttribute), false))
          {
            string message = string.Format ("Enumerated Type '{0}' cannot be used as a security state. Valid security states must have the {1} applied.",
                stateType, typeof (SecurityStateAttribute).FullName);

            throw new ArgumentException (message, "states");
          }

          securityStates.Add (valuePair.Key, new EnumWrapper (valuePair.Value));
        }
      }

      _class = TypeUtility.GetPartialAssemblyQualifiedName (classType);
      _owner = owner;
      _ownerGroup = ownerGroup;
      _ownerClient = ownerClient;
      _abstractRoles = abstractRoleList.ToArray ();
      _states = securityStates;
    }

    public string Class
    {
      get { return _class; }
    }

    public string Owner
    {
      get { return _owner; }
    }

    public string OwnerGroup
    {
      get { return _ownerGroup; }
    }

    public string OwnerClient
    {
      get { return _ownerClient; }
    }

    public EnumWrapper[] AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public EnumWrapper GetState (string propertyName)
    {
      return _states[propertyName];
    }
  }
}
