using System;
using System.Web.UI.WebControls;
using Rubicon.ObjectBinding.Web.Controls;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Reflection;
using Rubicon.Web.UI.Controls;

namespace OBRTest
{

public class PersonTreeView: BocTreeView
{
	public PersonTreeView()
	{
	}

  protected override BusinessObjectPropertyTreeNodeInfo[] GetPropertyNodes (IBusinessObjectWithIdentity businessObject)
  { 
    BusinessObjectPropertyTreeNodeInfo[] nodeInfos;
    if (businessObject is Person)
    {
      nodeInfos = new BusinessObjectPropertyTreeNodeInfo[2];
      nodeInfos[0] = new BusinessObjectPropertyTreeNodeInfo (
          "Children", 
          new IconInfo(null, Unit.Empty, Unit.Empty), 
          new ReflectionBusinessObjectReferenceProperty (
              typeof (Person).GetProperty ("Children"), typeof (Person), true));
      nodeInfos[1] = new BusinessObjectPropertyTreeNodeInfo (
          "Jobs", 
          new IconInfo(null, Unit.Empty, Unit.Empty), 
          new ReflectionBusinessObjectReferenceProperty (
              typeof (Person).GetProperty ("Jobs"), typeof (Job), true));
    }
    else
    {
      nodeInfos = new BusinessObjectPropertyTreeNodeInfo[0];
    }

    return nodeInfos;                                
  }

}

}
