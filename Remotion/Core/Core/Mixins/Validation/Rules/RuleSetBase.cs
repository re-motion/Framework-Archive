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
using Remotion.Utilities;

namespace Remotion.Mixins.Validation.Rules
{
  public abstract class RuleSetBase : IRuleSet
  {
    public abstract void Install (ValidatingVisitor visitor);

    protected void SingleShould (bool test, IValidationLog log, IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("rule", rule);

      if (!test)
        log.Warn (rule);
      else
        log.Succeed (rule);
    }

    protected void SingleMust (bool test, IValidationLog log, IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("rule", rule);

      if (!test)
        log.Fail (rule);
      else
        log.Succeed (rule);
    }
  }
}
