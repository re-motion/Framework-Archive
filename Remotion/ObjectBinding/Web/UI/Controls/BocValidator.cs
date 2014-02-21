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
using System.Web.UI.WebControls;
using FluentValidation.Results;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public class BocValidator : BaseValidator
  {
    protected IReadOnlyCollection<ValidationFailure> ValidationFailures = new List<ValidationFailure>();

    public IEnumerable<ValidationFailure> ApplyValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      var bocControl = control as BusinessObjectBoundEditableWebControl;
      if (bocControl == null)
        throw new InvalidOperationException ("BocValidator may only be applied to controls of type BusinessObjectBoundEditableWebControl");

      var validationFailures = failures.ToArray();
      ValidationFailures = validationFailures.Where (f => f.PropertyName.EndsWith (bocControl.PropertyIdentifier)).ToList ().AsReadOnly (); //TODO: check with MK -> control property path is not the full-path
      ErrorMessage = string.Join (Environment.NewLine, ValidationFailures.Select (f => f.ErrorMessage));
      
      return validationFailures.Except (ValidationFailures);
    }

    public override string ToolTip
    {
      get { return string.Join (Environment.NewLine, ValidationFailures.Select (f => f.ErrorMessage)); } //TODO AO: check weith MK
    }

    protected override bool EvaluateIsValid ()
    {
      return !ValidationFailures.Any();
    }
  }
}