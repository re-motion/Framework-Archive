<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="DesignTestBooleanValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestBooleanValueForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
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
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue1 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue2 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue3 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue4 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue17 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased">
    </obc:bocbooleanvalue><obc:bocbooleanvalue id=BocBooleanValue18 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue5 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" readonly="True"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue6 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased" readonly="True"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue7 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" Width="300px" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue8 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" Width="33%" propertyidentifier="Deceased">
    </obc:bocbooleanvalue><obc:bocbooleanvalue id=BocBooleanValue19 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" Width="33%" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue9 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue10 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue11 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" cssclass="bocBooleanValue right" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue12 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" cssclass="bocBooleanValue right" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue22 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue><obc:bocbooleanvalue id=BocBooleanValue23 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle>
</obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id=BocBooleanValue13 runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue14" cssclass="bocBooleanValue right" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" width="50%" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue15" cssclass="bocBooleanValue right" runat="server" Width="300px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue16" cssclass="bocBooleanValue right" runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue20" cssclass="bocBooleanValue right" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></obc:bocbooleanvalue>
<obc:bocbooleanvalue id="BocBooleanValue21" cssclass="bocBooleanValue right" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle>
</obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue24" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" cssclass="bocBooleanValue block"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue25" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="50%" propertyidentifier="Deceased" cssclass="bocBooleanValue block"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue26" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="300px" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue27" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="150px" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue28" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></obc:bocbooleanvalue>
    <obc:bocbooleanvalue id="BocBooleanValue29" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="33%" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue30" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocBooleanValue block">
</obc:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue31" cssclass="bocBooleanValue block" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Deceased" width="50%" readonly="True">
</obc:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue32" cssclass="bocBooleanValue block" runat="server" Width="300px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="Deceased">
</obc:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue33" cssclass="bocBooleanValue block" runat="server" Width="150px" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="Deceased">
</obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><obc:bocbooleanvalue id="BocBooleanValue34" cssclass="bocBooleanValue block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="Deceased"></obc:bocbooleanvalue>
<obc:bocbooleanvalue id="BocBooleanValue35" cssclass="bocBooleanValue block" runat="server" Width="33%" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" readonly="True" propertyidentifier="Deceased"></obc:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>  </table><obr:ReflectionBusinessObjectDataSourceControl id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:ReflectionBusinessObjectDataSourceControl></form>
  </body>
</html>
