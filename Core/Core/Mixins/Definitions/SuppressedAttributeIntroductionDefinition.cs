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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{FullName}, suppressed on {Attribute.DeclaringDefinition.FullName} by {Suppressor.DeclaringDefinition.FullName}")]
  public class SuppressedAttributeIntroductionDefinition : IVisitableDefinition
  {
    public readonly IAttributeIntroductionTarget Target;
    public readonly AttributeDefinition Attribute;
    public readonly AttributeDefinition Suppressor;

    public SuppressedAttributeIntroductionDefinition (IAttributeIntroductionTarget target, AttributeDefinition attribute,
        AttributeDefinition suppressor)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      ArgumentUtility.CheckNotNull ("suppressor", suppressor);

      Target = target;
      Attribute = attribute;
      Suppressor = suppressor;
    }

    public Type AttributeType
    {
      get { return Attribute.AttributeType; }
    }

    public string FullName
    {
      get { return Attribute.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Target; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
