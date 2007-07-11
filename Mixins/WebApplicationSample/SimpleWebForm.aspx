<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimpleWebForm.aspx.cs" Inherits="WebApplicationSample.SimpleWebForm" %>

<%@ Register Src="DynamicallyLoadedControl.ascx" TagName="DynamicallyLoadedControl"
  TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SimpleWebForm</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <label id="greeting" runat="server">Greetings: <%= this.GetGreetings() %></label>
    </div>
    
    <asp:PlaceHolder id="placeholder" runat="server">
    </asp:PlaceHolder>
    </form>
</body>
</html>
