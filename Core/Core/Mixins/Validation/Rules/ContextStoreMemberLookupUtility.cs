// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class ContextStoreMemberLookupUtility<TMemberDefinition>
      where TMemberDefinition : MemberDefinitionBase
  {
    public IEnumerable<TMemberDefinition> GetCachedMembersByName (IDataStore<object, object> contextStore, TargetClassDefinition targetClass, string name)
    {
      Tuple<string, TargetClassDefinition> cacheKey =
          Tuple.NewTuple (typeof (ContextStoreMemberLookupUtility<TMemberDefinition>).FullName + ".GetCachedMembersByName", targetClass);

      var methodDefinitions = (MultiDictionary<string, TMemberDefinition>)
          contextStore.GetOrCreateValue (cacheKey, delegate { return GetUncachedMethodDefinitions (targetClass); });
      return methodDefinitions[name];
    }

    private MultiDictionary<string, TMemberDefinition> GetUncachedMethodDefinitions (TargetClassDefinition targetClass)
    {
      var memberDefinitions = new MultiDictionary<string, TMemberDefinition> ();
      foreach (MemberDefinitionBase memberDefinition in targetClass.GetAllMembers ())
      {
        var castMemberDefinition = memberDefinition as TMemberDefinition;
        if (castMemberDefinition != null)
          memberDefinitions.Add (memberDefinition.Name, castMemberDefinition);
      }
      return memberDefinitions;
    }
  }
}
