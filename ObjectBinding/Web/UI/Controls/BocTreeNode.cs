using System;
using System.Drawing.Design;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI;
using Rubicon.ObjectBinding.Web.Design;
using log4net;

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

  protected BocTreeView BocTreeView
  {
    get { return (BocTreeView) OwnerControl; }
  }
}

public class BusinessObjectTreeNode: BocTreeNode
{
	private static readonly ILog s_log = LogManager.GetLogger (typeof (BusinessObjectTreeNode));

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
    Property = property;
    if (_property != null)
      _propertyIdentifier = property.Identifier;
    BusinessObject = businessObject;
  }

  public BusinessObjectTreeNode (
      string nodeID, 
      string text, 
      IBusinessObjectReferenceProperty property,
      IBusinessObjectWithIdentity businessObject)
    : this (nodeID, text, null, property, businessObject)
  {
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectWithIdentity"/> of this <see cref="BusinessObjectTreeNode"/>.
  /// </summary>
  public IBusinessObjectWithIdentity BusinessObject
  {
    get 
    {
      EnsureBusinessObject();
      return _businessObject; 
    }
    set 
    {
      _businessObject = value; 

      if (s_log.IsDebugEnabled)
      {
        if (_businessObject == null && ! Text.EndsWith (" (null)"))
          Text += " (null)";
        else if (_businessObject != null && Text.EndsWith (" (null)"))
          Text = Text.Remove (Text.Length - 6, 6);
      }
    }
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectReferenceProperty"/> that was used to access the 
  ///   <see cref="IBusinessObjectWithIdentity"/> of this <see cref="BusinessObjectTreeNode"/>.
  /// </summary>
  public IBusinessObjectReferenceProperty Property
  {
    get 
    {
      EnsureProperty();
      return _property; 
    }
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

  private void EnsureBusinessObject()
  {
    if (_businessObject != null)
      return;

    //  Is root node?
    if (ParentNode == null)
    {
      if (BocTreeView.Value == null)
        throw new InvalidOperationException ("Cannot evaluate the tree node hierarchy because the value collection is null.");

      foreach (IBusinessObjectWithIdentity businessObject in BocTreeView.Value)
      {
        if (NodeID == businessObject.UniqueIdentifier)
        {
          BusinessObject = businessObject;
          break;
        }
      }

      if (_businessObject == null)
      {
        //  Required business object has not been part of the values collection in this post back, get it from the class
        if (BocTreeView.DataSource == null)
          throw new InvalidOperationException ("Cannot look-up IBusinessObjectWithIdentity '" + NodeID + "': DataSoure is null.");
        if (BocTreeView.DataSource.BusinessObjectClass == null)
          throw new InvalidOperationException ("Cannot look-up IBusinessObjectWithIdentity '" + NodeID + "': DataSource.BusinessObjectClass is null.");
        if (! (BocTreeView.DataSource.BusinessObjectClass is IBusinessObjectClassWithIdentity))
          throw new InvalidOperationException ("Cannot look-up IBusinessObjectWithIdentity '" + NodeID + "': DataSource.BusinessObjectClass is of type '" + BocTreeView.DataSource.BusinessObjectClass.GetType() + "' but must be of type IBusinessObjectClassWithIdentity.");
        
        BusinessObject = 
            ((IBusinessObjectClassWithIdentity) BocTreeView.DataSource.BusinessObjectClass).GetObject (NodeID);
        if (_businessObject == null) // This test could be omitted if graceful recovery is wanted.
          throw new InvalidOperationException ("Could not find IBusinessObjectWithIdentity '" + NodeID + "' via the DataSource.");
      }
    }
    else
    {
      IBusinessObjectReferenceProperty property = Property;
      string businessObjectID = NodeID;
      BusinessObject = ((IBusinessObjectClassWithIdentity) property.ReferenceClass).GetObject (businessObjectID);
    }
  }

  private void EnsureProperty()
  {
    if (_property != null)
      return;

    BusinessObjectTreeNode businessObjectParentNode = ParentNode as BusinessObjectTreeNode;
    BusinessObjectPropertyTreeNode propertyParentNode = ParentNode as BusinessObjectPropertyTreeNode;
    
    if (businessObjectParentNode != null)
    {
      IBusinessObjectProperty property = 
          businessObjectParentNode.BusinessObject.BusinessObjectClass.GetPropertyDefinition (_propertyIdentifier);
      Property = (IBusinessObjectReferenceProperty) property;

      if (_property == null) // This test could be omitted if graceful recovery is wanted.
        throw new InvalidOperationException ("Could not find IBusinessObjectReferenceProperty '" + _propertyIdentifier + "'.");
    }
    else if (propertyParentNode != null)
    {
      Property = propertyParentNode.Property;
      return;
    }
  }
}

public class BusinessObjectPropertyTreeNode: BocTreeNode
{
	private static readonly ILog s_log = LogManager.GetLogger (typeof (BusinessObjectPropertyTreeNode));
  IBusinessObjectReferenceProperty _property;

  public BusinessObjectPropertyTreeNode (
      string nodeID, 
      string text, 
      IconInfo icon, 
      IBusinessObjectReferenceProperty property)
    : base (nodeID, text, icon)
  {
    Property = property;
  }

  public BusinessObjectPropertyTreeNode (
      string nodeID, 
      string text, 
      IBusinessObjectReferenceProperty property)
    : this (nodeID, text, null, property)
  {
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectReferenceProperty"/> of this <see cref="BusinessObjectProeprtyTreeNode"/>.
  /// </summary>
  public IBusinessObjectReferenceProperty Property
  {
    get 
    {
      EnsureProperty();
      return _property; 
    }
    set 
    {
      _property = value; 

      if (s_log.IsDebugEnabled)
      {
        if (_property == null && ! Text.EndsWith (" (null)"))
          Text += " (null)";
        else if (_property != null && Text.EndsWith (" (null)"))
          Text = Text.Remove (Text.Length - 6, 6);
      }
    }
  }

  ///<summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "PropertyNode"; }
  }

  private void EnsureProperty()
  {
    if (_property != null)
      return;

    BusinessObjectTreeNode parentNode = (BusinessObjectTreeNode) ParentNode;
    if (parentNode == null)
      throw new InvalidOperationException ("BusinessObjectPropertyTreeNode with NodeID '" + NodeID + "' has no parent node but property nodes cannot be used as root nodes.");

    IBusinessObjectProperty property = parentNode.BusinessObject.BusinessObjectClass.GetPropertyDefinition (NodeID);
    Property = (IBusinessObjectReferenceProperty) property;
  }
}

}