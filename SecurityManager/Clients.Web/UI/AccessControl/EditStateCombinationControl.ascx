<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStateCombinationControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditStateCombinationControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>

<rubicon:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Rubicon.SecurityManager.Domain.AccessControl.StateCombination, Rubicon.SecurityManager" />
<rubicon:BocReferenceValue id="StateDefinitionField" runat="server" />