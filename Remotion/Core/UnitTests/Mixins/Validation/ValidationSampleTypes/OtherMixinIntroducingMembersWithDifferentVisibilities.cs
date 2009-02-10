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
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.Validation.ValidationSampleTypes
{
  public interface IOtherMixinIntroducingMembersWithDifferentVisibilities
  {
    void MethodWithDefaultVisibility ();
    int PropertyWithDefaultVisibility { get; set; }
    event EventHandler EventWithDefaultVisibility;

    void MethodWithPublicVisibility ();
    int PropertyWithPublicVisibility { get; set; }
    event EventHandler EventWithPublicVisibility;
  }

  public class OtherMixinIntroducingMembersWithDifferentVisibilities : IOtherMixinIntroducingMembersWithDifferentVisibilities
  {
    public void MethodWithDefaultVisibility ()
    {
    }

    public int PropertyWithDefaultVisibility
    {
      get { return 0; }
      set { Dev.Null = value; }
    }

    public event EventHandler EventWithDefaultVisibility;

    [MemberVisibility (MemberVisibility.Public)]
    public void MethodWithPublicVisibility ()
    {
    }

    [MemberVisibility (MemberVisibility.Public)]
    public int PropertyWithPublicVisibility
    {
      get { return 0; }
      set { Dev.Null = value; }
    }

    [MemberVisibility (MemberVisibility.Public)]
    public event EventHandler EventWithPublicVisibility;
  }
}
