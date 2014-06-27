﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core
{
  public class AccessTypeExtensionsTest
  {
    [Test]
    public void HasAccess_WithSingleRequiredAccessType_IsMatchingSingleAllowedAccessType_ReturnsTrue ()
    {
      bool hasAccess = AccessTypeExtensions.HasAccess (
          new[] { AccessType.Get (GeneralAccessTypes.Edit) },
          new[] { AccessType.Get (GeneralAccessTypes.Edit) });

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithSingleRequiredAccessType_IsNotMatchingSingleAllowedAccessType_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.HasAccess (
          new[] { AccessType.Get (GeneralAccessTypes.Create) },
          new[] { AccessType.Get (GeneralAccessTypes.Edit) });

      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void HasAccess_WithSingleRequiredAccessType_AreNotPartOfAllowedAccessTypes_ReturnsTrue ()
    {
      bool hasAccess = AccessTypeExtensions.HasAccess (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Create),
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          },
          new[] { AccessType.Get (GeneralAccessTypes.Read) });

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithAllRequiredAccessTypes_ArePartOfAllowedAccessTypes_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.HasAccess (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Create),
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          },
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Create)
          });

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void HasAccess_WithoutAllRequiredAccessTypes_ArePartOfAllowedAccessTypes_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.HasAccess (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Create),
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          },
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Find)
          });

      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void HasAccess_WithEmptySetOfAllowedAccessTypes_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.HasAccess (
          new AccessType[0],
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Find)
          });

      Assert.That (hasAccess, Is.EqualTo (false));
    }
  }
}