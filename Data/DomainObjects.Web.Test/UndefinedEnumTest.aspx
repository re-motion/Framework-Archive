<%@ Page language="c#" Codebehind="UndefinedEnumTest.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Data.DomainObjects.Web.Test.UndefinedEnumTestPage" %>

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
<h2>UndefinedEnumValueTest</h2>
      <TABLE id="SearchFormGrid" cellSpacing="0" cellPadding="0" width="300" border="0" runat="server">
        <TR>
          <TD style="WIDTH: 214px">Neues Objekt (1):</TD>
          <TD style="WIDTH: 403px"><rubicon:bocenumvalue id="NewObjectEnumProperty" runat="server" DataSourceControl="NewObjectWithUndefinedEnumDataSource" PropertyIdentifier="UndefinedEnum">
<ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding="">
</ListControlStyle>
            </rubicon:bocenumvalue></TD>
        </TR>
        <TR>
          <TD style="WIDTH: 214px"> Bestehendes Objekt (2):</TD>
          <TD style="WIDTH: 403px"><rubicon:bocenumvalue id="ExistingObjectEnumProperty" runat="server" DataSourceControl="ExistingObjectWithUndefinedEnumDataSource" PropertyIdentifier="UndefinedEnum">
<ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding="">
</ListControlStyle>
            </rubicon:bocenumvalue></TD>
        </TR>
        <TR>
          <TD style="WIDTH: 214px">
      <P>Search Objekt (3):</P></TD>
          <TD style="WIDTH: 403px"><rubicon:bocenumvalue id="SearchObjectEnumProperty" runat="server" DataSourceControl="SearchObjectWithUndefinedEnumDataSource" PropertyIdentifier="UndefinedEnum" Required="False">
<ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding="">
</ListControlStyle>
            </rubicon:bocenumvalue></TD>
        </TR>
      </TABLE>
<P>Visuelle Checks des gerenderten BocEnumValue 
Controls:</P>
<UL>
  <LI>"Value1" muss überall auszuwählen sein. 
  <LI>"Undefined" darf nirgends zur Auswahl stehen.
  <LI>Bei (1) muss ein Stern zu sehen und initial eine 
  leere Zeile ausgewält sein. <BR>Es muss ein Validator anspringen, 
  solange&nbsp;nicht "Value1"&nbsp;ausgewählt ist. 
  <LI>Bei (2) muss ein Stern zu sehen sein und&nbsp;es darf keine leere Zeile 
  auswählbar sein.&nbsp; 
  <LI>Bei (3)&nbsp;darf kein Stern zu sehen sein&nbsp;und 
  eine leere Zeile muss zur Auswahl stehen.</LI></UL>
<P>Für den&nbsp;Abschluss des Tests muss "Value1" bei (1) ausgewählt werden und 
beim Klicken auf "Test fortsetzen" darf keine Exception kommen.</P>
<P>
      <asp:button id="TestButton" runat="server" Text="Test fortsetzten"></asp:button><rubicon:formgridmanager id="FormGridManager" runat="server"></rubicon:formgridmanager><rubicon:domainobjectdatasourcecontrol id="ExistingObjectWithUndefinedEnumDataSource" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithUndefinedEnum, Rubicon.Data.DomainObjects.Web.Test"></rubicon:domainobjectdatasourcecontrol><rubicon:domainobjectdatasourcecontrol id="NewObjectWithUndefinedEnumDataSource" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithUndefinedEnum, Rubicon.Data.DomainObjects.Web.Test"></rubicon:domainobjectdatasourcecontrol>
      <rubicon:SearchObjectDataSourceControl id="SearchObjectWithUndefinedEnumDataSource" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.SearchObjectWithUndefinedEnum, Rubicon.Data.DomainObjects.Web.Test"></rubicon:SearchObjectDataSourceControl></P></form>
  </body>
</HTML>
