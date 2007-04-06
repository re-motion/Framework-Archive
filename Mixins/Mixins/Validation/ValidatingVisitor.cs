using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;

namespace Mixins.Validation
{
  public class ValidatingVisitor : IDefinitionVisitor
  {
    private IValidationLog _validationLog;

    private List<IValidationRule<ApplicationDefinition>> _applicationRules = new List<IValidationRule<ApplicationDefinition>> ();
    private List<IValidationRule<BaseClassDefinition>> _baseClassRules = new List<IValidationRule<BaseClassDefinition>> ();
    private List<IValidationRule<MixinDefinition>> _mixinRules = new List<IValidationRule<MixinDefinition>> ();
    private List<IValidationRule<InterfaceIntroductionDefinition>> _interfaceIntroductionRules = new List<IValidationRule<InterfaceIntroductionDefinition>> ();
    private List<IValidationRule<MethodDefinition>> _methodRules = new List<IValidationRule<MethodDefinition>> ();
    private List<IValidationRule<RequiredFaceTypeDefinition>> _requiredFaceTypeRules = new List<IValidationRule<RequiredFaceTypeDefinition>> ();
    private List<IValidationRule<RequiredBaseCallTypeDefinition>> _requiredBaseCallTypeRules = new List<IValidationRule<RequiredBaseCallTypeDefinition>> ();
    private List<IValidationRule<ThisDependencyDefinition>> _thisDependencyRules = new List<IValidationRule<ThisDependencyDefinition>> ();
    private List<IValidationRule<BaseDependencyDefinition>> _baseDependencyRules = new List<IValidationRule<BaseDependencyDefinition>> ();

    public ValidatingVisitor(IValidationLog validationLog)
    {
      _validationLog = validationLog;
    }

    public IList<IValidationRule<ApplicationDefinition>> ApplicationRules
    {
      get { return _applicationRules; }
    }

    public IList<IValidationRule<BaseClassDefinition>> BaseClassRules
    {
      get { return _baseClassRules; }
    }

    public IList<IValidationRule<MixinDefinition>> MixinRules
    {
      get { return _mixinRules; }
    }

    public IList<IValidationRule<InterfaceIntroductionDefinition>> InterfaceIntroductionRules
    {
      get { return _interfaceIntroductionRules; }
    }

    public IList<IValidationRule<MethodDefinition>> MethodRules
    {
      get { return _methodRules; }
    }

    public IList<IValidationRule<RequiredFaceTypeDefinition>> RequiredFaceTypeRules
    {
      get { return _requiredFaceTypeRules; }
    }

    public IList<IValidationRule<RequiredBaseCallTypeDefinition>> RequiredBaseCallTypeRules
    {
      get { return _requiredBaseCallTypeRules; }
    }

    public IList<IValidationRule<ThisDependencyDefinition>> ThisDependencyRules
    {
      get { return _thisDependencyRules; }
    }

    public IList<IValidationRule<BaseDependencyDefinition>> BaseDependencyRules
    {
      get { return _baseDependencyRules; }
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

    public void Visit (ThisDependencyDefinition dependency)
    {
      CheckRules (_thisDependencyRules, dependency);
    }

    public void Visit (BaseDependencyDefinition dependency)
    {
      CheckRules (_baseDependencyRules, dependency);
    }

    private void CheckRules<TDefinition> (List<IValidationRule<TDefinition>> rules, TDefinition definition) where TDefinition : IVisitableDefinition
    {
      _validationLog.ValidationStartsFor (definition);
      foreach (IValidationRule<TDefinition> rule in rules)
      {
        try
        {
          rule.Execute (this, definition, _validationLog);
        }
        catch (Exception ex)
        {
          _validationLog.UnexpectedException (rule, ex);
        }
      }
      _validationLog.ValidationEndsFor (definition);
    }
  }
}
