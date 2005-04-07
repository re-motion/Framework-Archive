<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="PersonDetailsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonDetailsForm" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>PersonDetails Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
    <rwc:htmlheadcontents runat="server" id="HtmlHeadContents"></rwc:htmlheadcontents>
  </head>
  <body>
    <form id=Form method=post runat="server"><h1>PersonDetails Form</h1>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=2>Persondetails</td></tr>
        <tr>
          <td></td>
          <td><obc:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="200px" required="True"></obc:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><obc:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" Width="200px" required="True"></obc:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><obc:bocenumvalue id="GenderField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Gender">
              <listcontrolstyle radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" controltype="DropDownList" radiobuttonlistrepeatdirection="Vertical">
              </listcontrolstyle></obc:bocenumvalue></td></tr>
        <tr>
          <td style="HEIGHT: 14px"></td>
          <td style="HEIGHT: 14px"><obc:bocreferencevalue id="PartnerField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="Partner">
<optionsmenuitems>
<obc:BocMenuItem Text="Copy" ItemID="Copy" RequiredSelection="ExactlyOne">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems>

<persistedcommand>
<obc:BocCommand Type="None"></obc:BocCommand>
</PersistedCommand></obc:bocreferencevalue></td></tr>
        <tr>
          <td></td>
          <td><obc:bocdatetimevalue id="BirthdayField" runat="server" datasourcecontrol="ReflectionBusinessObjectDataSourceControl" propertyidentifier="DateOfBirth"></obc:bocdatetimevalue></td></tr>
        <tr>
          <td></td>
          <td><obc:BocBooleanValue id="DeceasedField" runat="server" propertyidentifier="Deceased" datasourcecontrol="ReflectionBusinessObjectDataSourceControl"></obc:BocBooleanValue></td></tr>
        <tr>
          <td></td>
          <td><obc:BocMultilineTextValue id="CVField" runat="server" propertyidentifier="CV" datasourcecontrol="ReflectionBusinessObjectDataSourceControl">
<textboxstyle textmode="MultiLine">
</TextBoxStyle></obc:BocMultilineTextValue></td></tr>
        <tr>
          <td style="HEIGHT: 17px"></td>
          <td style="HEIGHT: 17px"><obc:BocList id="JobList" runat="server" PropertyIdentifier="Jobs" DataSourceControl="ReflectionBusinessObjectDataSourceControl" ShowAdditionalColumnsList="False" ShowAllProperties="True" EnableSelection="True" showemptylistmenu="False">
<optionsmenuitems>
<obc:BocMenuItem Text="Copy" ItemID="Copy">
<persistedcommand>
<obc:BocMenuItemCommand Type="Event"></obc:BocMenuItemCommand>
</PersistedCommand>
</obc:BocMenuItem>
</OptionsMenuItems></obc:BocList></td></tr>
          </table>
      <p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
      <p><rwc:formgridmanager id=FormGridManager runat="server" visible="true"></rwc:formgridmanager><obr:reflectionbusinessobjectdatasourcecontrol id="ReflectionBusinessObjectDataSourceControl" runat="server" typename="OBRTest.Person, OBRTest"></obr:reflectionbusinessobjectdatasourcecontrol></p></form>

  </body>
</html>
