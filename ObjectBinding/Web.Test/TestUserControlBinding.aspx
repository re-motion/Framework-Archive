<%@ Page language="c#" Codebehind="TestUserControlBinding.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestUserControlBinding" %>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <meta name="vs_showGrid" content="False">
    <title>TestUserControlBinding</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <rubicon:HtmlHeadContents id="HtmlHeadContents" runat="server"></rubicon:HtmlHeadContents>
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <div visible="false" runat="server" style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; FONT-SIZE: x-small; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid; FONT-FAMILY: Verdana, Arial, Sans-Serif; BACKGROUND-COLOR: #ffffe0">
        <rubicon:FormGridManager id="FormGridManager" runat="server"></rubicon:FormGridManager>
        <rubicon:BindableObjectDataSourceControl id="DataSource" runat="server" TypeName="OBWTest.Person" />
      </div>
      <TABLE id="NameFormGrid" runat="server" style="WIDTH: 100%">
        <TR>
          <TD>
            <asp:Label id="Label1" runat="server">Personendaten</asp:Label></TD>
          <TD></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <rubicon:BocTextValue id="BocTextValue1" runat="server"></rubicon:BocTextValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD></TD>
        </TR>
      </TABLE>
      <P>
        <rubicon:UserControlBinding id="UserControlBinding1" runat="server" UserControlPath="address.ascx" DataSourceControl="DataSource"
          PropertyIdentifier="Partner"></rubicon:UserControlBinding></P>
    </form>
  </body>
</HTML>
