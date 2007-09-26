<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SecurableClassDefinitionListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.SecurableClassDefinitionListForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Rubicon.SecurityManager.Clients.Web" Namespace="Rubicon.SecurityManager.Clients.Web.Classes" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl ID="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <rubicon:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="Rubicon.SecurityManager.Domain.Metadata.SecurableClassDefinition, Rubicon.SecurityManager" />
  <securityManager:SecurableClassDefinitionTreeView ID="SecurableClassDefinitionTree" runat="server" DataSourceControl="CurrentObject" EnableLookAheadEvaluation="True" EnableTopLevelExpander="False" OnClick="SecurableClassDefinitionTree_Click" PropertyIdentifier="DerivedClasses" />
</asp:Content>
