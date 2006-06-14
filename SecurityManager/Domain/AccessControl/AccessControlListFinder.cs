using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public class AccessControlListFinder
  {
    public AccessControlList Find (ClientTransaction transaction, SecurityContext context)
    {
      // TODO: make unit tests
      ArgumentUtility.CheckNotNull ("context", context);

      SecurableClassDefinition classDefinition = SecurableClassDefinition.FindByName (transaction, context.Class);
      if (classDefinition == null)
        throw new AccessControlException (string.Format ("The class '{0}' cannot be found.", context.Class));

      return Find (classDefinition, context);
    }

    public AccessControlList Find (SecurableClassDefinition classDefinition, SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("context", context);

      StateCombination foundStateCombination = classDefinition.FindStateCombination (GetStates (classDefinition, context));
      return foundStateCombination.AccessControlList;
    }

    private List<StateDefinition> GetStates (SecurableClassDefinition classDefinition, SecurityContext context)
    {
      List<StateDefinition> states = new List<StateDefinition> ();

      if (context.IsStateless)
        return states;

      foreach (StatePropertyDefinition property in classDefinition.StateProperties)
      {
        if (!context.ContainsState (property.Name))
          throw CreateAccessControlException ("The state '{0}' is missing in the security context.", property.Name);

        EnumWrapper enumWrapper = context.GetState (property.Name);

        if (!property.ContainsState (enumWrapper.Name))
        {
          throw CreateAccessControlException ("The state '{0}' is not defined for the property '{1}' of securable class '{2}'.",
              enumWrapper.Name, property.Name, classDefinition.Name);
        }

        states.Add (property.GetState (enumWrapper.Name));
      }

      if (context.GetNumberOfStates () > classDefinition.StateProperties.Count)
        throw CreateAccessControlException ("The security context contains at least one state not defined by securable class '{0}'.", classDefinition.Name);

      return states;
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}
