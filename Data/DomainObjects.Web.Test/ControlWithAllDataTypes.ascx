<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="dob" Namespace="Rubicon.Data.DomainObjects.ObjectBinding.Web" Assembly="Rubicon.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ControlWithAllDataTypes.ascx.cs" Inherits="Rubicon.Data.DomainObjects.Web.Test.ControlWithAllDataTypes" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>



<P><STRONG><FONT color="#ff3333">Achtung: Auf dieser Seite befinden sich Controls, 
die mehrfach auf die gleiche Porperty gebunden sind. Dadurch überschreiben sich 
diese gegenseitig beim Zurückspeichern der Werte. Dies bitte bei Tests 
beachten!</FONT></STRONG></P>
<TABLE id="FormGrid" cellSpacing="0" cellPadding="0" border="0" runat="server">
  <tr>
    <td></td>
    <td><h2>Standard Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><obc:bocbooleanvalue id="BocBooleanValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BooleanProperty"></obc:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocenumvalue id="BocEnumValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BooleanProperty">
        <ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding=""></ListControlStyle>
      </obc:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue10" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ByteProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 25px"></TD>
    <TD style="HEIGHT: 25px"><obc:boctextvalue id="Boctextvalue12" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocdatetimevalue id="Bocdatetimevalue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateProperty"></obc:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 25px"></TD>
    <TD style="HEIGHT: 25px"><obc:boctextvalue id="Boctextvalue7" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateTimeProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocdatetimevalue id="BocDateTimeValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateTimeProperty"></obc:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocdatetimevalue id="BocDateTimeValue7" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ReadOnlyDateTimeProperty"></obc:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue16" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DecimalProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue15" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DoubleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocenumvalue id="BocEnumValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="EnumProperty">
        <ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding=""></ListControlStyle>
      </obc:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD>
      <obc:bocenumvalue id="Bocenumvalue5" runat="server" PropertyIdentifier="EnumProperty" DataSourceControl="CurrentObject"
        ReadOnly="True">
        <ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding=""></ListControlStyle>
      </obc:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue19" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="GuidProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue11" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Int16Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Int32Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue20" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="SingleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <td></td>
    <TD><obc:boctextvalue id="BocTextValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringProperty">
        <TextBoxStyle TextMode="MultiLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Nullable Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><obc:bocbooleanvalue id="BocBooleanValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanProperty"
        TrueDescription="Ja" FalseDescription="Nein" NullDescription="Undefiniert"></obc:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 12px"></TD>
    <TD style="HEIGHT: 12px"><obc:bocenumvalue id="BocEnumValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanProperty">
        <ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding=""></ListControlStyle>
      </obc:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaByteProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue13" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocdatetimevalue id="Bocdatetimevalue5" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateProperty"></obc:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue8" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocdatetimevalue id="BocDateTimeValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeProperty"></obc:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue24" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDecimalProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue25" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDoubleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue27" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaGuidProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue22" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt16Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue5" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt32Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue28" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaSingleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Nullable Types with null values</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><obc:bocbooleanvalue id="BocBooleanValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanWithNullValueProperty"
        TrueDescription="Ja" FalseDescription="Nein" NullDescription="Undefiniert"></obc:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocenumvalue id="BocEnumValue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanWithNullValueProperty">
        <ListControlStyle RadioButtonListCellSpacing="" RadioButtonListCellPadding=""></ListControlStyle>
      </obc:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue21" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaByteWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue14" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateWithNullValueProperty"
        ValueType="Date">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocdatetimevalue id="Bocdatetimevalue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateWithNullValueProperty"></obc:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue9" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeWithNullValueProperty"
        ValueType="Date">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocdatetimevalue id="BocDateTimeValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeWithNullValueProperty"></obc:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue26" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDecimalWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue18" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDoubleWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue17" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaGuidWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue23" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt16WithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt32WithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="Boctextvalue29" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaSingleWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boctextvalue id="BocTextValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </obc:boctextvalue></TD>
  </TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Reference Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><obc:bocreferencevalue id="BocReferenceValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassForRelationTestMandatory"
        Select="GetAllRelatedObjects">
        <PersistedCommand>
          <obc:BocCommand Type="None"></obc:BocCommand>
        </PersistedCommand>
      </obc:bocreferencevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:bocreferencevalue id="BocReferenceValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassForRelationTestOptional"
        Select="GetAllRelatedObjects">
        <PersistedCommand>
          <obc:BocCommand Type="None"></obc:BocCommand>
        </PersistedCommand>
      </obc:bocreferencevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boclist id="BocList1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassesForRelationTestMandatoryNavigateOnly">
        <FixedColumns>
          <obc:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
            </PersistedCommand>
          </obc:BocSimpleColumnDefinition>
          <obc:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
            <PersistedCommand>
              <obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
            </PersistedCommand>
          </obc:BocSimpleColumnDefinition>
        </FixedColumns>
      </obc:boclist></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><obc:boclist id="BocList2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassesForRelationTestOptionalNavigateOnly">
        <FixedColumns>
          <obc:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
            </PersistedCommand>
          </obc:BocSimpleColumnDefinition>
          <obc:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
            <PersistedCommand>
              <obc:BocListItemCommand Type="None"></obc:BocListItemCommand>
            </PersistedCommand>
          </obc:BocSimpleColumnDefinition>
        </FixedColumns>
      </obc:boclist></TD>
  </TR>
</TABLE>
<P><br>
  <asp:button id="SaveButton" Text="Speichern" Runat="server"></asp:button></P>
<P><rwc:formgridmanager id="FormGridManager" runat="server" visible="true"></rwc:formgridmanager><dob:domainobjectdatasourcecontrol id="CurrentObject" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Rubicon.Data.DomainObjects.Web.Test"></dob:domainobjectdatasourcecontrol></P>
