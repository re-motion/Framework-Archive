<%-- This file is part of re-strict (www.re-motion.org)
 % Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
 % 
 % This program is free software; you can redistribute it and/or modify
 % it under the terms of the GNU Affero General Public License version 3.0 
 % as published by the Free Software Foundation.
 % 
 % This program is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Affero General Public License for more details.
 % 
 % You should have received a copy of the GNU Affero General Public License
 % along with this program; if not, see http://www.gnu.org/licenses.
 % 
 % Additional permissions are listed in the file re-motion_exceptions.txt.
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.Test._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html >
<head runat="server">
    <title>Untitled Page</title>
    <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <p>
    <a href="UserList.wxe">Aufbauorganisation verwalten</a>
    </p>
    <p>
    <a href="SecurableClassDefinitionList.wxe?WxeReturnToSelf=True&TabbedMenuSelection=AccessControlTab">Berechtigungen verwalten</a>
    </p>
    <p>
      <asp:Button ID="EvaluateSecurity" runat="server" Text="Evaluate Security" OnClick="EvaluateSecurity_Click" />
    </p>
    <p>
      <remotion:BocReferenceValue runat="server" ID="UsersField" OnSelectionChanged="UsersField_SelectionChanged">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
        <DropDownListStyle AutoPostBack="True" />
      </remotion:BocReferenceValue>
    </p>
    </form>
</body>
</html>
