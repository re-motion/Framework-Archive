<%@ Page language="c#" Codebehind="DesignTestTextValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestTextValueForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: TextValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><rubicon:webbutton id=PostBackButton runat="server" Text="PostBack"></rubicon:webbutton>
<h1>DesignTest: TextValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue1 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject"></rubicon:boctextvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue36 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="100%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue37 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="100%">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">textbox 100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue2 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject"></rubicon:boctextvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue38 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="50%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue39 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="50%">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">textbox 50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue3 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px"></rubicon:boctextvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue40 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="300px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue41 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="300px">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">textbox 300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue4 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px"></rubicon:boctextvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue42 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<commonstyle width="150px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue43 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject">
<textboxstyle width="150px">
</TextBoxStyle></rubicon:boctextvalue></td>
    <td width="50%">textbox 150px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue17 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%">
    </rubicon:boctextvalue><rubicon:boctextvalue id=BocTextValue18 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%"></rubicon:boctextvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue5 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" readonly="True"></rubicon:boctextvalue></td>
    <td width="50%">width 100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue44 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="100%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">common 100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue45 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="100%">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">label 100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue6 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" readonly="True"></rubicon:boctextvalue></td>
    <td width="50%">width 50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue46 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="50%">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">common 50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue47 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="50%">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">label 50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue7 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" readonly="True">
<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">width 300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue48 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="300px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">common 300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue49 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="300px">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">label 300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue51 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" readonly="True">
<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">width 150px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue50 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<commonstyle width="150px">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">common 150px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=Boctextvalue52 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" readonly="True">
<textboxstyle>
</TextBoxStyle>

<labelstyle width="150px">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">label 150px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue8 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True">
    </rubicon:boctextvalue><rubicon:boctextvalue id=BocTextValue19 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True"></rubicon:boctextvalue></td>
    <td width="50%">2x 33%</td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue9 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue10 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue11 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue12 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue22 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boctextvalue><rubicon:boctextvalue id=BocTextValue23 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boctextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue13 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue14 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue15 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue16 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue20 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue><rubicon:boctextvalue id=BocTextValue21 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" readonly="True" cssclass="bocTextValue right">
<commonstyle cssclass="common">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>
</rubicon:boctextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue24 runat="server" propertyidentifier="FirstName" width="100%" datasourcecontrol="CurrentObject" cssclass="bocTextValue block"></rubicon:boctextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue25 runat="server" propertyidentifier="FirstName" width="50%" datasourcecontrol="CurrentObject" cssclass="bocTextValue block"></rubicon:boctextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue26 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocTextValue block"></rubicon:boctextvalue></td>
    <td width="50%">300px</td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue27 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocTextValue block"></rubicon:boctextvalue></td>
    <td width="50%">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%"><rubicon:boctextvalue id=BocTextValue28 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue block"></rubicon:boctextvalue><rubicon:boctextvalue id=BocTextValue29 runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocTextValue block"></rubicon:boctextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%"><rubicon:bocTextvalue id="BocTextValue30" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="FirstName" readonly="True" cssclass="bocTextValue block">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:bocTextvalue></td>
    <td width="50%">100%</td></tr>
  <tr>
    <td width="50%"><rubicon:bocTextvalue id="BocTextValue31" cssclass="bocTextValue block" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="FirstName" width="50%" readonly="True">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:bocTextvalue></td>
    <td width="50%">50%</td></tr>
  <tr>
    <td width="50%"><rubicon:bocTextvalue id="BocTextValue32" cssclass="bocTextValue block" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:bocTextvalue></td>
    <td width="50%">300px</td>
  </tr>
  <tr>
    <td width="50%"><rubicon:bocTextvalue id="BocTextValue33" cssclass="bocTextValue block" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle>
</rubicon:bocTextvalue></td>
    <td width="50%">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%"><rubicon:bocTextvalue id="BocTextValue34" cssclass="bocTextValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle></rubicon:bocTextvalue>
<rubicon:bocTextvalue id="BocTextValue35" cssclass="bocTextValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="FirstName">
<commonstyle cssclass="label">
</CommonStyle>

<textboxstyle>
</TextBoxStyle>

<labelstyle cssclass="label">
</LabelStyle></rubicon:bocTextvalue></td>
    <td width="50%">
      <p>2x 33%</p></td>
  </tr></table><rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Rubicon.ObjectBinding.Sample::Person" /></form>
  </body>
</html>
