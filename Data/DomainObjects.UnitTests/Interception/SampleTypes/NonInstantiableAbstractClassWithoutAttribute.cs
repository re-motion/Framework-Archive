namespace Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public abstract class NonInstantiableAbstractClassWithoutAttribute : DomainObject
  {
    protected NonInstantiableAbstractClassWithoutAttribute ()
    {
    }
  }
}