<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Rubicon.SecurityManager.Clients.Web.Test._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <p>
    <a href="UserList.wxe?ClientID=Client|00000001-0000-0000-0000-000000000001|System.Guid">Aufbauorganisation verwalten</a>
    </p>
    <p>
    <a href="SecurableClassDefinitionList.wxe?ClientID=Client|00000001-0000-0000-0000-000000000001|System.Guid&WxeReturnToSelf=True&TabbedMenuSelection=AccessControlTab">Berechtigungen verwalten</a>
    </p>
    <p>
      <asp:Button ID="EvaluateSecurity" runat="server" Text="Evaluate Security" OnClick="EvaluateSecurity_Click" />
    </p>
    </form>
</body>
</html>
