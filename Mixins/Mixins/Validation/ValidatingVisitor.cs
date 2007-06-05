using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Rubicon.Utilities;

namespace Mixins.Validation
{
  public class ValidatingVisitor : IDefinitionVisitor
  {
    private IValidationLog _validationLog;

    private List<IValidationRule<BaseClassDefinition>> _baseClassRules = new List<IValidationRule<BaseClassDefinition>> ();
    private List<IValidationRule<MixinDefinition>> _mixinRules = new List<IValidationRule<MixinDefinition>> ();
    private List<IValidationRule<InterfaceIntroductionDefinition>> _interfaceIntroductionRules = new List<IValidationRule<InterfaceIntroductionDefinition>> ();
    private List<IValidationRule<MethodIntroductionDefinition>> _methodIntroductionRules = new List<IValidationRule<MethodIntroductionDefinition>> ();
    private List<IValidationRule<PropertyIntroductionDefinition>> _propertyIntroductionRules = new List<IValidationRule<PropertyIntroductionDefinition>> ();
    private List<IValidationRule<EventIntroductionDefinition>> _eventIntroductionRules = new List<IValidationRule<EventIntroductionDefinition>> ();
    private List<IValidationRule<MethodDefinition>> _methodRules = new List<IValidationRule<MethodDefinition>> ();
    private List<IValidationRule<PropertyDefinition>> _propertyRules = new List<IValidationRule<PropertyDefinition>> ();
    private List<IValidationRule<EventDefinition>> _eventRules = new List<IValidationRule<EventDefinition>> ();
    private List<IValidationRule<RequiredFaceTypeDefinition>> _requiredFaceTypeRules = new List<IValidationRule<RequiredFaceTypeDefinition>> ();
    private List<IValidationRule<RequiredBaseCallTypeDefinition>> _requiredBaseCallTypeRules = new List<IValidationRule<RequiredBaseCallTypeDefinition>> ();
    private List<IValidationRule<ThisDependencyDefinition>> _thisDependencyRules = new List<IValidationRule<ThisDependencyDefinition>> ();
    private List<IValidationRule<BaseDependencyDefinition>> _baseDependencyRules = new List<IValidationRule<BaseDependencyDefinition>> ();
    private List<IValidationRule<AttributeDefinition>> _attributeRules = new List<IValidationRule<AttributeDefinition>>();
    private List<IValidationRule<RequiredBaseCallMethodDefinition>> _requiredBaseCallMethodRules = new List<IValidationRule<RequiredBaseCallMethodDefinition>> ();

    public ValidatingVisitor(IValidationLog validationLog)
    {
      ArgumentUtility.CheckNotNull ("validationLog", validationLog);
      _validationLog = validationLog;
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

    public IList<IValidationRule<MethodIntroductionDefinition>> MethodIntroductionRules
    {
      get { return _methodIntroductionRules; }
    }

    public IList<IValidationRule<PropertyIntroductionDefinition>> PropertyIntroductionRules
    {
      get { return _propertyIntroductionRules; }
    }

    public IList<IValidationRule<EventIntroductionDefinition>> EventIntroductionRules
    {
      get { return _eventIntroductionRules; }
    }

    public IList<IValidationRule<MethodDefinition>> MethodRules
    {
      get { return _methodRules; }
    }

    public IList<IValidationRule<PropertyDefinition>> PropertyRules
    {
      get { return _propertyRules; }
    }

    public IList<IValidationRule<EventDefinition>> EventRules
    {
      get { return _eventRules; }
    }

    public IList<IValidationRule<RequiredFaceTypeDefinition>> RequiredFaceTypeRules
    {
      get { return _requiredFaceTypeRules; }
    }

    public IList<IValidationRule<RequiredBaseCallTypeDefinition>> RequiredBaseCallTypeRules
    {
      get { return _requiredBaseCallTypeRules; }
    }

    public IList<IValidationRule<RequiredBaseCallMethodDefinition>> RequiredBaseCallMethodRules
    {
      get { return _requiredBaseCallMethodRules; }
    }

    public IList<IValidationRule<ThisDependencyDefinition>> ThisDependencyRules
    {
      get { return _thisDependencyRules; }
    }

    public IList<IValidationRule<BaseDependencyDefinition>> BaseDependencyRules
    {
      get { return _baseDependencyRules; }
    }

    public IList<IValidationRule<AttributeDefinition>> AttributeRules
    {
      get { return _attributeRules; }
    }


    public void Visit (BaseClassDefinition baseClass)
    {
      ArgumentUtility.CheckNotNull ("baseClass", baseClass);
      CheckRules (_baseClassRules, baseClass);
    }

    public void Visit (MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      CheckRules (_mixinRules, mixin);
    }

    public void Visit (InterfaceIntroductionDefinition interfaceIntroduction)
    {
      ArgumentUtility.CheckNotNull ("interfaceIntroduction", interfaceIntroduction);
      CheckRules (_interfaceIntroductionRules, interfaceIntroduction);
    }

    public void Visit (MethodIntroductionDefinition methodIntroduction)
    {
      ArgumentUtility.CheckNotNull ("methodIntroduction", methodIntroduction);
      CheckRules (_methodIntroductionRules, methodIntroduction);
    }

    public void Visit (PropertyIntroductionDefinition propertyIntroduction)
    {
      ArgumentUtility.CheckNotNull ("propertyIntroduction", propertyIntroduction);
      CheckRules (_propertyIntroductionRules, propertyIntroduction);
    }

    public void Visit (EventIntroductionDefinition eventIntroduction)
    {
      ArgumentUtility.CheckNotNull ("eventIntroduction", eventIntroduction);
      CheckRules (_eventIntroductionRules, eventIntroduction);
    }

    public void Visit (MethodDefinition method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      CheckRules (_methodRules, method);
    }

    public void Visit (PropertyDefinition property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      CheckRules (_propertyRules, property);
    }

    public void Visit (EventDefinition eventDefinition)
    {
      ArgumentUtility.CheckNotNull ("event", eventDefinition);
      CheckRules (_eventRules, eventDefinition);
    }

    public void Visit (RequiredFaceTypeDefinition requiredFaceType)
    {
      ArgumentUtility.CheckNotNull ("requiredFaceType", requiredFaceType);
      CheckRules (_requiredFaceTypeRules, requiredFaceType);
    }

    public void Visit (RequiredBaseCallTypeDefinition requiredBaseCallType)
    {
      ArgumentUtility.CheckNotNull ("requiredBaseCallType", requiredBaseCallType);
      CheckRules (_requiredBaseCallTypeRules, requiredBaseCallType);
    }

    public void Visit (RequiredBaseCallMethodDefinition requiredBaseCallMethod)
    {
      ArgumentUtility.CheckNotNull ("requiredBaseCallMethod", requiredBaseCallMethod);
      CheckRules (_requiredBaseCallMethodRules, requiredBaseCallMethod);
    }

    public void Visit (ThisDependencyDefinition dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      CheckRules (_thisDependencyRules, dependency);
    }

    public void Visit (BaseDependencyDefinition dependency)
    {
      ArgumentUtility.CheckNotNull ("dependency", dependency);
      CheckRules (_baseDependencyRules, dependency);
    }

    public void Visit (AttributeDefinition attribute)
    {
      ArgumentUtility.CheckNotNull ("attribute", attribute);
      CheckRules (_attributeRules, attribute);
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
