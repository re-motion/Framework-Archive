<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.DefaultPage" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html>
<head runat="server">
    <title>Security Manager</title>
    <rubicon:HtmlHeadContents id="HtmlHeadContents" runat="server"></rubicon:HtmlHeadContents>
</head>
<body>
  <form id="ThisForm" runat="server">
    <p>
    <a href="UserList.wxe">Aufbauorganisation verwalten</a>
    </p>
    <p>
    <a href="SecurableClassDefinitionList.wxe?WxeReturnToSelf=True&TabbedMenuSelection=AccessControlTab">Berechtigungen verwalten</a>
    </p>
  </form>
</body>
</html>
