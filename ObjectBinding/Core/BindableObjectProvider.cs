using System;
using System.Collections;

namespace Rubicon.ObjectBinding
{
  public class BindableObjectProvider : BusinessObjectProvider
  {
    private static BindableObjectProvider s_instance = new BindableObjectProvider();

    public static BindableObjectProvider Instance
    {
      get { return s_instance; }
    }

    public BindableObjectProvider ()
    {
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