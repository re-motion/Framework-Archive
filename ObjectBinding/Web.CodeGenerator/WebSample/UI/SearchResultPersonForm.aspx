<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchResultPersonForm.aspx.cs" Inherits="WebSample.UI.SearchResultPersonForm" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="dow" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="uc1" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Search Person</title>
  <rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
</head>

<body>
  <dow:SearchObjectDataSourceControl id="PersonSearchDataSource" runat="server" TypeName="DomainSample.Person, DomainSample" Mode="Read" />
  <rubicon:FormGridManager id="FormGridManager" runat="server" />
  <uc1:NavigationTabs id="NavigationTabs" runat="server" />
  <form id="Form1" method="post" runat="server">
    <obw:BocList ID="PersonList" runat="server" DataSourceControl="PersonSearchDataSource">
			<FixedColumns>
				<obw:BocAllPropertiesPlacehoderColumnDefinition />
				<obw:BocCommandColumnDefinition Text="$res:Edit">
					<PersistedCommand>
						<obw:BocListItemCommand
								Type="WxeFunction"
								WxeFunctionCommand-Parameters="id"
								WxeFunctionCommand-TypeName="WebSample.WxeFunctions.EditPersonFunction,WebSample" />
					</PersistedCommand>
				</obw:BocCommandColumnDefinition>
			</FixedColumns>
    </obw:BocList>
  </form>
</body>

</html>
