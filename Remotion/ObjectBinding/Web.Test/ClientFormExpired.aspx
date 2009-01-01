<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % version 3.0 as published by the Free Software Foundation.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page language="c#" Codebehind="ClientFormExpired.aspx.cs" AutoEventWireup="false" Inherits="ClientFormExpired" %>
<%@ Import namespace="OBWTest"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>ClientFormExpired</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
<script language="javascript">
  var _expiredLocation = 'WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormClosingWxeFunction,OBWTest';
  
  function OnUnload()
  {
    window.document.location = _expiredLocation;
  }
  function OnBeforeUnload()
  {
    var isOutsideClientLeft = event.clientX < 0;
    var isOutsideClientTop = event.clientY < 0;
    var isOutsideClientRight = event.clientX > (window.document.body.clientLeft + window.document.body.clientWidth);
    var isOutsideClientBottom = event.clientY > (window.document.body.clientTop + window.document.body.clientHeight);
    var isOutsideClient = isOutsideClientLeft || isOutsideClientTop || isOutsideClientRight || isOutsideClientBottom;
    
    if (isOutsideClient)
      window.document.body.onunload = OnUnload;
  }
</script>
  </head>
<body MS_POSITIONING="FlowLayout" onBeforeUnload="OnBeforeUnload();" onkeydown="OnKeyDown();">
	
    <form id="Form" method="post" runat="server">
<h1> Client Form has expired. </h1>
     </form>
	
  </body>
</html>
