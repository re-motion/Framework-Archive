<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Step2.aspx.cs" Inherits="TestApplication.Step2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        name: 
        <asp:TextBox runat="server" ID="nameTextBox"></asp:TextBox>
    
        <asp:Panel runat="server" ID="displayNamePanel" Visible="False">
            your name is <asp:Label runat="server" ID="nameLabel"></asp:Label>
        </asp:Panel>

        <asp:Button runat="server" ID="nextPageButton" Text="nextPage" OnClick="nextPageButton_OnClick" />
        <asp:Button runat="server" ID="submitButton" Text="submit" />
    </div>
    </form>
</body>
</html>
