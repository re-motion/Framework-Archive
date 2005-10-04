<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="DesignTestDateTimeValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestDateTimeValueForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: TestDateTimeValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><rwc:webbutton id=PostBackButton runat="server" Text="PostBack"></rwc:webbutton>
<h1>DesignTest: DateTimeVale Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale1 runat="server" propertyidentifier="DateOfBirth" width="100%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale36 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="100%">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale37 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<datetimetextboxstyle width="100%">
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">textbox 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale2 runat="server" propertyidentifier="DateOfBirth" width="50%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale38 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="50%">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale39 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<datetimetextboxstyle width="50%">
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">textbox 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale3 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale40 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="300px">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale41 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<datetimetextboxstyle width="300px">
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">textbox 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale4 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale42 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="150px">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale43 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<datetimetextboxstyle width="150px">
</datetimetextboxstyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">textbox 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale17 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%">
    </obc:bocdatetimevalue><obc:bocdatetimevalue id=BocDateTimeVale18 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale5 runat="server" propertyidentifier="DateOfBirth" width="100%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale44 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="100%">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale45 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle width="100%">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">label 100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale6 runat="server" propertyidentifier="DateOfBirth" width="50%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale46 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="50%">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale47 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle width="50%">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">label 50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale7 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" readonly="True">
<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale48 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="300px">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale49 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle width="300px">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">label 300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale51 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" readonly="True">
<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">width 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale50 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="150px">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">common 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale52 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle width="150px">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">label 150px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale8 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" readonly="True">
    </obc:bocdatetimevalue><obc:bocdatetimevalue id=BocDateTimeVale19 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" readonly="True"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale9 runat="server" propertyidentifier="DateOfBirth" width="100%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale10 runat="server" propertyidentifier="DateOfBirth" width="50%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale11 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale12 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale22 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue><obc:bocdatetimevalue id=BocDateTimeVale23 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale13 runat="server" propertyidentifier="DateOfBirth" width="100%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale14 runat="server" propertyidentifier="DateOfBirth" width="50%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale15 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" readonly="True" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale16 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" readonly="True" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale20 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" readonly="True" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue><obc:bocdatetimevalue id=BocDateTimeVale21 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" readonly="True" cssclass="BocDateTimeVale right">
<commonstyle cssclass="common">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale24 runat="server" propertyidentifier="DateOfBirth" width="100%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" cssclass="BocDateTimeVale block"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale25 runat="server" propertyidentifier="DateOfBirth" width="50%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" cssclass="BocDateTimeVale block"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale26 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" cssclass="BocDateTimeVale block"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale27 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" cssclass="BocDateTimeVale block"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id=BocDateTimeVale28 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocDateTimeVale block"></obc:bocdatetimevalue><obc:bocdatetimevalue id=BocDateTimeVale29 runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocDateTimeVale block"></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id="BocDateTimeVale30" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="DateOfBirth" readonly="True" cssclass="BocDateTimeVale block">
<commonstyle cssclass="label">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id="BocDateTimeVale31" cssclass="BocDateTimeVale block" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="DateOfBirth" width="50%" readonly="True">
<commonstyle cssclass="label">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id="BocDateTimeVale32" cssclass="BocDateTimeVale block" runat="server" Width="300px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="DateOfBirth">
<commonstyle cssclass="label">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id="BocDateTimeVale33" cssclass="BocDateTimeVale block" runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="DateOfBirth">
<commonstyle cssclass="label">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><obc:bocdatetimevalue id="BocDateTimeVale34" cssclass="BocDateTimeVale block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="DateOfBirth">
<commonstyle cssclass="label">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle></obc:bocdatetimevalue>
<obc:bocdatetimevalue id="BocDateTimeVale35" cssclass="BocDateTimeVale block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="DateOfBirth">
<commonstyle cssclass="label">
</CommonStyle>

<datetimetextboxstyle>
</datetimetextboxstyle>

<labelstyle cssclass="label">
</LabelStyle></obc:bocdatetimevalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr></table><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></form>
  </body>
</html>
