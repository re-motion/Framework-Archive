using System;
namespace Rubicon.Security
{
  [Obsolete ("Use IObjectSecurityAdapter instead. (Version: 1.7.41)", true)]
  public interface IObjectSecurityProvider : IObjectSecurityAdapter, ISecurityProviderObsolete
  {
  }

  //TODO: SD: definiert adapter für security layer. registiert in secadapterregistry, 
  //verwendet in implementierung von ibusinessobjectproperty um sewcurity abfragen zu tun.
  //geht ueblicherweise auf secclient der weiterleitet an objsectrategy (geliefert von isecobj), die fuer die abfrage zustaendig ist.
  public interface IObjectSecurityAdapter : ISecurityAdapter
  {
    //verwendet fuer sichtbarkeit, secObj = isntanz fur die sec gecheckt wird.
    bool HasAccessOnGetAccessor (ISecurableObject securableObject, string propertyName);
    //verwendet fuer read-only
    bool HasAccessOnSetAccessor (ISecurableObject securableObject, string propertyName);
  }
}
