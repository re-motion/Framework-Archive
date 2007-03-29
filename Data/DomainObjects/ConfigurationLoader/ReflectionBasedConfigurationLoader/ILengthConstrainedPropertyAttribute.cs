namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public interface ILengthConstrainedPropertyAttribute: IMappingAttribute
  {
    int? MaximumLength { get;}
  }
}