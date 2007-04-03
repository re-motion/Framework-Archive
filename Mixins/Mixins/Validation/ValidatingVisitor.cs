using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation
{
  public delegate void ValidationRule <TDefinition> (TDefinition definition, IValidationLog log);

  public class ValidatingVisitor : IDefinitionVisitor
  {
    private IValidationLog _validationLog;

    private List<ValidationRule<ApplicationDefinition>> _applicationRules = new List<ValidationRule<ApplicationDefinition>> ();
    private List<ValidationRule<BaseClassDefinition>> _baseClassRules = new List<ValidationRule<BaseClassDefinition>> ();
    private List<ValidationRule<MixinDefinition>> _mixinRules = new List<ValidationRule<MixinDefinition>> ();
    private List<ValidationRule<InterfaceIntroductionDefinition>> _interfaceIntroductionRules = new List<ValidationRule<InterfaceIntroductionDefinition>> ();
    private List<ValidationRule<MethodDefinition>> _methodRules = new List<ValidationRule<MethodDefinition>> ();
    private List<ValidationRule<RequiredFaceTypeDefinition>> _requiredFaceTypeRules = new List<ValidationRule<RequiredFaceTypeDefinition>> ();
    private List<ValidationRule<RequiredBaseCallTypeDefinition>> _requiredBaseCallTypeRules = new List<ValidationRule<RequiredBaseCallTypeDefinition>> ();

    public ValidatingVisitor(IValidationLog validationLog)
    {
      _validationLog = validationLog;
    }

    public IList<ValidationRule<ApplicationDefinition>> ApplicationRules
    {
      get { return _applicationRules; }
    }

    public IList<ValidationRule<BaseClassDefinition>> BaseClassRules
    {
      get { return _baseClassRules; }
    }

    public IList<ValidationRule<MixinDefinition>> MixinRules
    {
      get { return _mixinRules; }
    }

    public IList<ValidationRule<InterfaceIntroductionDefinition>> InterfaceIntroductionRules
    {
      get { return _interfaceIntroductionRules; }
    }

    public IList<ValidationRule<MethodDefinition>> MethodRules
    {
      get { return _methodRules; }
    }

    public IList<ValidationRule<RequiredFaceTypeDefinition>> RequiredFaceTypeRules
    {
      get { return _requiredFaceTypeRules; }
    }

    public IList<ValidationRule<RequiredBaseCallTypeDefinition>> RequiredBaseCallTypeRules
    {
      get { return _requiredBaseCallTypeRules; }
    }

    public void Visit (ApplicationDefinition application)
    {
      CheckRules (_applicationRules, application);
    }

    public void Visit (BaseClassDefinition baseClass)
    {
      CheckRules (_baseClassRules, baseClass);
    }

    public void Visit (MixinDefinition mixin)
    {
      CheckRules (_mixinRules, mixin);
    }

    public void Visit (InterfaceIntroductionDefinition interfaceIntroduction)
    {
      CheckRules (_interfaceIntroductionRules, interfaceIntroduction);
    }

    public void Visit (MethodDefinition method)
    {
      CheckRules (_methodRules, method);
    }

    public void Visit (RequiredFaceTypeDefinition requiredFaceType)
    {
      CheckRules (_requiredFaceTypeRules, requiredFaceType);
    }

    public void Visit (RequiredBaseCallTypeDefinition requiredBaseCallType)
    {
      CheckRules (_requiredBaseCallTypeRules, requiredBaseCallType);
    }

    private void CheckRules<TDefinition> (List<ValidationRule<TDefinition>> rules, TDefinition definition) where TDefinition : IVisitableDefinition
    {
      _validationLog.ValidationStartsFor (definition);
      foreach (ValidationRule<TDefinition> rule in rules)
      {
        try
        {
          rule (definition, _validationLog);
        }
        catch (Exception ex)
        {
          _validationLog.UnexpectedException (ex);
        }
      }
      _validationLog.ValidationEndsFor (definition);
    }
  }
}
