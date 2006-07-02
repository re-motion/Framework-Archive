<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStateCombinationControl.ascx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.EditStateCombinationControl" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>

<rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.AccessControl.StateCombination, Rubicon.SecurityManager" />
<rubicon:BocReferenceValue id="StateDefinitionField" runat="server" />