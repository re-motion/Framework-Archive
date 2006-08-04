<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResult$DOMAIN_CLASSNAME$Form.aspx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.SearchResult$DOMAIN_CLASSNAME$Form" %>
<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="dow" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="uc1" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">

<head>
  <title>$res:$DOMAIN_CLASSNAME$</title>
  <rubicon:htmlheadcontents id="HtmlHeadContents" runat="server"></rubicon:htmlheadcontents>
</head>

<body>
  <dow:SearchObjectDataSourceControl id="$DOMAIN_CLASSNAME$SearchDataSource" runat="server" TypeName="$DOMAIN_QUALIFIEDCLASSTYPENAME$" Mode="Read" />
  <rubicon:FormGridManager id="FormGridManager" runat="server" />
  <uc1:NavigationTabs id="NavigationTabs" runat="server" />
  <form id="Form1" method="post" runat="server">
    <obw:BocList ID="$DOMAIN_CLASSNAME$List" runat="server" DataSourceControl="$DOMAIN_CLASSNAME$SearchDataSource" OnListItemCommandClick="$DOMAIN_CLASSNAME$List_ListItemCommandClick">
      <FixedColumns>
        <obw:BocAllPropertiesPlacehoderColumnDefinition />
        <obw:BocCommandColumnDefinition ItemID="Edit" Text="$res:Edit">
          <PersistedCommand>
            <obw:BocListItemCommand Type="Event" />
          </PersistedCommand>
        </obw:BocCommandColumnDefinition>
      </FixedColumns>
    </obw:BocList>
  </form>
</body>

</html>
