<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BindableObjectTest.aspx.cs" Inherits="OBWTest.BindableObjectTest" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="TheForm" runat="server">
    <rubicon:BocTextValue ID="FirstNameField" runat="server" DataSourceControl="CurrentObject" />
    <rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" />
    </form>
</body>
</html>
