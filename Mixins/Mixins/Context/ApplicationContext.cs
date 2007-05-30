using System;
using System.Collections.Generic;
using Mixins.CodeGeneration.SingletonUtilities;
using Rubicon.Utilities;

namespace Mixins.Context
{
  /// <summary>
  /// The set of class contexts active for the current thread.
  /// </summary>
  public class ApplicationContext
  {
    private Dictionary<Type, ClassContext> _classContexts;

    /// <summary>
    /// Creates a new application context that does not inherit anything from another context.
    /// </summary>
    public ApplicationContext ()
    {
      _classContexts = new Dictionary<Type, ClassContext> ();
    }

    /// <summary>
    /// Creates a new application context that inherits from another context.
    /// </summary>
    /// <param name="parentContext">The parent context. The new context will inherit all class contexts from its parent context.</param>
    public ApplicationContext (ApplicationContext parentContext)
    {
      _classContexts = new Dictionary<Type, ClassContext> (parentContext._classContexts);
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

      if (HasClassContext (classContext.Type))
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
      return HasClassContext (type) ? _classContexts[type] : null;
    }

    /// <summary>
    /// Checks whether the <see cref="ApplicationContext"/> holds a <see cref="ClassContext"/> for the given <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check for.</param>
    /// <returns>True if the <see cref="ApplicationContext"/> holds a <see cref="ClassContext"/> for the given <see cref="Type"/>; false otherwise.
    /// </returns>
    public bool HasClassContext (Type type)
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
      if (!HasClassContext (type))
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
  }
}
