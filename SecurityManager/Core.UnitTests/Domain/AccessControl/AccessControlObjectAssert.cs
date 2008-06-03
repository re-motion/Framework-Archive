/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  public static class AccessControlObjectAssert
  {
    public static void ContainsGroup (string groupUniqueIdentifier, IEnumerable<Group> groups)
    {
      foreach (Group group in groups)
      {
        if (group.UniqueIdentifier == groupUniqueIdentifier)
          return;
      }

      Assert.Fail ("The list does not contain the group '{0}'.", groupUniqueIdentifier);
    }
  }
}
