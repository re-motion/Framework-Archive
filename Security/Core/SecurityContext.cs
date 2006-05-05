using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class SecurityContext
  {
    private readonly string _class;
    private readonly string _owner;
    private readonly string _ownerGroup;
    private readonly string _ownerClient;
    private Dictionary<string, string> _states;
    private List<string> _abstractRoles;
	
    public SecurityContext (
        string fullClassName,
        string owner,
        string ownerGroup,
        string ownerClient, 
        Dictionary<string, string> states,
        List<string> abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fullClassName", fullClassName);

      _class = fullClassName;
      _owner = owner;
      _ownerGroup = ownerGroup;
      _ownerClient = ownerClient;
      _states = states;
      _abstractRoles = abstractRoles;
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

    public Dictionary<string, string> States
    {
      get { return _states; }
    }

    public List<string> AbstractRoles
    {
      get { return _abstractRoles; }
    }
  }
}
