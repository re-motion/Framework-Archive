<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page language="c#" Codebehind="PersonsForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.PersonsForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Persons Form</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
    <meta content=C# name=CODE_LANGUAGE>
    <meta content=JavaScript name=vs_defaultClientScript>
    <meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
    <remotion:htmlheadcontents runat="server" id="HtmlHeadContents"></remotion:htmlheadcontents>
  </head>
  <body>
    <form id=Form method=post runat="server"><h1>Persons Form</h1>
      <table id=FormGrid runat="server">
        <tr>
          <td colSpan=2>Persons</td></tr>
        <tr>
          <td></td>
          <td><remotion:BocList id="PersonList" runat="server" PropertyIdentifier="" DataSourceControl="CurrentObject" ShowAllProperties="True" >
<fixedcolumns>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
<persistedcommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns></remotion:BocList></td></tr>
          </table>
      <p><asp:button id="PostBackButton" runat="server" Text="Post Back"></asp:button></p>
      <p><remotion:formgridmanager id=FormGridManager runat="server" visible="true"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" /></p></form>

  </body>
</html>
