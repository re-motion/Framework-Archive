using System;
using Rubicon.ObjectBinding;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web
{

/// <summary>
///   Provides services for business object bound web applications
/// </summary>
public interface IBusinessObjectWebUIService: IBusinessObjectService
{
  IconInfo GetIcon (IBusinessObject obj);
}

}
