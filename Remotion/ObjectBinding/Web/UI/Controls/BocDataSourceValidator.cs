// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FluentValidation.Results;
using Remotion.FunctionalProgramming;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public class BocDataSourceValidator : BaseValidator
  {
    private IReadOnlyCollection<ValidationFailure> _unhandledFailures = new List<ValidationFailure>();
    
    public IEnumerable<ValidationFailure> ApplyValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      var dataSourceControl = control as BindableObjectDataSourceControl;
      if (dataSourceControl == null)
        throw new InvalidOperationException ("BocDataSourceValidator may only be applied to controls of type BindableObjectDataSourceControl.");

      var namingContainers = dataSourceControl.GetBoundControlsWithValidBinding().OfType<IBusinessObjectBoundEditableWebControl>().Select (c=>c.NamingContainer).Distinct();
      var unhandledFailures = new List<ValidationFailure>(failures);
      foreach (var namingContainer in namingContainers)
      {
        var validators = EnumerableUtility.SelectRecursiveDepthFirst (namingContainer, c => c.Controls.Cast<Control>()).OfType<BocValidator>();
        foreach (var validator in validators)
        {
          unhandledFailures = validator.ApplyValidationFailures (unhandledFailures).ToList();
          validator.Validate();
        }
      }

      _unhandledFailures = unhandledFailures.AsReadOnly();
      ErrorMessage = string.Join (Environment.NewLine, _unhandledFailures.Select (f => f.ErrorMessage));
      return _unhandledFailures;
    }

    protected override bool EvaluateIsValid ()
    {
      return !_unhandledFailures.Any();
    }

    //TODO AO: check with MK!
    protected override bool ControlPropertiesValid ()
    {
      return true;
    }
  }
}