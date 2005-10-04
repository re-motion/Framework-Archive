<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="DesignTestEnumValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestEnumValueForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: TestTextValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><rwc:webbutton id=PostBackButton runat="server" Text="PostBack"></rwc:webbutton>
<h1>DesignTest: EnumValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode: Drop Down List</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue1 runat="server" propertyidentifier="MarriageStatus" width="100%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:bocenumvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue36 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="100%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue37 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle width="100%">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue2 runat="server" propertyidentifier="MarriageStatus" width="50%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:bocenumvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue38 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="50%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue39 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle width="50%">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue3 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px"></obc:bocenumvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue40 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="300px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue41 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle width="300px">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue4 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px"></obc:bocenumvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue42 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="150px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue43 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle width="150px" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue17 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%">
    </obc:bocenumvalue><obc:bocenumvalue id=BocEnumValue18 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%"></obc:bocenumvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode: Radio Button List</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue53" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="MarriageStatus">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue54" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus">
<commonstyle width="100%">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue55" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus">
<listcontrolstyle width="100%" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue56" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="MarriageStatus">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue5" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="50%">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue6" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle width="50%" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue7" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue8" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="300px">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue9" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle width="300px" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue10" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue11" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<commonstyle width="150px">
</CommonStyle>

<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue12" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<listcontrolstyle width="150px" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">listcontrol 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue13" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle>
    </obc:bocenumvalue><obc:bocenumvalue id="Bocenumvalue14" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%">
<listcontrolstyle controltype="RadioButtonList" radiobuttonlistrepeatdirection="Horizontal" radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></obc:bocenumvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue57" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="MarriageStatus" readonly="True"></obc:bocenumvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue44 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="100%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue45 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="100%">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">label 100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue58" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="MarriageStatus" readonly="True"></obc:bocenumvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue46 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="50%">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue47 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="50%">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">label 50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue59" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus" Width="300px" readonly="True">
<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue48 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="300px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue49 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="300px">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">label 300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue51 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" readonly="True">
<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue50 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<commonstyle width="150px">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue52 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True">
<listcontrolstyle>
</ListControlStyle>

<labelstyle width="150px">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">label 150px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue60" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus" Width="33%" readonly="True">
    </obc:bocenumvalue><obc:bocenumvalue id=BocEnumValue19 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" readonly="True"></obc:bocenumvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" style="HEIGHT: 17px"><obc:bocenumvalue id="Bocenumvalue61" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="MarriageStatus" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%" style="HEIGHT: 17px">100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue62" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="MarriageStatus" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue63" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus" Width="300px" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue64" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus" Width="150px" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue22 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocenumvalue><obc:bocenumvalue id=BocEnumValue23 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:bocenumvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue65" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="MarriageStatus" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id="Bocenumvalue66" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="MarriageStatus" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue15 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue16 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue20 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue><obc:bocenumvalue id=BocEnumValue21 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" readonly="True" cssclass="BocEnumValue right">
<commonstyle cssclass="common">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>
</obc:bocenumvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue24 runat="server" propertyidentifier="MarriageStatus" width="100%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" cssclass="BocEnumValue block"></obc:bocenumvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue25 runat="server" propertyidentifier="MarriageStatus" width="50%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" cssclass="BocEnumValue block"></obc:bocenumvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue26 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" cssclass="BocEnumValue block"></obc:bocenumvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue27 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" cssclass="BocEnumValue block"></obc:bocenumvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><obc:bocenumvalue id=BocEnumValue28 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocEnumValue block"></obc:bocenumvalue><obc:bocenumvalue id=BocEnumValue29 runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="BocEnumValue block"></obc:bocenumvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%"><obc:BocEnumValue id="BocEnumValue30" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="MarriageStatus" readonly="True" cssclass="BocEnumValue block">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:BocEnumValue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><obc:BocEnumValue id="BocEnumValue31" cssclass="BocEnumValue block" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="MarriageStatus" width="50%" readonly="True">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:BocEnumValue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><obc:BocEnumValue id="BocEnumValue32" cssclass="BocEnumValue block" runat="server" Width="300px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:BocEnumValue></td>
    <td width="50%">300px</td>
  </tr>
  <tr>
    <td width="50%"><obc:BocEnumValue id="BocEnumValue33" cssclass="BocEnumValue block" runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle>
</obc:BocEnumValue></td>
    <td width="50%">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%"><obc:BocEnumValue id="BocEnumValue34" cssclass="BocEnumValue block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle></obc:BocEnumValue>
<obc:BocEnumValue id="BocEnumValue35" cssclass="BocEnumValue block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="MarriageStatus">
<commonstyle cssclass="label">
</CommonStyle>

<listcontrolstyle>
</ListControlStyle>

<labelstyle cssclass="label">
</LabelStyle></obc:BocEnumValue></td>
    <td width="50%">
      <p>2x 33%</p></td>
  </tr></table><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></form>
  </body>
</html>
