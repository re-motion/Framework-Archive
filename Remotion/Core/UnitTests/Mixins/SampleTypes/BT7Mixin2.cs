// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public interface IBT7Mixin2Reqs : IBT7Mixin3, IBaseType7
  {
  }

  public interface IBT7Mixin2
  {
    string One<T> (T t);
    string Two ();
    string Three ();
    string Four ();
  }

  [Extends (typeof (BaseType7))]
  public class BT7Mixin2 : Mixin<BaseType7, IBT7Mixin2Reqs>, IBT7Mixin2
  {
    [OverrideTarget]
    public virtual string One<T> (T t)
    {
      return "BT7Mixin2.One(" + t + ")-" + ((IBaseType7) Base).One (t) + "-" + ((IBT7Mixin3) Base).One (t) + "-" + Base.Two() + "-" + This.Two();
    }

    [OverrideTarget]
    public virtual string Two()
    {
      return "BT7Mixin2.Two";
    }

    [OverrideTarget]
    public virtual string Three ()
    {
      return "BT7Mixin2.Three-" + Base.Three ();
    }

    [OverrideTarget]
    public virtual string Four ()
    {
      return "BT7Mixin2.Four-" + Base.Four() + "-" + Base.NotOverridden();
    }
  }
}
