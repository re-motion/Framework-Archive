// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class AclProbe_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AclProbe>
  {
    public override void ToText (AclProbe x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AclProbe> ("").e("token",x.SecurityToken).e("conditions",x.AccessConditions) .ie ();
    }
  }
}