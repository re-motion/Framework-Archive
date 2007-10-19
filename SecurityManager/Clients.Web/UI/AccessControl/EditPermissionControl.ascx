<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditPermissionControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>

<rubicon:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Rubicon.SecurityManager.Domain.AccessControl.Permission, Rubicon.SecurityManager" />
<rubicon:BusinessObjectReferenceDataSourceControl ID="AccessType" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="AccessType" />
<rubicon:BocBooleanValue ID="AllowedField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BinaryAllowed" ShowDescription="False" Width="1em" />
<rubicon:BocTextValue ID="NameField" runat="server" DataSourceControl="AccessType" PropertyIdentifier="DisplayName" ReadOnly="True" />
