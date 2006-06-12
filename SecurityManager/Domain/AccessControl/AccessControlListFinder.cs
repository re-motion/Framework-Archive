using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public class AccessControlListFinder
  {
    public AccessControlList Find (SecurableClassDefinition classDefinition, SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("context", context);

      List<StateDefinition> states = new List<StateDefinition> ();
      foreach (StatePropertyReference propertyReference in classDefinition.StatePropertyReferences)
      {
        EnumWrapper enumWrapper = context.GetState (propertyReference.StateProperty.Name);
        states.Add (propertyReference.StateProperty.GetStateByName (enumWrapper.Value));
      }

      StateCombination foundStateCombination = FindStateCombination (classDefinition, states);
      return foundStateCombination.AccessControlList;
    }

    private StateCombination FindStateCombination (SecurableClassDefinition classDefinition, List<StateDefinition> states)
    {
      foreach (StateCombination stateCombination in classDefinition.StateCombinations)
      {
        if (stateCombination.MatchesStates (states))
          return stateCombination;
      }

      return null;
    }
  }
}
