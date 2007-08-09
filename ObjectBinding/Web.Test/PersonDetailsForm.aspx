

<%@ Page language="c#" Codebehind="PersonDetailsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonDetailsForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>PersonDetails Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
    <rubicon:htmlheadcontents runat="server" id="HtmlHeadContents"></rubicon:htmlheadcontents>
  </head>
  <body>
    <form id=Form method=post runat="server"><h1>PersonDetails Form</h1>
      <table id=FormGrid runat="server" width="100%">
        <tr>
          <td colSpan=2>Persondetails</td></tr>
        <tr>
          <td></td>
          <td><rubicon:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" Width="100%" required="True">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><rubicon:boctextvalue id=FirstNameField runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" Width="100%" required="True">
<textboxstyle textmode="SingleLine">
</TextBoxStyle></rubicon:boctextvalue></td></tr>
        <tr>
          <td></td>
          <td><rubicon:bocenumvalue id="GenderField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Gender" width="100%">
<listcontrolstyle radiobuttonlistcellspacing="" radiobuttonlistcellpadding="">
</ListControlStyle></rubicon:bocenumvalue></td></tr>
        <tr>
          <td style="HEIGHT: 14px"></td>
          <td style="HEIGHT: 14px"><rubicon:bocreferencevalue id="PartnerField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Partner" width="100%">

<dropdownliststyle autopostback="True">
</DropDownListStyle>

<persistedcommand>
<rubicon:BocCommand Type="Event"></rubicon:BocCommand>
</PersistedCommand></rubicon:bocreferencevalue></td></tr>
  <tr>
    <td></td>
    <td><rubicon:BocTextValue id="ParterFirstNameField" runat="server" DataSourceControl="PartnerDataSource" propertyidentifier="FirstName" width="100%">
<textboxstyle textmode="SingleLine">
</TextBoxStyle>
</rubicon:BocTextValue></td></tr>
        <tr>
          <td></td>
          <td><rubicon:bocdatetimevalue id="BirthdayField" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="DateOfBirth" width="100%"></rubicon:bocdatetimevalue></td></tr>
        <tr>
          <td></td>
          <td><rubicon:BocBooleanValue id="DeceasedField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject" width="100%"></rubicon:BocBooleanValue></td></tr>
        <tr>
          <td></td>
          <td><rubicon:BocMultilineTextValue id="CVField" runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject" width="100%">
<textboxstyle textmode="MultiLine">
</TextBoxStyle></rubicon:BocMultilineTextValue></td></tr>
        <tr>
          <td style="HEIGHT: 17px"></td>
          <td style="HEIGHT: 17px"><rubicon:BocList id="JobList" runat="server" PropertyIdentifier="Jobs" DataSourceControl="CurrentObject" ShowAvailableViewsList="false" ShowAllProperties="True" EnableSelection="True" showemptylistmenu="False">
</rubicon:BocList></td></tr>
          </table>
      <p><asp:button id=SaveButton runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
      <p><rubicon:formgridmanager id=FormGridManager runat="server" visible="true"></rubicon:formgridmanager><rubicon:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Rubicon.ObjectBinding.Sample::Person" /><rubicon:BusinessObjectReferenceDataSourceControl id="PartnerDataSource" runat="server" PropertyIdentifier="Partner" DataSourceControl="CurrentObject"></rubicon:BusinessObjectReferenceDataSourceControl></p></form>

  </body>
</html>
