<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSuiteForm.aspx.cs" Inherits="Remotion.Web.Test.MultiplePostBackCatching.TestSuiteForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Remotion.Web.Tests.MultiplePostBackCatching</title>
    <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body style="overflow:visible;">
  <form id="MyForm" runat="server">
    <asp:Table ID="TestSuiteTable" runat="server">
      <asp:TableHeaderRow>
        <asp:TableHeaderCell>Test Suite for MultiplePostBackCatching</asp:TableHeaderCell>
      </asp:TableHeaderRow>
    </asp:Table>
  </form>
</body>
</html>
