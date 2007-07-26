


<%@ Page language="c#" Codebehind="SingleColumnFormGridsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleColumnFormGridsForm" %><!doctype HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Singe Column Form Grids</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
    <style>
.singleColumnFormGrid TD.formGridMarkersCell { BORDER-RIGHT: white 0px solid; PADDING-RIGHT: 5px; BORDER-TOP: white 1px solid; PADDING-LEFT: 5px; PADDING-BOTTOM: 0px; BORDER-LEFT: white 0px solid; WIDTH: 100%; PADDING-TOP: 0px; BORDER-BOTTOM: white 0px solid; WHITE-SPACE: nowrap; BACKGROUND-COLOR: #e1ecfc }
.singleColumnFormGrid TD.formGridTopDataRow { BORDER-RIGHT: white 0px solid; BORDER-TOP: white 3px solid; BORDER-LEFT: white 0px solid; BORDER-BOTTOM: white 0px solid }
</style>
</head>
  <body MS_POSITIONING="FlowLayout">
	
    <form id="Form" method="post" runat="server">
<table id="MainFormGrid" style="WIDTH: 100%" runat="server" >
<tr>
<td><rubicon:smartlabel id="Smartlabel4" runat="server" forcontrol="BocTextValue1" width="100%"></rubicon:SmartLabel></td>
<td >
<table style="width:100%">
<tr>
<td style="width:50%; vertical-align:top">
<table id="LeftFormGrid" style="WIDTH: 100%; vertical-align:top;" runat="server">
  <tr class="singleColumnFormGrid">
    <td style="WIDTH: 0%">
      <rubicon:SmartLabel id=SmartLabel1 runat="server" forcontrol="BocTextValue1" style="width:100%; white-space:nowrap;">
      </rubicon:SmartLabel></td>
      <td style="DISPLAY:none"></td></tr>
      <tr>
      <td colspan="2"><rubicon:BocTextValue id="BocTextValue1" runat="server" style="width:100%;">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:BocTextValue></td>
      </tr>
  <tr class="singleColumnFormGrid">
    <td style="WIDTH: 0%">
      <rubicon:smartlabel id="Smartlabel3" runat="server" forcontrol="BocTextValue1" style="width:100%; white-space:nowrap;">
      </rubicon:smartlabel></td>
      <td style="DISPLAY:none"></td></tr>
      <tr>
      <td colspan="2"><rubicon:BocList id="BocList1" runat="server">
<fixedcolumns>
<rubicon:BocCommandColumnDefinition Text="first" ColumnTitle="First">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
<rubicon:BocCommandColumnDefinition Text="second" ColumnTitle="Second">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
<rubicon:BocCommandColumnDefinition Text="thrid" ColumnTitle="Third">
<persistedcommand>
<rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
</PersistedCommand>
</rubicon:BocCommandColumnDefinition>
</FixedColumns>
</rubicon:BocList></td>
      </tr>
</table>
</td>
<td style="width:50%;vertical-align:top">
<table id="RightFormGrid" style="WIDTH: 100%;vertical-align:top" runat="server">
  <tr class="singleColumnFormGrid">
    <td style="WIDTH: 0%">
      <rubicon:smartlabel id="Smartlabel2" runat="server" forcontrol="BocTextValue2" style="width:100%; white-space:nowrap;">
      </rubicon:smartlabel></td>
      <td style="DISPLAY:none"></td></tr>
      <tr>
      <td colspan="2"><rubicon:boctextvalue id="Boctextvalue2" runat="server" style="width:100%;">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:boctextvalue></td>
      </tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
</table>
<rubicon:formgridmanager id="FormgridManager" runat="server"></rubicon:formgridmanager>
     </form>
	
  </body>
</html>
