using System;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.Web
{
	/// <summary>
	///   Provides services for business object bound web applications
	/// </summary>
	public interface IBusinessObjectWebUIService: IBusinessObjectService
	{
    string GetIconPath (IBusinessObjectWithIdentity obj);
	}
}
