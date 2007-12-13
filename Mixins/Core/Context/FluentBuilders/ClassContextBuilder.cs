using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.FluentBuilders
{
  public class ClassContextBuilder
  {
    private readonly MixinConfigurationBuilder _parent;
    private readonly Type _targetType;
    private readonly List<MixinContextBuilder> _mixinContextBuilders = new List<MixinContextBuilder> ();
    private readonly List<Type> _completeInterfaces = new List<Type> ();
    private readonly List<ClassContext> _typesToInheritFrom = new List<ClassContext> ();

    private ClassContext _parentContext;

    public ClassContextBuilder (MixinConfigurationBuilder parent, Type targetType, ClassContext parentContext)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      _parent = parent;
      _targetType = targetType;
      _parentContext = parentContext;
    }

    public MixinConfigurationBuilder Parent
    {
      get { return _parent; }
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public ClassContext ParentContext
    {
      get { return _parentContext; }
    }

    public IEnumerable<MixinContextBuilder> MixinContextBuilders
    {
      get { return _mixinContextBuilders; }
    }

    public IEnumerable<Type> CompleteInterfaces
    {
      get { return _completeInterfaces; }
    }

    public IEnumerable<ClassContext> TypesToInheritFrom
    {
      get { return _typesToInheritFrom; }
    }

    public virtual ClassContextBuilder IgnoreParent ()
    {
      _parentContext = null;
      return this;
    }

    public virtual MixinContextBuilder AddMixin (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      MixinContextBuilder mixinContextBuilder = new MixinContextBuilder (this, mixinType);
      _mixinContextBuilders.Add (mixinContextBuilder);
      return mixinContextBuilder;
    }

    public virtual MixinContextBuilder AddMixin<TMixin> ()
    {
      return AddMixin (typeof (TMixin));
    }

    public virtual ClassContextBuilder AddMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      foreach (Type mixinType in mixinTypes)
        AddMixin (mixinType);
      return this;
    }

    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2> ()
    {
      return AddMixins (typeof (TMixin1), typeof (TMixin2));
    }

    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return AddMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    public virtual ClassContextBuilder AddOrderedMixins (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      Type lastMixinType = null;
      foreach (Type mixinType in mixinTypes)
      {
        MixinContextBuilder mixinContextBuilder = AddMixin (mixinType);
        if (lastMixinType != null)
          mixinContextBuilder.WithDependency (lastMixinType);
        lastMixinType = mixinType;
      }
      return this;
    }

    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> ()
    {
      return AddOrderedMixins (typeof (TMixin1), typeof (TMixin2));
    }

    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return AddOrderedMixins (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    public virtual ClassContextBuilder AddCompleteInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      _completeInterfaces.Add (interfaceType);
      return this;
    }

    public virtual ClassContextBuilder AddCompleteInterface<TInterface> ()
    {
      return AddCompleteInterface (typeof (TInterface));
    }

    public virtual ClassContextBuilder AddCompleteInterfaces (params Type[] interfaceTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("interfaceTypes", interfaceTypes);
      foreach (Type interfaceType in interfaceTypes)
        AddCompleteInterface (interfaceType);
      return this;
    }

    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2> ()
    {
      return AddCompleteInterfaces (typeof (TInterface1), typeof (TInterface2));
    }

    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ()
    {
      return AddCompleteInterfaces (typeof (TInterface1), typeof (TInterface2), typeof (TInterface3));
    }

    public virtual ClassContextBuilder InheritFrom (ClassContext contextOfTargetTypeToInheritFrom)
    {
      ArgumentUtility.CheckNotNull ("contextOfTargetTypeToInheritFrom", contextOfTargetTypeToInheritFrom);
      _typesToInheritFrom.Add (contextOfTargetTypeToInheritFrom);
      return this;
    }

    public virtual ClassContextBuilder InheritFrom (Type targetTypeToInheritFrom)
    {
      ArgumentUtility.CheckNotNull ("targetTypeToInheritFrom", targetTypeToInheritFrom);
      ClassContext contextForType = Parent.ParentConfiguration.GetClassContext (targetTypeToInheritFrom);
      if (contextForType != null)
        return InheritFrom (contextForType);
      else
        return this;
    }

    public virtual ClassContextBuilder InheritFrom<TTypeToInheritFrom> ()
    {
      return InheritFrom (typeof (TTypeToInheritFrom));
    }

    public virtual ClassContext BuildClassContext (MixinConfiguration mixinConfiguration)
    {
      ClassContext classContext;
      if (ParentContext != null)
        classContext = ParentContext.CloneForSpecificType (_targetType);
      else
        classContext = new ClassContext (_targetType);

      foreach (MixinContextBuilder mixinContextBuilder in _mixinContextBuilders)
        mixinContextBuilder.BuildMixinContext (classContext);
      foreach (Type completeInterface in _completeInterfaces)
        classContext.AddCompleteInterface (completeInterface);
      foreach (ClassContext typeToInheritFrom in _typesToInheritFrom)
        classContext.InheritFrom (typeToInheritFrom);
      mixinConfiguration.AddClassContext (classContext);
      return classContext;
    }

    #region Parent members

    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      return _parent.ForClass (targetType);
    }

    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return _parent.ForClass<TTargetType>();
    }

    public virtual MixinConfiguration BuildConfiguration ()
    {
      return _parent.BuildConfiguration();
    }

    public virtual IDisposable EnterScope ()
    {
      return _parent.EnterScope();
    }
    #endregion
  }
}