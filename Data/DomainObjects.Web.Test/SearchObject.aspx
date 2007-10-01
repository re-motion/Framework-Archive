<%@ Page language="c#" Codebehind="SearchObject.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.SearchObjectPage" %>

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
          <TD><rubicon:boctextvalue id="StringPropertyValue" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="StringProperty">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </rubicon:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><rubicon:boctextvalue id="BytePropertyFromTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyFrom">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </rubicon:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><rubicon:boctextvalue id="BytePropertyToTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyTo">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </rubicon:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><rubicon:bocenumvalue id="EnumPropertyValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="EnumProperty">
              <ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding=""></ListControlStyle>
            </rubicon:bocenumvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <rubicon:BocDateTimeValue id="DatePropertyFromValue" runat="server" PropertyIdentifier="DatePropertyFrom"
              DataSourceControl="CurrentSearchObject"></rubicon:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <rubicon:BocDateTimeValue id="DatePropertyToValue" runat="server" PropertyIdentifier="DatePropertyTo" DataSourceControl="CurrentSearchObject"></rubicon:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <rubicon:BocDateTimeValue id="DateTimeFromValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="DateTimePropertyFrom"></rubicon:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <rubicon:BocDateTimeValue id="BocDateTimeValue2" runat="server" PropertyIdentifier="DateTimePropertyTo" DataSourceControl="CurrentSearchObject"></rubicon:BocDateTimeValue></TD>
        </TR>
      </TABLE>
      <asp:button id="SearchButton" runat="server" Text="Suchen"></asp:button><rubicon:boclist id="ResultList" runat="server" DataSourceControl="FoundObjects">
<FixedColumns>
<rubicon:BocRowEditModeColumnDefinition SaveText="Speichern" CancelText="Abbrechen" EditText="Bearbeiten" ColumnTitle="Aktion"></rubicon:BocRowEditModeColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="StringProperty">
<PersistedCommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="ByteProperty">
<PersistedCommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
<PersistedCommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DateProperty">
<PersistedCommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DateTimeProperty">
<PersistedCommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="BooleanProperty">
<PersistedCommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
<rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="NaBooleanProperty">
<PersistedCommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocSimpleColumnDefinition>
</FixedColumns>
      </rubicon:boclist><rubicon:formgridmanager id="SearchFormGridManager" runat="server"></rubicon:formgridmanager><rubicon:domainobjectdatasourcecontrol id="FoundObjects" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Rubicon.Data.DomainObjects.Web.Test"></rubicon:domainobjectdatasourcecontrol>
      <rubicon:SearchObjectDataSourceControl id="CurrentSearchObject" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypesSearch, Rubicon.Data.DomainObjects.Web.Test"></rubicon:SearchObjectDataSourceControl></form>
  </body>
</HTML>
