<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="TestApplication._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <a href="WxeHandler.ashx?WxeFunctionType=TestApplication.TestFunction,TestApplication">execute function</a>
        <a href="WxeHandler.ashx?WxeFunctionType=TestApplication.TestAsyncFunction,TestApplication">execute async function</a>
    </div>
    </form>
</body>
</html>

