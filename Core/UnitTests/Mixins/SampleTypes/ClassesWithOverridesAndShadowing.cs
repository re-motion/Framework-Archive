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
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class TargetForOverridesAndShadowing
  {
    public virtual void Method (int i) { }

    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    public virtual event EventHandler Event;
  }
  
  public class BaseWithOverrideAttributes
  {
    [OverrideTarget]
    public virtual void Method(int i)
    {
    }

    [OverrideTarget]
    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    [OverrideTarget]
    public virtual event EventHandler Event;
  }

  public class DerivedWithoutOverrideAttributes : BaseWithOverrideAttributes
  {
    public override void Method (int i)
    {
    }

    public override int Property
    {
      get { return 0; }
      set { }
    }

    public override event EventHandler Event;
  }

  public class DerivedNewWithAdditionalOverrideAttributes : BaseWithOverrideAttributes
  {
    [OverrideTarget]
    public new void Method (int i)
    {
    }

    [OverrideTarget]
    public new int Property
    {
      get { return 0; }
      set { }
    }

    [OverrideTarget]
    public new event EventHandler Event;
  }

  public class BaseWithoutOverrideAttributes
  {
    public virtual void Method (int i)
    {
    }

    public virtual int Property
    {
      get { return 0; }
      set { }
    }

    public virtual event EventHandler Event;
  }

  public class DerivedNewWithOverrideAttributes : BaseWithoutOverrideAttributes
  {
    [OverrideTarget]
    public new void Method (int i)
    {
    }

    [OverrideTarget]
    public new int Property
    {
      get { return 0; }
      set { }
    }

    [OverrideTarget]
    public new event EventHandler Event;
  }

  
  public class DerivedNewWithoutOverrideAttributes : BaseWithoutOverrideAttributes
  {
    public new void Method (int i)
    {
    }

    public new int Property
    {
      get { return 0; }
      set { }
    }

    public new event EventHandler Event;
  }
}
