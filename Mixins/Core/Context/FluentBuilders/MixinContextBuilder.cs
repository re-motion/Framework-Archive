using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Context.FluentBuilders
{
  public class MixinContextBuilder
  {
    private readonly ClassContextBuilder _parent;
    private readonly Type _mixinType;
    private readonly List<Type> _dependencies = new List<Type>();

    public MixinContextBuilder (ClassContextBuilder parent, Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("parent", parent);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _parent = parent;
      _mixinType = mixinType;
    }

    public ClassContextBuilder Parent
    {
      get { return _parent; }
    }

    public Type MixinType
    {
      get { return _mixinType; }
    }

    public IEnumerable<Type> Dependencies
    {
      get { return _dependencies; }
    }

    public virtual MixinContextBuilder WithDependency (Type requiredMixin)
    {
      ArgumentUtility.CheckNotNull ("requiredMixin", requiredMixin);
      _dependencies.Add (requiredMixin);
      return this;
    }

    public virtual MixinContextBuilder WithDependency<TRequiredMixin> ()
    {
      return WithDependency (typeof (TRequiredMixin));
    }

    public virtual MixinContextBuilder WithDependencies (params Type[] requiredMixins)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("requiredMixins", requiredMixins);
      foreach (Type requiredMixin in requiredMixins)
        WithDependency (requiredMixin);
      return this;
    }

    public virtual MixinContextBuilder WithDependencies<TMixin1, TMixin2> ()
    {
      return WithDependencies (typeof (TMixin1), typeof (TMixin2));
    }

    public virtual MixinContextBuilder WithDependencies<TMixin1, TMixin2, TMixin3> ()
    {
      return WithDependencies (typeof (TMixin1), typeof (TMixin2), typeof (TMixin3));
    }

    public virtual MixinContext BuildMixinContext (ClassContext classContext)
    {
      MixinContext mixinContext = classContext.AddMixin (_mixinType);
      foreach (Type dependency in _dependencies)
        mixinContext.AddExplicitDependency (dependency);
      return mixinContext;
    }

    #region Parent members

    public virtual MixinContextBuilder AddMixin (Type mixinType)
    {
      return _parent.AddMixin (mixinType);
    }

    public virtual MixinContextBuilder AddMixin<TMixin> ()
    {
      return _parent.AddMixin<TMixin>();
    }

    public virtual ClassContextBuilder AddMixins (params Type[] mixinTypes)
    {
      return _parent.AddMixins (mixinTypes);
    }

    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2> ()
    {
      return _parent.AddMixins<TMixin1, TMixin2> ();
    }

    public virtual ClassContextBuilder AddMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.AddMixins<TMixin1, TMixin2, TMixin3>();
    }

    public virtual ClassContextBuilder AddOrderedMixins (params Type[] mixinTypes)
    {
      return _parent.AddOrderedMixins (mixinTypes);
    }

    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2> ()
    {
      return _parent.AddOrderedMixins<TMixin1, TMixin2> ();
    }

    public virtual ClassContextBuilder AddOrderedMixins<TMixin1, TMixin2, TMixin3> ()
    {
      return _parent.AddOrderedMixins<TMixin1, TMixin2, TMixin3> ();
    }

    public virtual ClassContextBuilder AddCompleteInterface (Type interfaceType)
    {
      return _parent.AddCompleteInterface (interfaceType);
    }

    public virtual ClassContextBuilder AddCompleteInterface<TInterface> ()
    {
      return _parent.AddCompleteInterface<TInterface> ();
    }

    public virtual ClassContextBuilder AddCompleteInterfaces (params Type[] interfaceTypes)
    {
      return _parent.AddCompleteInterfaces (interfaceTypes);
    }

    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2> ()
    {
      return _parent.AddCompleteInterfaces<TInterface1, TInterface2> ();
    }

    public virtual ClassContextBuilder AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ()
    {
      return _parent.AddCompleteInterfaces<TInterface1, TInterface2, TInterface3> ();
    }

    public virtual ClassContextBuilder InheritFrom (Type targetTypeToInheritFrom)
    {
      return _parent.InheritFrom (targetTypeToInheritFrom);
    }

    public virtual ClassContext BuildClassContext (MixinConfiguration mixinConfiguration)
    {
      return _parent.BuildClassContext (mixinConfiguration);
    }

    public virtual ClassContextBuilder ForClass (Type targetType)
    {
      return _parent.ForClass (targetType);
    }

    public virtual ClassContextBuilder ForClass<TTargetType> ()
    {
      return _parent.ForClass<TTargetType> ();
    }

    public virtual MixinConfiguration BuildConfiguration ()
    {
      return _parent.BuildConfiguration ();
    }

    public virtual IDisposable EnterScope ()
    {
      return _parent.EnterScope ();
    }
    #endregion
  }
}