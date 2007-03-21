using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Interception.Castle
{
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

    /// <summary>
    /// Creates a new interceptable instance of a type.
    /// </summary>
    /// <param name="type">The type which the object must support.</param>
    /// <param name="args">The arguments to be passed to the object's constructor.</param>
    /// <returns>A new interceptable object instance.</returns>
    /// <remarks><para>This method does not directly instantiate the given <paramref name="type"/>, but instead dynamically creates a subclass proxy
    /// which overrides virtual methods in order to intercept method calls.</para>
    /// <para>The given <paramref name="type"/> must implement a constructor whose signature matches the arguments passed via <paramref name="args"/>.
    /// Avoid passing ambiguous argument arrays to this method. Or better: Avoid writing objects with such construcors.
    /// </para>
    ///</remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> or <paramref name="args"/> argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type"/> cannot be intercepted because it is sealed or abstract (apart from automatic
    /// properties) or it's not at least of type <typeparamref name="TInterceptorTarget"/>.</exception>
    /// <exception cref="MissingMethodException">The given <paramref name="type"/> does not implement a corresponding public or protected constructor.
    /// </exception>
    /// <exception cref="System.Reflection.TargetInvocationException">The constructor of the given <paramref name="type"/> threw an exception. See
    /// <see cref="Exception.InnerException"/>.</exception>
    // TODO: change selector handling as soon as DynamicProxy 2 implements it
    public object CreateInterceptableObject (Type type, object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("args", args);
      if (type.IsSealed)
      {
        throw new ArgumentException ("Cannot instantiate type " + type.FullName + " as it is sealed.");
      }

      ProxyGenerationOptions options = new ProxyGenerationOptions (_hook);
      try
      {
        return _generator.CreateClassProxy (type, _markerInterfaces, options, args, _mainInterceptor);
      }
      catch (NonInterceptableTypeException ex)
      {
        throw new ArgumentException (ex.Message, "type", ex);
      }
      catch (MissingMethodException ex)
      {
        throw new MissingMethodException ("Type " + type.FullName + " does not support the requested constructor with signature ("
            + ReflectionUtility.GetSignatureForArguments (args) + ").", ex);
      }
    }

    /// <summary>
    /// Checks whether the given object was created by a CastleInterceptableObjectGenerator instance.
    /// </summary>
    /// <param name="o">The object to be checked.</param>
    /// <returns>True if <paramref name="o"/> was created by this generator implementation, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="o"/> parameter was <see langword="null"/></exception>
    public bool WasCreatedByGenerator (object o)
    {
      return o is IProxyMarker;
    }
  }
}
