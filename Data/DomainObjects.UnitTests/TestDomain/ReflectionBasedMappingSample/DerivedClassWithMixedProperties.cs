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

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [Instantiable]
  public abstract class DerivedClassWithMixedProperties : ClassWithMixedProperties
  {
    protected DerivedClassWithMixedProperties()
    {
    }

    public override int Int32
    {
      get { return 0; }
      set { }
    }

    public abstract string OtherString { get; set; }

    [DBColumn ("NewString")]
    public new abstract string String { get; set; }

    [DBColumn ("DerivedPrivateString")]
    private string PrivateString
    {
      get {
        return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.PrivateString"]
            .GetValue<string> ();
      }
      set
      {
        Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithMixedProperties.PrivateString"]
            .SetValue (value);
      }
    }
  }
}
