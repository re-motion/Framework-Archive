using System;

namespace Rubicon.Security
{
  //TODO: SD: basisinterface fuer alle businessobjects mit security. hier umfangreichste ablaufdoku. eventuell bei den anderen nur hierher verweisen.
  public interface ISecurableObject
  {
  //objsecstrat fuer dieses object. verwendet von secclient
    IObjectSecurityStrategy GetSecurityStrategy ();
  }
}
