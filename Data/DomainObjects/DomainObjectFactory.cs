using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;
using Castle.Core.Interceptor;
using System.Diagnostics;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
  public static class DomainObjectFactory
  {
    class GenerationHook : IProxyGenerationHook
    {
      public void MethodsInspected ()
      {
        // nothing to do
      }

      public void NonVirtualMemberNotification (Type type, MemberInfo memberInfo)
      {
        // nothing to do
      }

      public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
      {
        if (memberInfo.IsAbstract) {
          // only allow abstract members for property accessors of automatic properties
          PropertyInfo property = ReflectionUtility.GetPropertyForMethod(memberInfo);
          if (property == null || !property.IsDefined (typeof (AutomaticPropertyAttribute), true))
          {
            throw new InvalidOperationException("Cannot instantiate type, the method " + type.FullName + "." + memberInfo.Name
                + " is abstract and not part of an automatic property.");
          }
        }
        return memberInfo.Name == "GetPublicDomainObjectType" || ReflectionUtility.IsPropertyAccessor (memberInfo);
      }
    }

    // TODO: Instead of MainInterceptor use IInterceptorSelector as soon as implemented by DynamicProxy 2
    class MainInterceptor : IInterceptor
    {
      private IInterceptor _propertyInterceptor;

      public MainInterceptor (IInterceptor propertyInterceptor)
      {
        _propertyInterceptor = propertyInterceptor;
      }

      public void Intercept (IInvocation invocation)
      {
        if (invocation.Method.Name == "GetPublicDomainObjectType")
        {
          invocation.ReturnValue = invocation.TargetType;
        }
        else
        {
          _propertyInterceptor.Intercept (invocation);
        }
      }
    }

    public interface IProxyMarker { }

    private readonly static ProxyGenerator _generator = new ProxyGenerator ();
    private readonly static GenerationHook _hook = new GenerationHook ();
    private readonly static MainInterceptor _interceptor = new MainInterceptor (new PropertyInterceptor ());
    private readonly static Type[] _markerInterfaces = new Type[] { typeof (IProxyMarker) };

    /// <summary>
    /// Creates a new instance of a domain object.
    /// </summary>
    /// <param name="type">The type which the object must support.</param>
    /// <param name="args">The arguments to be passed to the domain object's constructor.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks><para>This method does not directly instantiate the given <paramref name="type"/>, but instead dynamically creates a subclass that
    /// intercepts certain method calls in order to perform management tasks.</para>
    /// <para>This method ensures that the created domain object supports the new property syntax.</para>
    /// <para>The given <paramref name="type"/> must implement a constructor whose signature matches the arguments passed via <paramref name="args"/>.
    /// Avoid passing ambiguous argument arrays to this method. Or better: Avoid writing domain objects with such construcors.
    /// If you need to call a constructor with exactly one <c>null</c> argument, you can either pass <c>null</c> or <c>new object[] { null }</c>
    /// to this method. If you need to call a constructor which takes a single argument of array type, wrap it in a dedicated
    /// <c>object[]</c> (e.g. <c>new object[] { new int[] { 1, 2, 3 } }</c>).</para></remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type"/> argument is sealed, contains abstract methods (apart from automatic
    /// properties), or is not derived from <see cref="DomainObject"/>.</exception>
    internal static object Create (Type type, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!typeof (DomainObject).IsAssignableFrom (type))
      {
        throw new ArgumentException ("Cannot instantiate type " + type.FullName + " as it is not derived from DomainObject.");
      }
      if (type.IsSealed)
      {
        throw new ArgumentException ("Cannot instantiate type " + type.FullName + " as it is sealed.");
      }
      if (args == null)
      {
        args = new object[] { null };
      }

      ProxyGenerationOptions options = new ProxyGenerationOptions (_hook);
      try
      {
        return _generator.CreateClassProxy (type, _markerInterfaces, options, args, _interceptor);
      }
      catch (InvalidOperationException ex)
      {
        throw new ArgumentException (ex.Message, "type", ex);
      }
      catch (MissingMethodException ex)
      {
        throw new MissingMethodException ("Type " + type.FullName + " dows not support the requested constructor with signature ("
            + GetSignatureForArguments (args) + ").", ex);
      }
    }

    /// <summary>
    /// Creates a new instance of a domain object.
    /// </summary>
    /// <param name="type">The type which the object must support.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks><para>This method does not directly instantiate the given <paramref name="type"/>, but instead dynamically creates a subclass that
    /// intercepts certain method calls in order to perform management tasks.</para>
    /// <para>This method ensures that the created domain object supports the new property syntax.</para>
    /// <para>The given <paramref name="type"/> must implement a default constructor.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type"/> argument is sealed, contains abstract methods (apart from automatic
    /// properties), or is not derived from <see cref="DomainObject"/>.</exception>
    public static object Create (Type type)
    {
      return Create (type, new object[0]);
    }

    /// <summary>
    /// Creates a new instance of a domain object.
    /// </summary>
    /// <param name="type">The type which the object must support.</param>
    /// <param name="clientTransaction">The client transaction to be passed to the domain object's constructor.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks><para>This method does not directly instantiate the given <paramref name="type"/>, but instead dynamically creates a subclass that
    /// intercepts certain method calls in order to perform management tasks.</para>
    /// <para>This method ensures that the created domain object supports the new property syntax.</para>
    /// <para>The given <paramref name="type"/> must implement a constructor taking exactly one <see cref="ClientTransaction"/> argument.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> argument is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="type"/> argument is sealed, contains abstract methods (apart from automatic
    /// properties), or is not derived from <see cref="DomainObject"/>.</exception>
    public static object Create (Type type, ClientTransaction clientTransaction)
    {
      return Create (type, new object[] { clientTransaction });
    }

    /// <summary>
    /// Creates a new instance of a domain object.
    /// </summary>
    /// <typeparam name="T">The type which the object must support.</typeparam>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// <para>This method does not directly instantiate the given type <typeparamref name="T"/>, but instead dynamically creates a subclass that
    /// intercepts certain method calls in order to perform management tasks.</para>
    /// <para>This method ensures that the created domain object supports the new property syntax.</para>
    /// <para>The given <paramref name="type"/> must implement a default constructor. This is not enforced by a <c>new()</c> constraint
    /// in order to support abstract classes with automatic properties.</para>
    /// </remarks>
    /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is sealed or contains abstract methods (apart from automatic
    /// properties).</exception>
    public static T Create<T> () where T : DomainObject
    {
      try
      {
        return (T) Create (typeof (T));
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException (ex.Message, "T", ex);
      }
    }

    /// <summary>
    /// Creates a new instance of a domain object.
    /// </summary>
    /// <typeparam name="T">The type which the object must support.</typeparam>
    /// <param name="clientTransaction">The client transaction to be passed to the domain object's constructor.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// <para>This method does not directly instantiate the given type <typeparamref name="T"/>, but instead dynamically creates a subclass that
    /// intercepts certain method calls in order to perform management tasks.</para>
    /// <para>This method ensures that the created domain object supports the new property syntax.</para>
    /// <para>The given type <typeparamref name="T"/> must implement a constructor taking a single <see cref="ClientTransaction"/> argument.</para>
    /// </remarks>
    /// <exception cref="ArgumentException">The type <typeparamref name="T"/> is sealed or contains abstract methods (apart from automatic
    /// properties).</exception>
    public static T Create<T> (ClientTransaction clientTransaction) where T : DomainObject
    {
      try
      {
        return (T) Create (typeof (T), new object[] { clientTransaction });
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException (ex.Message, "T", ex);
      }
    }

    /// <summary>
    /// Checkes whether a given object instance was created by the factory or not.
    /// </summary>
    /// <param name="o">The object instance to be checked.</param>
    /// <returns>True if <paramref name="o"/> was created by the factory, else false.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="o"/> parameter was null.</exception>
    public static bool WasCreatedByFactory (object o)
    {
      ArgumentUtility.CheckNotNull ("o", o);
      return o is IProxyMarker;
    }

    private static string GetSignatureForArguments (object[] args)
    {
      Type[] argumentTypes = GetTypesForArgs (args);
      return ReflectionUtility.GetTypeListAsString (argumentTypes);
    }

    private static Type[] GetTypesForArgs (object[] args)
    {
      Type[] types = new Type[args.Length];
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i] == null)
        {
          types[i] = null;
        }
        else
        {
          types[i] = args[i].GetType ();
        }
      }
      return types;
    }
  }
}
