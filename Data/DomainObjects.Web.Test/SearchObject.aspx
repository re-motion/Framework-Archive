<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="cc1" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="SearchObject.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.SearchObjectPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>SearchObject</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
  </HEAD>
  <body>
    <form id="SearchObjectForm" method="post" runat="server">
      <TABLE id="SearchFormGrid" cellSpacing="0" cellPadding="0" width="300" border="0" runat="server">
        <TR>
          <TD></TD>
          <TD><obw:boctextvalue id="StringPropertyValue" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="StringProperty">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </obw:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><obw:boctextvalue id="BytePropertyFromTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyFrom">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </obw:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><obw:boctextvalue id="BytePropertyToTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyTo">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </obw:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><obw:bocenumvalue id="EnumPropertyValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="EnumProperty">
              <ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding=""></ListControlStyle>
            </obw:bocenumvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <obw:BocDateTimeValue id="DatePropertyFromValue" runat="server" PropertyIdentifier="DatePropertyFrom"
              DataSourceControl="CurrentSearchObject"></obw:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <obw:BocDateTimeValue id="DatePropertyToValue" runat="server" PropertyIdentifier="DatePropertyTo" DataSourceControl="CurrentSearchObject"></obw:BocDateTimeValue></TD>
        </TR>
      </TABLE>
      <asp:button id="SearchButton" runat="server" Text="Suchen"></asp:button><obw:boclist id="ResultList" runat="server" DataSourceControl="FoundObjects">
        <FixedColumns>
          <obw:BocSimpleColumnDefinition PropertyPathIdentifier="StringProperty">
            <PersistedCommand>
              <obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
            </PersistedCommand>
          </obw:BocSimpleColumnDefinition>
          <obw:BocSimpleColumnDefinition PropertyPathIdentifier="ByteProperty">
            <PersistedCommand>
              <obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
            </PersistedCommand>
          </obw:BocSimpleColumnDefinition>
          <obw:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
            <PersistedCommand>
              <obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
            </PersistedCommand>
          </obw:BocSimpleColumnDefinition>
          <obw:BocSimpleColumnDefinition PropertyPathIdentifier="DateProperty">
            <PersistedCommand>
              <obw:BocListItemCommand Type="None"></obw:BocListItemCommand>
            </PersistedCommand>
          </obw:BocSimpleColumnDefinition>
        </FixedColumns>
      </obw:boclist><rubicon:formgridmanager id="SearchFormGridManager" runat="server"></rubicon:formgridmanager><cc1:domainobjectdatasourcecontrol id="FoundObjects" runat="server" TypeName="Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain.ClassWithAllDataTypes, Rubicon.Data.DomainObjects.ObjectBinding.Web.Test"></cc1:domainobjectdatasourcecontrol>
      <cc1:SearchObjectDataSourceControl id="CurrentSearchObject" runat="server" TypeName="Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain.ClassWithAllDataTypesSearch, Rubicon.Data.DomainObjects.ObjectBinding.Web.Test"></cc1:SearchObjectDataSourceControl></form>
  </body>
</HTML>
