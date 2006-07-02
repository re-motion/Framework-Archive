<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditPermissionControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>

<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.AccessControl.Permission, Rubicon.SecurityManager" />
<rubicon:BusinessObjectReferenceDataSourceControl ID="AccessType" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="AccessType" />
<rubicon:BocTextValue ID="NameField" runat="server" DataSourceControl="AccessType" PropertyIdentifier="DisplayName" ReadOnly="True" />
<rubicon:BocBooleanValue ID="AllowedField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BinaryAllowed" TrueDescription="Allowed" FalseDescription="Undefined" />