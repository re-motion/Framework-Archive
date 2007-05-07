namespace Mixins.CodeGeneration.SingletonUtilities
{
  public class DefaultInstanceCreator<T> : IInstanceCreator<T> where T : new()
  {
    public T CreateInstance()
    {
      return new T();
    }
  }
}