<%@ Page language="c#" Codebehind="DesignTestCheckBoxForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestCheckBoxForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: BooleanValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><rubicon:webbutton id=PostBackButton runat="server" Text="PostBack"></rubicon:webbutton>
<h1>DesignTest: BooleanValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox1 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox2 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox3 runat="server" datasourcecontrol="CurrentObject" Width="300px" propertyidentifier="Deceased" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox4 runat="server" datasourcecontrol="CurrentObject" Width="150px" propertyidentifier="Deceased" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox17 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" showdescription="True">
    </rubicon:boccheckbox><rubicon:boccheckbox id=BocCheckBox18 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox5 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox6 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" readonly="True" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox7 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="300px" propertyidentifier="Deceased" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox8 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="33%" propertyidentifier="Deceased" showdescription="True">
    </rubicon:boccheckbox><rubicon:boccheckbox id=BocCheckBox19 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="33%" propertyidentifier="Deceased" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox9 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox10 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox11 runat="server" datasourcecontrol="CurrentObject" Width="300px" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox12 runat="server" datasourcecontrol="CurrentObject" Width="150px" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox22 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox><rubicon:boccheckbox id=BocCheckBox23 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id=BocCheckBox13 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocCheckBox right" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox14" cssclass="bocCheckBox right" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="50%" readonly="True" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox15" cssclass="bocCheckBox right" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox16" cssclass="bocCheckBox right" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox20" cssclass="bocCheckBox right" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></rubicon:boccheckbox>
<rubicon:boccheckbox id="BocCheckBox21" cssclass="bocCheckBox right" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle>
</rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox24" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" cssclass="bocCheckBox block" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox25" runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" cssclass="bocCheckBox block" showdescription="True"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox26" runat="server" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox27" runat="server" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox28" runat="server" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></rubicon:boccheckbox>
    <rubicon:boccheckbox id="BocCheckBox29" runat="server" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox30" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocCheckBox block" showdescription="True">
</rubicon:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox31" cssclass="bocCheckBox block" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="50%" readonly="True" showdescription="True">
</rubicon:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox32" cssclass="bocCheckBox block" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
</rubicon:boccheckbox></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox33" cssclass="bocCheckBox block" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True">
</rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><rubicon:boccheckbox id="BocCheckBox34" cssclass="bocCheckBox block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased"></rubicon:boccheckbox>
<rubicon:boccheckbox id="BocCheckBox35" cssclass="bocCheckBox block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased"></rubicon:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>  </table><rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" typename="Rubicon.ObjectBinding.Sample::Person"></rubicon:BindableObjectDataSourceControl></form>
  </body>
</html>
