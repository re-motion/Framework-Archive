// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Web;
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes.AccessControl
{
  /// <summary>
  /// The <see cref="CollapsedAccessControlConditionsRenderer"/> type is responsible for generating the collapsed view on an <see cref="AccessControlEntry"/>
  /// </summary>
  public class CollapsedAccessControlConditionsRenderer
  {
    private readonly AccessControlEntry _accessControlEntry;
    private readonly IServiceLocator _serviceLocator;

    public CollapsedAccessControlConditionsRenderer (AccessControlEntry accessControlEntry, IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull ("currentAccessControlEntry", accessControlEntry);
      ArgumentUtility.CheckNotNull ("serviceLocator", serviceLocator);

      _accessControlEntry = accessControlEntry;
      _serviceLocator = serviceLocator;
    }

    public AccessControlEntry AccessControlEntry
    {
      get { return _accessControlEntry; }
    }

    public void RenderTenant (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.TenantCondition)
      {
        case TenantCondition.None:
          writer.Write (HttpUtility.HtmlEncode (AccessControlResources.TenantCondition_None));
          break;
        case TenantCondition.OwningTenant:
          RenderTenantHierarchyIcon (writer, container);
          RenderPropertyPathString (writer, "TenantCondition");
          break;
        case TenantCondition.SpecificTenant:
          RenderTenantHierarchyIcon (writer, container);
          RenderLabelAfterPropertyPathString (writer, "SpecificTenant.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderGroup (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.GroupCondition)
      {
        case GroupCondition.None:
          writer.Write (HttpUtility.HtmlEncode (AccessControlResources.GroupCondition_None));
          break;
        case GroupCondition.OwningGroup:
          RenderGroupHierarchyIcon (writer, container);
          RenderPropertyPathString (writer, "GroupCondition");
          break;
        case GroupCondition.SpecificGroup:
          RenderGroupHierarchyIcon (writer, container);
          RenderLabelAfterPropertyPathString (writer, "SpecificGroup.ShortName");
          break;
        case GroupCondition.BranchOfOwningGroup:
          RenderLabelBeforePropertyPathString (writer, AccessControlResources.BranchOfOwningGroupLabel, "SpecificGroupType.DisplayName");
          break;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          RenderLabelAfterPropertyPathString (writer, "SpecificGroupType.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderUser (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      switch (_accessControlEntry.UserCondition)
      {
        case UserCondition.None:
          writer.Write (HttpUtility.HtmlEncode (AccessControlResources.UserCondition_None));
          break;
        case UserCondition.Owner:
          RenderPropertyPathString (writer, "UserCondition");
          break;
        case UserCondition.SpecificUser:
          RenderLabelAfterPropertyPathString (writer, "SpecificUser.DisplayName");
          break;
        case UserCondition.SpecificPosition:
          RenderLabelAfterPropertyPathString (writer, "SpecificPosition.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderAbstractRole (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("container", container);

      RenderLabelBeforePropertyPathString (writer, string.Empty, "SpecificAbstractRole.DisplayName");
    }

    private void RenderLabelAfterPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      writer.RenderBeginTag (HtmlTextWriterTag.Em);
      RenderPropertyPathString (writer, propertyPathIdentifier);
      writer.RenderEndTag();
      writer.Write (" (");
      string label = GetPropertyDisplayName (propertyPathIdentifier);
      writer.Write (HttpUtility.HtmlEncode (label));
      writer.Write (")");
    }

    private void RenderLabelBeforePropertyPathString (HtmlTextWriter writer, string label, string propertyPathIdentifier)
    {
      writer.Write (HttpUtility.HtmlEncode (label));
      writer.Write (" ");
      writer.RenderBeginTag (HtmlTextWriterTag.Em);
      RenderPropertyPathString (writer, propertyPathIdentifier);
      writer.RenderEndTag();
    }

    private void RenderPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.Parse (businessObject.BusinessObjectClass, propertyPathIdentifier);
      writer.Write (HttpUtility.HtmlEncode (propertyPath.GetString (businessObject, null)));
    }

    private string GetPropertyDisplayName (string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.Parse (businessObject.BusinessObjectClass, propertyPathIdentifier);
      Assertion.IsTrue (propertyPath.Properties.Length >= 2);
      return propertyPath.Properties[propertyPath.Properties.Length - 2].DisplayName;
    }

    private void RenderTenantHierarchyIcon (HtmlTextWriter writer, IControl container)
    {
      string url;
      string text;
      switch (_accessControlEntry.TenantHierarchyCondition)
      {
        case TenantHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.This:
          url = "HierarchyThis.gif";
          text = AccessControlResources.TenantHierarchyCondition_This;
          break;
        case TenantHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          text = AccessControlResources.TenantHierarchyCondition_ThisAndParent;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url, container)) { AlternateText = text };
      icon.Render (writer);
    }

    private void RenderGroupHierarchyIcon (HtmlTextWriter writer, IControl container)
    {
      string url;
      string text;
      switch (_accessControlEntry.GroupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.This:
          url = "HierarchyThis.gif";
          text = AccessControlResources.GroupHierarchyCondition_This;
          break;
        case GroupHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.Children:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.ThisAndParent:
          url = "HierarchyThisAndParent.gif";
          text = AccessControlResources.GroupHierarchyCondition_ThisAndParent;
          break;
        case GroupHierarchyCondition.ThisAndChildren:
          url = "HierarchyThisAndChildren.gif";
          text = AccessControlResources.GroupHierarchyCondition_ThisAndChildren;
          break;
        case GroupHierarchyCondition.ThisAndParentAndChildren:
          url = "HierarchyThisAndParentAndChildren.gif";
          text = AccessControlResources.GroupHierarchyCondition_ThisAndParentAndChildren;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo (GetIconUrl (url, container)) { AlternateText = text };
      icon.Render (writer);
    }

    private string GetIconUrl (string url, IControl container)
    {
      return ResourceUrlResolver.GetResourceUrl (container, typeof (CollapsedAccessControlConditionsRenderer), ResourceType.Image, ResourceTheme, url);
    }
  
    protected ResourceTheme ResourceTheme
    {
      get { return _serviceLocator.GetInstance<ResourceTheme> (); }
    }
  }
}