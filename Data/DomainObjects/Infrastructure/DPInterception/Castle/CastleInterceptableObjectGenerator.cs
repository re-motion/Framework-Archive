using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Castle.DynamicProxy;
using Rubicon.Collections;
using Rubicon.Reflection;
using Rubicon.Utilities;
using CastleInterceptor = Castle.Core.Interceptor.IInterceptor;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception.Castle
{
  using CacheKey = Tuple<Type, Type>; // defining type, delegate type

  /// <summary>
  /// Marker interface indicating that a type was constructed by DynamicProxy.
  /// </summary>
  public interface IProxyMarker { } // must not be generic for current version of DP 2

  internal class CastleInterceptableObjectGenerator<TTarget> : IInterceptableObjectGenerator<TTarget>
  {
    private static ICache<CacheKey, Delegate> s_delegateCache = new InterlockedCache<CacheKey, Delegate> ();

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

    public IFuncInvoker<TMinimal> MakeTypesafeConstructorInvoker<TMinimal> (Type concreteType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteType", concreteType, typeof (TMinimal));

      return new FuncInvoker<CastleInterceptor[], TMinimal> (
          delegate (Type delegateType)
          {
            CacheKey key = new CacheKey (concreteType, delegateType);
            Delegate result;
            if (! s_delegateCache.TryGetValue (key, out result))
            {
              result = s_delegateCache.GetOrCreateValue (
                  key,
                  delegate
                  {
                    if (!WasCreatedByGenerator (concreteType))
                    {
                      string message = string.Format ("Type {0} is not an interceptable type created by this kind of generator.",
                        concreteType.FullName);
                      throw new ArgumentException (message);
                    }

                    Type[] argumentTypes = ConstructorWrapper.GetParameterTypes (delegateType);
                    BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    try
                    {
                      return ConstructorWrapper.CreateDelegate (
                          concreteType, delegateType, bindingFlags, null, CallingConventions.Any, argumentTypes, null);
                    }
                    catch (MissingMethodException ex)
                    {
                      Type[] realArgumentTypes = new Type[argumentTypes.Length - 1];
                      Array.Copy (argumentTypes, 1, realArgumentTypes, 0, realArgumentTypes.Length);
                      string message = string.Format ("Type {0} does not support the requested constructor with signature ({1}).",
                          concreteType.BaseType.FullName, 
                          ReflectionUtility.GetTypeListAsString (realArgumentTypes));
                      throw new MissingMethodException (message, ex);
                    }
                  });
            }
            return result;
          },
          CreateInterceptorArray ());
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
      Assertion.IsNotNull(field, "DynamicProxy 2 __interceptors field must exist");
      field.SetValue (instance, CreateInterceptorArray());
    }
  }
}
