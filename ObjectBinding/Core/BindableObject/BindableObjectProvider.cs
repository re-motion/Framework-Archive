using System;
using System.Collections;
using Rubicon.Collections;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectProvider : BusinessObjectProvider
  {
    private static BindableObjectProvider s_instance = new BindableObjectProvider();

    public static BindableObjectProvider Instance
    {
      get { return s_instance; }
    }

    private readonly InterlockedCache<Type, BindableObjectClass> _businessObjectClassCache = new InterlockedCache<Type, BindableObjectClass>();

    public BindableObjectProvider ()
    {
    }

    public InterlockedCache<Type, BindableObjectClass> BusinessObjectClassCache
    {
      get { return _businessObjectClassCache; }
    }

    /// <summary> The <see cref="IDictionary"/> used to store the references to the registered servies. </summary>
    /// <remarks>
    ///   <note type="inotes">
    ///    If your object model does not support services, this property may always return <see langword="null"/>.
    ///   </note>
    /// </remarks>
    protected override IDictionary ServiceDictionary
    {
      get { throw new NotImplementedException (); }
    }
  }
}