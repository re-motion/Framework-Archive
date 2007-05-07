namespace Mixins.CodeGeneration.SingletonUtilities
{
  public interface IInstanceCreator<T>
  {
    T CreateInstance();
  }
}