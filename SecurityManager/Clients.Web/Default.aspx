<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.DefaultPage" %>

<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head runat="server">
  <title>Security Manager</title>
  <rubicon:HtmlHeadContents ID="HtmlHeadContents" runat="server">
  </rubicon:HtmlHeadContents>
</head>
<body>
  <form id="ThisForm" runat="server">
    <p>
      <a href="UserList.wxe">Aufbauorganisation verwalten</a>
    </p>
    <p>
      <a href="SecurableClassDefinitionList.wxe?WxeReturnToSelf=True&TabbedMenuSelection=AccessControlTab">Berechtigungen verwalten</a>
    </p>
    <p>
      <rubicon:BocReferenceValue runat="server" ID="UsersField" OnSelectionChanged="UsersField_SelectionChanged">
        <PersistedCommand>
          <rubicon:BocCommand />
        </PersistedCommand>
        <DropDownListStyle AutoPostBack="True" />
      </rubicon:BocReferenceValue>
    </p>
  </form>
</body>
</html>
