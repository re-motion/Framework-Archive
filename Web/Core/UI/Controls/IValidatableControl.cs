using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI
{

/// <summary>
///   Provides the ability to register validators with their target control and call validate on the web controls themselves.
/// </summary>
/// <remarks>
///   Use <see cref="ValidatableControlInitializer"/> to register all validators with their validatable controls.
/// </remarks>
public interface IValidatableControl: IControl
{
  /// <summary>
  ///   Registers a validator that references this control.
  /// </summary>
  /// <remarks>
  ///   The control may choose to ignore this call.
  /// </remarks>
  void RegisterValidator (BaseValidator validator);

  /// <summary>
  ///   Calls <see cref="BaseValidator.Validate"/> on all registered validators.
  /// </summary>
  /// <returns> True, if all validators validated. </returns>
  bool Validate ();
}

/// <summary>
///   Initializes validators for <see cref="IValidatableControl"/>.
/// </summary>
public class ValidatableControlInitializer
{
  /// <summary>
  ///   Registers validators with their <see cref="IValidatableControl"/> web controls.
  /// </summary>
  /// <remarks>
  ///   All <see cref="BaseValidator"/> controls within <paramref cref="control"/> (or its children) that
  ///   validate a <see cref="IValidatableControl"/> control are registered. This method is best called
  ///   from f
  /// </remarks>
  public static void InitializeValidatableControls (Control control)
  {
    BaseValidator[] validators = (BaseValidator[]) ControlHelper.GetControlsRecursive (control.NamingContainer, typeof (BaseValidator));
    foreach (BaseValidator validator in validators)
    {
      IValidatableControl validatableControl = validator.NamingContainer.FindControl (validator.ControlToValidate) as IValidatableControl;
      if (control != null)
        validatableControl.RegisterValidator (validator);
    }
  }

  private Control _control;
  private bool _initialized;

  /// <summary>
  ///   Creates a new initializer for <c>control</c> and all sub-controls.
  /// </summary>
  public ValidatableControlInitializer (Control control)
  {
    _control = control;
    _initialized = false;
  }

  /// <summary>
  ///   When called the first time, registers validators with their controls. Call this method before validating.
  /// </summary>
  public void EnsureValidatableControlsInitialized ()
  {
    if (! _initialized)
    {
      InitializeValidatableControls (_control);
      _initialized = true;
    }
  }
}

}
