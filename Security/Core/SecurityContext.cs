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
    private IDictionary<string, Enum> _states;
    private string[] _abstractRoles;

    public SecurityContext (
        string fullClassName,
        string owner,
        string ownerGroup,
        string ownerClient,
        IDictionary<string, Enum> states,
        ICollection<Enum> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fullClassName", fullClassName);

      List<string> abstractRoleList = new List<string> ();

      if (abstractRoles != null)
      {
        foreach (Enum abstractRole in abstractRoles)
        {
          Type roleType = abstractRole.GetType ();
          if (!roleType.IsDefined (typeof (AbstractRoleAttribute), false))
          {
            string message = string.Format ("Enumerated Type '{0}' cannot be used as an abstract role. Valid abstract roles must have the"
                + " Rubicon.Security.AbstractRoleAttribute applied.", roleType);

            throw new ArgumentException (message, "abstractRoles");
          }

          string role = roleType.FullName + "." + abstractRole.ToString () + ", " + roleType.Assembly.GetName ().Name;

          abstractRoleList.Add (role);
        }
      }

      _abstractRoles = abstractRoleList.ToArray ();

      Dictionary<string, Enum> securityStates = new Dictionary<string, Enum> ();

      if (states != null)
      {
        foreach (KeyValuePair<string, Enum> valuePair in states)
        {
          Type stateType = valuePair.Value.GetType ();
          if (!stateType.IsDefined (typeof (SecurityStateAttribute), false))
          {
            string message = string.Format ("Enumerated Type '{0}' cannot be used as a security state. Valid security states must have the"
                + " Rubicon.Security.SecurityStateAttribute applied.", stateType);

            throw new ArgumentException (message, "states");
          }

          securityStates.Add (valuePair.Key, valuePair.Value);
        }
      }

      _class = fullClassName;
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

    public string[] AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public Enum GetState (string propertyName)
    {
      return _states[propertyName];
    }
  }
}
