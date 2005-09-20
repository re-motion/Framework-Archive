<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="dow" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="WxeTestPage.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.FirstPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>FirstPage</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:HtmlHeadContents>
  </HEAD>
<body>
<form id=Form1 method=post 
runat="server"><dow:domainobjectdatasourcecontrol 
id=ClassWithAllDataTypesDataSource runat="server" 
TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Rubicon.Data.DomainObjects.Web.Test"></dow:domainobjectdatasourcecontrol>
<TABLE id=Table1 cellSpacing=1 cellPadding=1 width=300 border=1>
  <TR>
    <TD>StringProperty</TD>
    <TD><obw:boctextvalue id=StringValue runat="server" PropertyIdentifier="StringProperty" DataSourceControl="ClassWithAllDataTypesDataSource">
<TextBoxStyle TextMode="SingleLine" MaxLength="100">
</TextBoxStyle>
</obw:boctextvalue></TD></TR>
  <TR>
    <TD>
      <P>EnumValues</P></TD>
    <TD><obw:bocenumvalue id=BocEnumValue1 runat="server" PropertyIdentifier="EnumProperty" DataSourceControl="ClassWithAllDataTypesDataSource">
<ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding="">
</ListControlStyle>
</obw:bocenumvalue></TD></TR>
  <TR>
    <TD></TD>
    <TD></TD></TR></TABLE><BR><asp:button id=OpenWithNewClientTransactionButton runat="server" Text="Open in new Window with new ClientTransaction"></asp:button><BR><asp:button id=OpenWithSameClientTransactionButton runat="server" Text="Open in new Window with same ClientTransaction"></asp:button></FORM>
	
  </body>
</HTML>
