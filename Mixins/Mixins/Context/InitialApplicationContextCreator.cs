using System;
using Mixins.CodeGeneration.SingletonUtilities;
using System.Reflection;
namespace Mixins.Context
{
  public class InitialApplicationContextCreator : IInstanceCreator<ApplicationContext>
  {
    public ApplicationContext CreateInstance ()
    {
      return new ApplicationContext(); // TODO: use AssemblyFinder for the initial context
    }
  }
}