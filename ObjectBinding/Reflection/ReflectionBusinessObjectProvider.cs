using System;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectProvider: IBusinessObjectProvider
{
  public static ReflectionBusinessObjectProvider s_instance = new ReflectionBusinessObjectProvider();

  public static ReflectionBusinessObjectProvider Instance 
  {
    get { return s_instance; }
  }

  public IBusinessObjectService GetService (Type serviceType)
  {
    return null;
  }
}

}
