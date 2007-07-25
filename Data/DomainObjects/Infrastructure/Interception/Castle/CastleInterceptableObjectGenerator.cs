using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy;
using Rubicon.Reflection;
using Rubicon.Utilities;
using CastleInterceptor = Castle.Core.Interceptor.IInterceptor;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception.Castle
{
  /// <summary>
  /// Marker interface indicating that a type was constructed by DynamicProxy.
  /// </summary>
  public interface IProxyMarker { } // must not be generic for current version of DP 2

  class CastleInterceptableObjectGenerator<TTarget> : IInterceptableObjectGenerator<TTarget>
  {
    private readonly Type[] _markerInterfaces = new Type[] { typeof (IProxyMarker) };

    private readonly ProxyGenerator _generator = new ProxyGenerator ();

    private MainInterceptor<TTarget> _mainInterceptor;
    private IInterceptorSelector<TTarget> _selector;
    private GenerationHook<TTarget> _hook;

    public CastleInterceptableObjectGenerator(IInterceptorSelector<TTarget> selector)
    {
      _selector = selector;
      _hook = new GenerationHook<TTarget>(selector);
      _mainInterceptor = new MainInterceptor<TTarget> (_selector);
    }

    public IInterceptorSelector<TTarget> Selector
    {
      get { return _selector; }
      set
      {
        _selector = value;
        _hook = new GenerationHook<TTarget>(_selector);
        _mainInterceptor = new MainInterceptor<TTarget> (_selector);
      }
    }

    // TODO: change selector handling as soon as DynamicProxy 2 implements it
    public Type GetInterceptableType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("type", baseType);
      if (baseType.IsSealed)
      {
        throw new ArgumentException ("Cannot subclass type " + baseType.FullName + " as it is sealed.");
      }

      ProxyGenerationOptions options = new ProxyGenerationOptions (_hook);
      try
      {
        return _generator.ProxyBuilder.CreateClassProxy (baseType, _markerInterfaces, options);
      }
      catch (NonInterceptableTypeException ex)
      {
        throw new ArgumentException (ex.Message, "type", ex);
      }
    }

    public bool WasCreatedByGenerator (Type type)
    {
      return typeof (IProxyMarker).IsAssignableFrom (type);
    }

    public IInvokeWith<TMinimal> MakeTypesafeConstructorInvoker<TMinimal> (Type concreteType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteType", concreteType, typeof (TMinimal));

      if (!WasCreatedByGenerator (concreteType))
      {
        string message = string.Format ("Type {0} is not an interceptable type created by this kind of generator.",
          concreteType.FullName);
        throw new ArgumentException (message);
      }

      BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      GetDelegateWith<TMinimal> constructionDelegateCreator = new CachedGetDelegateWith<TMinimal,Type> (
          concreteType,
          delegate (Type[] argumentTypes, Type delegateType)
          {
            try
            {
              return ConstructorWrapper.CreateConstructorDelegate (concreteType, bindingFlags, null, CallingConventions.Any, argumentTypes, null, delegateType);
            }
            catch (MissingMethodException ex)
            {
              Type[] realArgumentTypes = new Type[argumentTypes.Length - 1];
              Array.Copy(argumentTypes, 1, realArgumentTypes, 0, realArgumentTypes.Length);
              string message = string.Format ("Type {0} does not support the requested constructor with signature ({1}).",
                concreteType.BaseType.FullName, ReflectionUtility.GetTypeListAsString (realArgumentTypes));
              throw new MissingMethodException (message, ex);
            }
          });
        
      return new InvokeWithBoundFirst<TMinimal, CastleInterceptor[]> (constructionDelegateCreator, CreateInterceptorArray());
    }

    private CastleInterceptor[] CreateInterceptorArray()
    {
      return new CastleInterceptor[] { _mainInterceptor };
    }

    public void PrepareUnconstructedInstance (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      Type instanceType = ((object)instance).GetType ();
      if (!WasCreatedByGenerator (instanceType))
      {
        string message = string.Format ("Type {0} is not an interceptable type created by this kind of generator.",
          instanceType.FullName);
        throw new ArgumentException (message);
      }

      FieldInfo field = instanceType.GetField ("__interceptors");
      Assertion.Assert (field != null, "DynamicProxy 2 __interceptors field must exist");
      field.SetValue (instance, CreateInterceptorArray());
    }
  }
}
