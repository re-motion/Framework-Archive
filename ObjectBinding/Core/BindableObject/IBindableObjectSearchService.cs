namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface IBindableObjectSearchService : IBusinessObjectService
  {
    bool SupportsIdentity (IBusinessObjectReferenceProperty property);

    IBusinessObject[] Search (IBusinessObject referencingObject, IBusinessObjectReferenceProperty property, string searchStatement);
  }
}