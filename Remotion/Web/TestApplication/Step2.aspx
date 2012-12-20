<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Step2.aspx.cs" Inherits="TestApplication.Step2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        step 2<br/>
        <asp:Label runat="server" ID="nameLabel"></asp:Label>

        <asp:Button runat="server" ID="submitButton" Text="update" />
        <asp:Button runat="server" ID="nextPageButton" Text="nextPage" OnClick="nextPageButton_OnClick" />
        <asp:Button runat="server" ID="executeAsyncSubFunction" Text="execute async sub function" OnClick="executeAsyncSubFunction_OnClick" />
    </div>
    </form>
</body>
</html>
