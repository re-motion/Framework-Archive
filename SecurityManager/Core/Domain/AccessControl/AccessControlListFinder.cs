using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using System.Collections;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public class AccessControlListFinder : IAccessControlListFinder
  {
    /// <exception cref="AccessControlException">
    ///   The <see cref="SecurableClassDefinition"/> is not found.<br/>- or -<br/>
    ///   A matching <see cref="AccessControlList"/> is not found.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and a <see cref="StatePropertyDefinition"/> is missing.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and contains an invalid state for a <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public AccessControlList Find (ClientTransaction transaction, SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      SecurableClassDefinition classDefinition = SecurableClassDefinition.FindByName (transaction, context.Class);
      if (classDefinition == null)
        throw CreateAccessControlException ("The securable class '{0}' cannot be found.", context.Class);

      return Find (classDefinition, context);
    }

    /// <exception cref="AccessControlException">
    ///   A matching <see cref="AccessControlList"/> is not found.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and a <see cref="StatePropertyDefinition"/> is missing.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and contains an invalid state for a <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public AccessControlList Find (SecurableClassDefinition classDefinition, SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("context", context);

      StateCombination foundStateCombination = null;

      while (foundStateCombination == null && classDefinition != null)
      {
        foundStateCombination = FindStateCombination (classDefinition, context);
        classDefinition = classDefinition.BaseClass;
      }

      if (foundStateCombination == null)
        throw CreateAccessControlException ("The ACL for the securable class '{0}' could not be found.", context.Class);

      return foundStateCombination.AccessControlList;
    }

    private StateCombination FindStateCombination (SecurableClassDefinition classDefinition, SecurityContext context)
    {
      List<StateDefinition> states = GetStates (classDefinition.StateProperties, context);
      if (states == null)
        return null;

      return classDefinition.FindStateCombination (states);
    }

    private List<StateDefinition> GetStates (IList stateProperties, SecurityContext context)
    {
      List<StateDefinition> states = new List<StateDefinition> ();

      if (context.IsStateless)
        return states;

      foreach (StatePropertyDefinition property in stateProperties)
      {
        if (!context.ContainsState (property.Name))
          throw CreateAccessControlException ("The state '{0}' is missing in the security context.", property.Name);

        EnumWrapper enumWrapper = context.GetState (property.Name);

        if (!property.ContainsState (enumWrapper.Name))
        {
          throw CreateAccessControlException ("The state '{0}' is not defined for the property '{1}' of the securable class '{2}' or its base classes.",
              enumWrapper.Name, property.Name, context.Class);
        }

        states.Add (property.GetState (enumWrapper.Name));
      }

      if (context.GetNumberOfStates () > stateProperties.Count)
        return null;

      return states;
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}
