<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SecurableClassDefinitionListForm.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.UI.AccessControl.SecurableClassDefinitionListForm" MasterPageFile="../SecurityManagerMasterPage.Master" %>
<%@ Register TagPrefix="securityManager" Src="SecurableClassDefinitionListControl.ascx" TagName="SecurableClassDefinitionListControl" %>
<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl id="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <securityManager:SecurableClassDefinitionListControl ID="SecurableClassDefinitionListControl" runat="server" />
</asp:Content>
