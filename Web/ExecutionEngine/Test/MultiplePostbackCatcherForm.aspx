<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Page language="c#" Codebehind="MultiplePostbackCatcherForm.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.PageTransition.MultiplePostbackCatcherForm" smartNavigation="False"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>ClientForm</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
<script language=javascript>
</script>
<style type="text/css">
  TD.first { BACKGROUND-COLOR: lightgrey }
  </style>
</head>
<body MS_POSITIONING="FlowLayout">
<form id=Form method=post runat="server">
<table style="WIDTH: 100%; HEIGHT: 100%">
  <tr>
    <td style="VERTICAL-ALIGN: top">
      <table cellspacing="0" cellpadding="0">
        <tr>
          <td></td>
          <td colSpan=2>Input</td>
          <td colSpan=2>Button</td>
          <td colSpan=2>Anchor Text</td>
          <td colSpan=2>Anchor Image</td>
          <td colSpan=2>Anchor Span</td>
          <td colSpan=2>Anchor Label</td>
          <td colSpan=2>Anchor Bold</td>
          <td colSpan=2>Script</td>  
          </tr>
        <tr>
          <td></td>
          <td>Type= Submit</td>
          <td>Type= Button</td>
          <td>Type= Submit</td>
          <td>Type= Button</td>
          <td>Href=# OnClick</td>
          <td>Href= javascript</td>
          <td>Href=# OnClick</td>
          <td>Href= javascript</td>
          <td>Href=# OnClick</td>
          <td>Href= javascript</td>
          <td>Href=# OnClick</td>
          <td>Href= javascript</td>
          <td>Href=# OnClick</td>
          <td>Href= javascript</td>
          <td>Href=# OnClick</td>
          <td>Href= javascript</td></tr>
        <tr>
          <td class="first"><asp:button id=Button1 runat="server" Text="Submit"></asp:button></td>
          <td></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id=Button3 runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id=Button4 runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><A onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</A></td>
          <td><asp:linkbutton id=Button6 runat="server">Href</asp:linkbutton></td>
          <td><A onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></A></td>
          <td><asp:linkbutton id=Button8 runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><A onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></A></td>
          <td><asp:linkbutton id=Button10 runat="server"><span>Href</span></asp:linkbutton></td>
          <td><A onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></A></td>
          <td><asp:linkbutton id="Button12" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton1" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>
        <tr>
          <td class="first"><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><asp:button id="Button2" runat="server" Text="Submit"></asp:button></td>
          <td></td>
          <td><rubicon:webbutton id="Webbutton1" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton2" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton2" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton3" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton4" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton5" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton6" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><rubicon:webbutton id="Webbutton3" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><asp:button id="Button5" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td></td>
          <td><rubicon:webbutton id="Webbutton4" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton7" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton8" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton9" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton10" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton11" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><rubicon:webbutton id="Webbutton6" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><asp:button id="Button7" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton5" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton12" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton13" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton14" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton15" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton16" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr> 
          <tr><td colspan="17">&nbsp;</td></tr>         
        <tr>
          <td class="first"><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:button id="Button9" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton7" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton8" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td></td>
          <td><asp:linkbutton id="Linkbutton17" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton18" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton19" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton20" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton21" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><asp:linkbutton id="Linkbutton22" runat="server">Href</asp:linkbutton></td>
          <td><asp:button id="Button11" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton9" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton10" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton23" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton24" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton25" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton26" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><a 
            onclick="__doPostBack('Button8',''); return false;" href="#"><img alt=OnClick 
            ></a></td>
          <td><asp:button id="Button13" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton11" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton12" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton27" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ></a></td>
          <td><asp:linkbutton id="Linkbutton28" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton29" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton30" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton31" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><asp:linkbutton id="Linkbutton33" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><asp:button id="Button14" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton13" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton14" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton32" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton34" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton35" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton36" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
          <tr><td colspan="17">&nbsp;</td></tr>         
        <tr>
          <td class="first"><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:button id="Button15" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton15" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton16" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton37" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton38" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td></td>
          <td><asp:linkbutton id="Linkbutton39" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton40" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton41" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><asp:linkbutton id="Linkbutton62" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><asp:button id="Button20" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton25" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton26" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton63" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton64" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton65" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton66" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:button id="Button16" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton17" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton18" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton42" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton43" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton44" runat="server"><span>Href</span></asp:linkbutton></td>
          <td></td>
          <td><asp:linkbutton id="Linkbutton45" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton46" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><asp:linkbutton id="Linkbutton50" runat="server"><label title="Link does not work. At least in IE">Href</label></asp:linkbutton></td>
          <td><asp:button id="Button17" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton19" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton20" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton47" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton48" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton49" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:linkbutton id="Linkbutton51" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td><asp:button id="Button18" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton21" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton22" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton52" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton53" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton54" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton55" runat="server"><label>Href</label></asp:linkbutton></td>
          <td></td>
          <td><asp:linkbutton id="Linkbutton56" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        <tr>
          <td class="first"><asp:linkbutton id="Linkbutton61" runat="server"><b>H</b>ref</asp:linkbutton></td>
          <td><asp:button id="Button19" runat="server" Text="Submit"></asp:button></td>
          <td><input type=button value=Button name=Button2 onclick="__doPostBack('Button6',''); return false;"></td>
          <td><rubicon:webbutton id="Webbutton23" runat="server" Text="Submit"></rubicon:webbutton></td>
          <td><rubicon:webbutton id="Webbutton24" runat="server" Text="Button" UseSubmitBehavior="False"></rubicon:webbutton></td>
          <td><a onclick="__doPostBack('Button6',''); return false;" href="#" >OnClick</a></td>
          <td><asp:linkbutton id="Linkbutton57" runat="server">Href</asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button8',''); return false;" href="#" ><img alt=OnClick 
            ></a></td>
          <td><asp:linkbutton id="Linkbutton58" runat="server"><img alt="Href"></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button10',''); return false;" href="#" ><span 
            >OnClick</span></a></td>
          <td><asp:linkbutton id="Linkbutton59" runat="server"><span>Href</span></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><label>OnClick</label></a></td>
          <td><asp:linkbutton id="Linkbutton60" runat="server"><label>Href</label></asp:linkbutton></td>
          <td><a onclick="__doPostBack('Button12',''); return false;" href="#" ><b>On</b>Click</a></td>
          <td></td>
          <td><a href="#" onclick="alert('script in onclick'); return false;">OnClick</a></td>
          <td><a href="javascript:alert('script in href');">Href</a></td>
          </tr>          
        </table></td></tr></table></form>
  </body>
</html>
