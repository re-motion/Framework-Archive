﻿<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TabbedMultiViewTest.aspx.cs" Inherits="Remotion.Web.Test.Theming.TabbedMultiViewTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!-- <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > -->

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Tabbed multi-view theming test</title>
    <remotion:HtmlHeadContents runat="server" />
</head>
<body>
    <form id="form1" runat="server" style="height:100%;">
    <div id="test">
    <remotion:TabbedMultiView ID="MyMultiView" runat="server">
      <TopControls>
        <h1>Tabbed multi-view theming test</h1>        
      </TopControls>
      <Views>
        <remotion:TabView ID="TabView1" runat="server" Title="First">
          <p>This is view no. 1</p>
        </remotion:TabView>
        <remotion:TabView ID="TabView2" runat="server" Title="Second">
          <p>This is view no. 2</p>
        </remotion:TabView>
        <remotion:TabView ID="TabView3" runat="server" Title="Person Details" Icon-Url="~/Images/Person.gif" Icon-Height="16" Icon-Width="16">
          <p>This is view no. 3</p>
        </remotion:TabView>
        <remotion:TabView ID="TabView4" runat="server" Title="Jobs" Icon-Url="~/Images/Job.gif" Icon-Height="16" Icon-Width="16">
          <p>This is view no. 4</p>
        </remotion:TabView>
        <remotion:TabView ID="TabView5" runat="server" Title="View 5">
          <p>This is view no. 52</p>
        </remotion:TabView>
      </Views>
      <BottomControls>
        <p>This is a test of the theming capabilities of the TabbedMultiViewControl.</p>
      </BottomControls>
    </remotion:TabbedMultiView>
    </div>
    </form>
</body>
</html>
