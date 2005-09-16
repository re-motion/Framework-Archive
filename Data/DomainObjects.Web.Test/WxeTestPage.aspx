<%@ Page language="c#" Codebehind="WxeTestPage.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.FirstPage" %>
<%@ Register TagPrefix="dow" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>FirstPage</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body >
	
    <form id="Form1" method="post" runat="server">
<dow:DomainObjectDataSourceControl id="ClassWithAllDataTypesDataSource" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Rubicon.Data.DomainObjects.Web.Test"></dow:DomainObjectDataSourceControl>
<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="300" border="1">
  <TR>
    <TD>StringProperty</TD>
    <TD>
<obw:BocTextValue id="StringValue" runat="server" DataSourceControl="ClassWithAllDataTypesDataSource" PropertyIdentifier="StringProperty">
<TextBoxStyle TextMode="SingleLine" MaxLength="100">
</TextBoxStyle>
</obw:BocTextValue></TD></TR>
  <TR>
    <TD>
      <P>EnumValues</P></TD>
    <TD>
<obw:BocEnumValue id="BocEnumValue1" runat="server" DataSourceControl="ClassForRelationTestDataSource" PropertyIdentifier="EnumProperty">
<ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding="">
</ListControlStyle>
</obw:BocEnumValue></TD></TR>
  <TR>
    <TD></TD>
    <TD></TD></TR></TABLE><BR>
<asp:Button id="Button1" runat="server" Text="Open in new Window with new ClientTransaction"></asp:Button><BR>
<asp:Button id="Button2" runat="server" Text="Open in new Window with same ClientTransaction"></asp:Button>

     </form>
	
  </body>
</HTML>
