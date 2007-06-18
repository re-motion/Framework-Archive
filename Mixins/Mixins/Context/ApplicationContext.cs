using System;
using System.Collections.Generic;
using Mixins.Definitions;
using Mixins.Utilities.Singleton;
using Rubicon.Utilities;
using Mixins.Validation;

namespace Mixins.Context
{
  /// <summary>
  /// The set of class contexts active for the current thread.
  /// </summary>
  public class ApplicationContext
  {
    private Dictionary<Type, ClassContext> _classContexts;
    private Dictionary<Type, ClassContext> _registeredInterfaces = new Dictionary<Type,ClassContext> ();

    /// <summary>
    /// Initializes a new empty application context that does not inherit anything from another context.
    /// </summary>
    public ApplicationContext ()
        : this (null)
    {
    }

    /// <summary>
    /// Initializes a new application context that inherits from another context.
    /// </summary>
    /// <param name="parentContext">The parent context. The new context will inherit all class contexts from its parent context. Can be
    /// <see langword="null"/>.</param>
    public ApplicationContext (ApplicationContext parentContext)
    {
      _classContexts = new Dictionary<Type, ClassContext>();
      if (parentContext != null)
        parentContext.CopyTo (this);
    }

    /// <summary>
    /// Gets the class context count for this <see cref="ApplicationContext"/>.
    /// </summary>
    /// <value>The number of class contexts currently stored in this <see cref="ApplicationContext"/>.</value>
    public int ClassContextCount
    {
      get { return _classContexts.Count; }
    }

    /// <summary>
    /// Gets the class contexts currently stored in this <see cref="ApplicationContext"/>.
    /// </summary>
    /// <value>The class contexts currently sotred in this application context.</value>
    public IEnumerable<ClassContext> ClassContexts
    {
      get { return _classContexts.Values; }
    }

    /// <summary>
    /// Adds a new class context to the <see cref="ApplicationContext"/>.
    /// </summary>
    /// <param name="classContext">The class context to add.</param>
    /// <exception cref="InvalidOperationException">The <see cref="ApplicationContext"/> already contains a <see cref="ClassContext"/> for the
    /// same <see cref="Type"/>.</exception>"
    public void AddClassContext (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull ("classContext", classContext);

      if (ContainsClassContext (classContext.Type))
      {
        string message = string.Format ("There is already a class context for type {0}.", classContext.Type.FullName);
        throw new InvalidOperationException (message);
      }
      _classContexts.Add (classContext.Type, classContext);
    }

    /// <summary>
    /// Retrives the class context for a given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to retrieve a class context for.</param>
    /// <returns>The <see cref="ClassContext"/> stored by the <see cref="ApplicationContext"/> for the given <see cref="Type"/>, or
    /// <see langword="null"/> if no such context exists.</returns>
    public ClassContext GetClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return ContainsClassContext (type) ? _classContexts[type] : null;
    }

