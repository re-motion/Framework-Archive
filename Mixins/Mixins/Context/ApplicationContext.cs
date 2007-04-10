using System;
using System.Collections.Generic;
using Rubicon.Utilities;

namespace Mixins.Context
{
  /// <summary>
  /// A set of class contexts to be fed into a definition builder.
  /// </summary>
  public class ApplicationContext
  {
    private Dictionary<Type, ClassContext> _classContexts = new Dictionary<Type, ClassContext> ();

    public IEnumerable<ClassContext> ClassContexts
    {
      get { return _classContexts.Values; }
    }

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

    public ClassContext GetClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return HasClassContext (type)? _classContexts[type] : null;
    }

    public bool HasClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _classContexts.ContainsKey (type);
    }

    public ClassContext GetOrAddClassContext (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!HasClassContext (type))
      {
        AddClassContext (new ClassContext (type));
      }
      return GetClassContext (type);
    }
  }
}
