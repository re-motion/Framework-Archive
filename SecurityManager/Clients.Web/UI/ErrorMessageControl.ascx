<%@ Import namespace="System.Web.UI.WebControls"%>
<%@ Import namespace="Rubicon.SecurityManager.Clients.Web.UI"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ErrorMessageControl.ascx.cs" Inherits="ErrorMessageControl" %>
<div id="ErrorContainer">
  <asp:Label ID="ErrorsOnPageLabel" runat="server" Text="###" Visible="false" EnableViewState="false" />  
</div>
