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

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  public abstract class SpecialOfficial : Official
  {
    public static new SpecialOfficial NewObject ()
    {
      return NewObject<SpecialOfficial>().With();
    }

    protected SpecialOfficial()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 255)]
    public abstract string Speciality { get; set;}
  }
}
