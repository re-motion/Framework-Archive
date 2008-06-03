<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CompleteBocTestUserControl.ascx.cs" Inherits="OBWTest.CompleteBocUserControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>






<table id="FormGrid" runat="server">
  <tr>
    <td colspan="2"><remotion:boctextvalue id="FirstNameField" runat="server" PropertyIdentifier="FirstName" datasourcecontrol="CurrentObject" ReadOnly="True"></remotion:boctextvalue>&nbsp;<remotion:boctextvalue id="LastNameField" runat="server" PropertyIdentifier="LastName" datasourcecontrol="CurrentObject" ReadOnly="True"></remotion:boctextvalue></td></tr>
  <tr>
    <td></td>
    <td><remotion:BocTextValue id="TextField" runat="server" propertyidentifier="FirstName" datasourcecontrol="CurrentObject"></remotion:BocTextValue></td></tr>
  <tr>
    <td></td>
    <td><remotion:BocMultilineTextValue id="MultilineTextField" runat="server" propertyidentifier="CV" datasourcecontrol="CurrentObject">
<textboxstyle textmode="MultiLine">
</textBoxStyle></remotion:BocMultilineTextValue></td></tr>
  <tr>
    <td></td>
    <td><remotion:BocDateTimeValue id="DateTimeField" runat="server" propertyidentifier="DateOfBirth" datasourcecontrol="CurrentObject"  incompleteerrormessage="Unvollständige Daten" invaliddateandtimeerrormessage="Ungültiges Datum oder Zeit" invaliddateerrormessage="Ungültiges Datum" invalidtimeerrormessage="Ungültige Zeit" ></remotion:BocDateTimeValue></td></tr>
  <tr>
    <td style="HEIGHT: 18px"></td>
    <td style="HEIGHT: 18px"><remotion:BocEnumValue id="EnumField" runat="server" propertyidentifier="MarriageStatus" datasourcecontrol="CurrentObject">
<listcontrolstyle>
</listControlStyle></remotion:BocEnumValue></td></tr>
  <tr>
    <td></td>
    <td><remotion:BocReferenceValue id="ReferenceField" runat="server" propertyidentifier="Partner" datasourcecontrol="CurrentObject">
</remotion:BocReferenceValue></td></tr>
  <tr>
    <td></td>
    <td><remotion:BocBooleanValue id="BooleanField" runat="server" propertyidentifier="Deceased" datasourcecontrol="CurrentObject"></remotion:BocBooleanValue></td></tr>
  <tr>
    <td></td>
    <td></td></tr>
  <tr>
    <td colspan="2"><remotion:BocList id="ListField" runat="server" propertyidentifier="Jobs" datasourcecontrol="CurrentObject" showsortingorder="True" enableselection="True" alwaysshowpageinfo="True" selection="Multiple">
<fixedcolumns>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Title">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="StartDate">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns></remotion:BocList></td></tr></table>
<p><remotion:formgridmanager id="FormGridManager" runat="server" visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"></remotion:BindableObjectDataSourceControl></p>
<p><asp:button id="SaveButton" runat="server" Width="80px" Text="Save"></asp:button><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
<remotion:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView" width="100%" height="10%">
<views> 
 <remotion:tabview id="first" title="First">
 </remotion:tabview>
 <remotion:tabview id="second" title="Second">
 </remotion:tabview>
</Views>
</remotion:tabbedmultiview>
