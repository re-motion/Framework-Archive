using System;
using System.Collections;
using Rubicon.Collections;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The default implementation of BusinessObjectProvider for the DomainObjects framework. This class is a Singleton.
  /// </summary>
  /// <remarks>Retrieve the reference to the object using the <see cref="Instance"/> property.</remarks>
  public class DomainObjectProvider : BusinessObjectProvider
  {
    private static readonly DomainObjectProvider s_instance = new DomainObjectProvider();

    /// <summary>
    /// Returns the single <see cref="DomainObjectProvider"/> instance.
    /// </summary>
    public static DomainObjectProvider Instance
    {
      get { return s_instance; }
    }

    private DomainObjectProvider ()
    {
    }

    private readonly InterlockedCache<Type, IBusinessObjectService> _serviceCache = new InterlockedCache<Type, IBusinessObjectService>();

    /// <summary> Gets the <see cref="IDictionary"/> used to store the references to the registered servies. </summary>
    protected override ICache<Type, IBusinessObjectService> ServiceCache
    {
      get { return _serviceCache; }
    }
  }
}