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
  public BocTreeNode (string nodeID, string text, IconInfo icon)
    : base (nodeID, text, icon)
  {
  }

  [Browsable (false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public override WebTreeNodeCollection Children
  {
    get { return base.Children; }
  }
}

public class BusinessObjectTreeNode: BocTreeNode
{
  IBusinessObjectWithIdentity _businessObject;
  IBusinessObjectReferenceProperty _property;
  string _propertyIdentifier;

  public BusinessObjectTreeNode (
      string nodeID, 
      string text, 
      IconInfo icon, 
      IBusinessObjectReferenceProperty property,
      IBusinessObjectWithIdentity businessObject)
    : base (nodeID, text, icon)
  {
    _property = property;
    if (_property != null)
      _propertyIdentifier = property.Identifier;
    _businessObject = businessObject;
  }

  public BusinessObjectTreeNode (
      string nodeID, 
      string text, 
      IBusinessObjectReferenceProperty property,
      IBusinessObjectWithIdentity businessObject)
    : this (nodeID, text, null, property, businessObject)
  {
  }

  public IBusinessObjectWithIdentity BusinessObject
  {
    get { return _businessObject; }
    set { _businessObject = value; }
  }

  public IBusinessObjectReferenceProperty Property
  {
    get { return _property; }
    set 
    { 
      _property = value; 
      if (value != null)
        _propertyIdentifier = value.Identifier;
      else
        _propertyIdentifier = string.Empty;
    }
  }

  public string PropertyIdentifier
  {
    get { return _propertyIdentifier; }
    set
    {
      _propertyIdentifier = value; 
      _property = null;
    }
  }

  ///<summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "ObjectNode"; }
  }
}

public class BusinessObjectPropertyTreeNode: BocTreeNode
{
  IBusinessObjectReferenceProperty _property;

  public BusinessObjectPropertyTreeNode (
      string nodeID, 
      string text, 
      IconInfo icon, 
      IBusinessObjectReferenceProperty property)
    : base (nodeID, text, icon)
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