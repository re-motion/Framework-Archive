<%@ Page language="c#" AutoEventWireup="false" Inherits="Rubicon.Web.UI.Controls.DatePickerPage" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Date Picker</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
  </head>
<body MS_POSITIONING="FlowLayout">
<form id=Form method=post runat="server"><asp:calendar id=Calendar runat="server" Height="100%" Width="100%" BackColor="White" DayNameFormat="FirstLetter" ForeColor="Black" Font-Size="8pt" Font-Names="Verdana" BorderColor="#999999" CellPadding="4">
<todaydaystyle forecolor="Black" backcolor="#CCCCCC">
</TodayDayStyle>

<selectorstyle backcolor="#CCCCCC">
</SelectorStyle>

<nextprevstyle verticalalign="Bottom">
</NextPrevStyle>

<dayheaderstyle font-size="7pt" font-bold="True" backcolor="#CCCCCC">
</DayHeaderStyle>

<selecteddaystyle font-bold="True" forecolor="White" backcolor="#666666">
</SelectedDayStyle>

<titlestyle font-bold="True" bordercolor="Black" backcolor="#999999">
</TitleStyle>

<weekenddaystyle backcolor="#FFFFCC">
</WeekendDayStyle>

<othermonthdaystyle forecolor="#808080">
</OtherMonthDayStyle>
</asp:Calendar></FORM></body>
</html>
