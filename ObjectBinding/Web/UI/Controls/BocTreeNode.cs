using System;
using System.Drawing.Design;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding.Web.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

public abstract class BocTreeNode: WebTreeNode
{
  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public override WebTreeNodeCollection Children
  {
    get { return base.Children; }
  }
}

public class BusinessObjectTreeNode: WebTreeNode
{
  IBusinessObjectWithIdentity _businessObject;

  public BusinessObjectTreeNode (
      string nodeID, 
      string text, 
      IconInfo icon, 
      IBusinessObjectWithIdentity businessObject)
    : base (nodeID, text, icon.Url)
  {
    _businessObject = businessObject;
  }

  public BusinessObjectTreeNode (
      string nodeID, 
      string text, 
      IBusinessObjectWithIdentity businessObject)
    : this (nodeID, text, null, businessObject)
  {
  }

  public IBusinessObjectWithIdentity BusinessObject
  {
    get { return _businessObject; }
    set { _businessObject = value; }
  }

  ///<summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "ObjectNode"; }
  }
}

public class BusinessObjectPropertyTreeNode: WebTreeNode
{
  IBusinessObjectReferenceProperty _property;

  public BusinessObjectPropertyTreeNode (
      string nodeID, 
      string text, 
      IconInfo icon, 
      IBusinessObjectReferenceProperty property)
    : base (nodeID, text, icon.Url)
  {
    _property = property;
  }

  public BusinessObjectPropertyTreeNode (
      string nodeID, 
      string text, 
      IBusinessObjectReferenceProperty property)
    : this (nodeID, text, null, property)
  {
  }

  public IBusinessObjectReferenceProperty Property
  {
    get { return _property; }
    set { _property = value; }
  }

  ///<summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "PropertyNode"; }
  }
}

}