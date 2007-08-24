<%@ Control Language="c#" AutoEventWireup="True" Codebehind="BocAutoCompleteReferenceValueUserControl.ascx.cs" Inherits="OBWTest.IndividualControlTests.BocAutoCompleteReferenceValueUserControl"
  TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<rubicon:FormGridManager ID="FormGridManager" runat="server" />
<rubicon:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Rubicon.ObjectBinding.Sample::Person"/>
<table id="FormGrid" runat="server">
  <tr>
    <td colspan="4">
      <rubicon:BocTextValue ID="FirstNameField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="FirstName">
      </rubicon:BocTextValue>
      &nbsp;<rubicon:BocTextValue ID="LastNameField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="LastName">
      </rubicon:BocTextValue>
    </td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="PartnerField" runat="server" ReadOnly="False" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Width="75%" ServiceMethod="GetPersonList" ServicePath="~/IndividualControlTests/AutoCompleteService.asmx" args="Test">
        <TextBoxStyle AutoPostBack="True" />
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      bound, 75%</td>
    <td style="width: 20%">
      <asp:Label ID="PartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="ReadOnlyPartnerField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Width="75%">
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      bound, read-only, 75%</td>
    <td style="width: 20%">
      <asp:Label ID="ReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="UnboundPartnerField" runat="server" Required="True" Width="15em" ServiceMethod="GetPersonList" 
      ServicePath="~/IndividualControlTests/AutoCompleteService.asmx">
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        unbound, value not set, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="UnboundPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="UnboundReadOnlyPartnerField" runat="server" ReadOnly="True" Width="15em">
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        unbound, value set, read only, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="UnboundReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="DisabledPartnerField" runat="server" ReadOnly="False" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Enabled="False" Width="75%" >
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      disabled, bound, 75%</td>
    <td style="width: 20%">
      <asp:Label ID="DisabledPartnerFieldValueLabel" runat="server" EnableViewState="False" >#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="DisabledReadOnlyPartnerField" runat="server" ReadOnly="True" DataSourceControl="CurrentObject" PropertyIdentifier="Partner"
        Enabled="False">
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      disabled, bound, read-only</td>
    <td style="width: 20%">
      <asp:Label ID="DisabledReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="DisabledUnboundPartnerField" runat="server" Required="True" Enabled="False" Width="15em">
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        disabled, unbound, value set, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="DisabledUnboundPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
  <tr>
    <td>
    </td>
    <td>
      <rubicon:BocAutoCompleteReferenceValue ID="DisabledUnboundReadOnlyPartnerField" runat="server" ReadOnly="True" Enabled="False" Width="15em">
      </rubicon:BocAutoCompleteReferenceValue>
    </td>
    <td>
      <p>
        disabled, unbound, value set, read only, 15em</p>
    </td>
    <td style="width: 20%">
      <asp:Label ID="DisabledUnboundReadOnlyPartnerFieldValueLabel" runat="server" EnableViewState="False">#</asp:Label></td>
  </tr>
</table>
<p>
  Partner Command Click:
  <asp:Label ID="PartnerCommandClickLabel" runat="server" EnableViewState="False">#</asp:Label></p>
<p>
  Partner Selection Changed:
  <asp:Label ID="PartnerFieldSelectionChangedLabel" runat="server" EnableViewState="False">#</asp:Label></p>
<p>
  Partner Menu Click:
  <asp:Label ID="PartnerFieldMenuClickEventArgsLabel" runat="server" EnableViewState="False">#</asp:Label></p>
<p>
  <br>
  <rubicon:WebButton ID="PartnerTestSetNullButton" runat="server" Text="Partner Set Null" />
  <rubicon:WebButton ID="PartnerTestSetNewItemButton" runat="server" Text="Partner Set New Item" />
</p>
<p>
  <rubicon:WebButton ID="ReadOnlyPartnerTestSetNullButton" runat="server" Text="Read Only Partner Set Null" />
  <rubicon:WebButton ID="ReadOnlyPartnerTestSetNewItemButton" runat="server" Text="Read Only Partner Set New Item" />
</p>
