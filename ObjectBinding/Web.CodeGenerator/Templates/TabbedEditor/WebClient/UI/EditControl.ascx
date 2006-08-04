<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit$DOMAIN_CLASSNAME$Control.ascx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.Edit$DOMAIN_CLASSNAME$Control" %>
<%@ Register Assembly="Rubicon.ObjectBinding.Web" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" TagPrefix="obw" %>
<%@ Register Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" TagPrefix="dow" %>
<%@ Register Assembly="Rubicon.Web" Namespace="Rubicon.Web.UI.Controls" TagPrefix="rubicon" %>

<rubicon:FormGridManager ID="FormGridManager" runat="server" />
<dow:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="$DOMAIN_QUALIFIEDCLASSTYPENAME$" />

<div>
  <table id="$DOMAIN_CLASSNAME$FormGrid" runat="server">
  $REPEAT_FOREACHPROPERTY_BEGIN$
  <tr>
    <td><rubicon:SmartLabel ID="$DOMAIN_PROPERTYNAME$Label" runat="server" ForControl="$DOMAIN_PROPERTYNAME$Field" /></td>
    <td>
      <$CONTROLTYPE$ ID="$DOMAIN_PROPERTYNAME$Field" runat="server" DataSourceControl="CurrentObject"
          PropertyIdentifier="$DOMAIN_PROPERTYNAME$" $ADDITIONALATTRIBUTES$>
        $ADDITIONALELEMENTS$
      </$CONTROLTYPE$>
    </td>
  </tr>$REPEAT_FOREACHPROPERTY_END$
</table>

</div>