    /// <summary>
    /// Checks whether the <see cref="ApplicationContext"/> holds a <see cref="ClassContext"/> for the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check for.</param>
    /// <returns>True if the <see cref="ApplicationContext"/> holds a <see cref="ClassContext"/> for the given <see cref="Type"/>; false otherwise.
    /// </returns>
    public bool ContainsClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _classContexts.ContainsKey (type);
    }

    /// <summary>
    /// Retrieves a <see cref="ClassContext"/> for the given <see cref="Type"/>, or creates and adds a new one if none exists.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which a <see cref="ClassContext"/> is to be retrieved.</param>
    /// <returns>A <see cref="ClassContext"/> for the given <see cref="Type"/>.</returns>
    public ClassContext GetOrAddClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!ContainsClassContext (type))
        AddClassContext (new ClassContext (type));
      return GetClassContext (type);
    }

    /// <summary>
    /// Removes a class context from the <see cref="ApplicationContext"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> whose class context is to be removed.</param>
    /// <returns>True if the <see cref="ApplicationContext"/> contained a respective class context; false otherwise.</returns>
    public bool RemoveClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ClassContext context = GetClassContext (type);
      if (context != null)
      {
        List<Type> interfacesToRemove = new List<Type>();
        foreach (Type registeredInterface in _registeredInterfaces.Keys)
        {
          if (object.ReferenceEquals (ResolveInterface (registeredInterface), context))
            interfacesToRemove.Add (registeredInterface);
        }
        foreach (Type interfaceToRemove in interfacesToRemove)
          _registeredInterfaces.Remove (interfaceToRemove);
      }
      return _classContexts.Remove (type);
    }

    /// <summary>
    /// Adds a class context, replacing the existing one if any.
    /// </summary>
    /// <param name="newContext">The class context to add to this <see cref="ApplicationContext"/>.</param>
    public void AddOrReplaceClassContext (ClassContext newContext)
    {
      ArgumentUtility.CheckNotNull ("newContext", newContext);

      RemoveClassContext (newContext.Type);
      AddClassContext (newContext);
    }

    /// <summary>
    /// Validates the whole application context.
    /// </summary>
    /// <returns>An <see cref="IValidationLog"/>, which contains information about whether the configuration reresented by this context is valid.</returns>
    /// <remarks>This method retrieves definition items for all the <see cref="ClassContexts"/> known by this application context and uses the
    /// <see cref="Validator"/> class to validate them. The validation results can be inspected, passed to a <see cref="ValidationException"/>, or
    /// be dumped using the <see cref="ConsoleDumper"/>.</remarks>
    public IValidationLog Validate()
    {
      List<IVisitableDefinition> definitions = new List<IVisitableDefinition>();
      foreach (ClassContext classContext in MixinConfiguration.ActiveContext.ClassContexts)
        definitions.Add (TypeFactory.GetActiveConfiguration (classContext.Type));
      return Validator.Validate (definitions);
    }

    /// <summary>
    /// Registers an interface to be associated with the given <see cref="ClassContext"/>. Later calls to <see cref="ResolveInterface"/>
    /// with the given interface type will result in the registeres context being returned.
    /// </summary>
    /// <param name="interfaceType">Type of the interface to be registered.</param>
    /// <param name="associatedClassContext">The class context to be associated with the interface type.</param>
    /// <exception cref="InvalidOperationException">The interface has already been registered.</exception>
    /// <exception cref="ArgumentNullException">One of the parameters is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface or
    /// <paramref name="associatedClassContext"/> has not been added to this application context.</exception>
    public void RegisterInterface (Type interfaceType, ClassContext associatedClassContext)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      ArgumentUtility.CheckNotNull ("associatedClassContext", associatedClassContext);

      if (!interfaceType.IsInterface)
        throw new ArgumentException ("The argument is not an interface.", "interfaceType");

      if (!_classContexts.ContainsKey (associatedClassContext.Type)
          || !object.ReferenceEquals (_classContexts[associatedClassContext.Type], associatedClassContext))
        throw new ArgumentException ("The class context hasn't been added to this application context.", "associatedClassContext");

      if (_registeredInterfaces.ContainsKey (interfaceType))
      {
        string message = string.Format ("The interface {0} has already been associated with a class context.", interfaceType.FullName);
        throw new InvalidOperationException (message);
      }

      _registeredInterfaces.Add (interfaceType, associatedClassContext);
    }

    public void RegisterInterface (Type interfaceType, Type associatedClassType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      ArgumentUtility.CheckNotNull ("associatedClassType", associatedClassType);

      if (!ContainsClassContext (associatedClassType))
      {
        string message = string.Format ("There is no class context for the given type {0}.", associatedClassType.FullName);
        throw new ArgumentException (message, "associatedClassType");
      }
      else
        RegisterInterface (interfaceType, GetClassContext (associatedClassType));
    }


    /// <summary>
    /// Resolves the given interface into a class context.
    /// </summary>
    /// <param name="interfaceType">The interface type to be resolved.</param>
    /// <returns>The <see cref="ClassContext"/> previously registered for the given type, or <see langword="null"/> if the no context was registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <paramref name="interfaceType"/> argument is not an interface.</exception>
    public ClassContext ResolveInterface (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);

      if (!interfaceType.IsInterface)
        throw new ArgumentException ("The argument is not an interface.", "interfaceType");

      if (_registeredInterfaces.ContainsKey (interfaceType))
        return _registeredInterfaces[interfaceType];
      else
        return null;
    }

    /// <summary>
    /// Copies all configuration data of this application context to a destination context, replacing class contexts and registered interfaces
    /// for types that are configured in both contexts.
    /// </summary>
    /// <param name="destination">The destination to copy all configuration data to..</param>
    public void CopyTo (ApplicationContext destination)
    {
      ArgumentUtility.CheckNotNull ("destination", destination);

      foreach (ClassContext classContext in ClassContexts)
        destination.AddOrReplaceClassContext (classContext.Clone());

      foreach (KeyValuePair<Type, ClassContext> interfaceRegistration in _registeredInterfaces)
      {
        if (destination._registeredInterfaces.ContainsKey (interfaceRegistration.Key))
          destination._registeredInterfaces.Remove (interfaceRegistration.Key);
        destination.RegisterInterface (interfaceRegistration.Key, interfaceRegistration.Value.Type);
      }
    }
  }
}
