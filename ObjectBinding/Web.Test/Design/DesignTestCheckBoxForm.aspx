<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="DesignTestCheckBoxForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestCheckBoxForm" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleBocTest: TestBooleanValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><rwc:webbutton id=PostBackButton runat="server" Text="PostBack"></rwc:webbutton>
<h1>DesignTest: BooleanValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox1 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox2 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox3 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" propertyidentifier="Deceased" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox4 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" propertyidentifier="Deceased" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox17 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased" showdescription="True">
    </obc:boccheckbox><obc:boccheckbox id=BocCheckBox18 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox5 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" readonly="True" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox6 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased" readonly="True" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox7 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" Width="300px" propertyidentifier="Deceased" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox8 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" Width="33%" propertyidentifier="Deceased" showdescription="True">
    </obc:boccheckbox><obc:boccheckbox id=BocCheckBox19 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" Width="33%" propertyidentifier="Deceased" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox9 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox10 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox11 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox12 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox22 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox><obc:boccheckbox id=BocCheckBox23 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle>
</obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id=BocCheckBox13 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox14" cssclass="bocCheckBox right" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" width="50%" readonly="True" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox15" cssclass="bocCheckBox right" runat="server" Width="300px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox16" cssclass="bocCheckBox right" runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox20" cssclass="bocCheckBox right" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></obc:boccheckbox>
<obc:boccheckbox id="BocCheckBox21" cssclass="bocCheckBox right" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle>
</obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox24" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" cssclass="bocCheckBox block" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox25" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased" cssclass="bocCheckBox block" showdescription="True"></obc:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox26" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></obc:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox27" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox28" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></obc:boccheckbox>
    <obc:boccheckbox id="BocCheckBox29" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox30" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocCheckBox block" showdescription="True">
</obc:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox31" cssclass="bocCheckBox block" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" width="50%" readonly="True" showdescription="True">
</obc:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox32" cssclass="bocCheckBox block" runat="server" Width="300px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True" propertyidentifier="Deceased">
</obc:boccheckbox></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox33" cssclass="bocCheckBox block" runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True">
</obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><obc:boccheckbox id="BocCheckBox34" cssclass="bocCheckBox block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True" propertyidentifier="Deceased"></obc:boccheckbox>
<obc:boccheckbox id="BocCheckBox35" cssclass="bocCheckBox block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" showdescription="True" propertyidentifier="Deceased"></obc:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>  </table><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></form>
  </body>
</html>
