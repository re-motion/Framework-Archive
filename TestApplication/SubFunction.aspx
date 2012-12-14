<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubFunction.aspx.cs" Inherits="TestApplication.SubFunction" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        this is a sub function <br />
        
        <asp:Label runat="server" ID="dateLabel"></asp:Label>

        <asp:Button runat="server" ID="updateButton" Text="update" OnClick="updateButton_OnClick" />
        <asp:Button runat="server" ID="exitSubFunctionButton" Text="exit sub funciton" OnClick="exitSubFunctionButton_OnClick" />

    </div>
    </form>
</body>
</html>
