/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  public class DerivedClassWithTwoBaseReferencesViaMixins : TargetClassReceivingTwoReferencesToDerivedClass
  {
    public new static DerivedClassWithTwoBaseReferencesViaMixins NewObject ()
    {
      return NewObject<DerivedClassWithTwoBaseReferencesViaMixins> ().With ();
    }

    [DBBidirectionalRelation ("MyDerived1")]
    public virtual TargetClassReceivingTwoReferencesToDerivedClass MyBase1 { get; set; }
    [DBBidirectionalRelation ("MyDerived2")]
    public virtual TargetClassReceivingTwoReferencesToDerivedClass MyBase2 { get; set; }
  }
}