<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ControlWithAllDataTypes.ascx.cs" Inherits="Rubicon.Data.DomainObjects.Web.Test.ControlWithAllDataTypes" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<rubicon:formgridmanager id="FormGridManager" runat="server" visible="true"></rubicon:formgridmanager>
<rubicon:domainobjectdatasourcecontrol id="CurrentObject" runat="server" TypeName="Rubicon.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Rubicon.Data.DomainObjects.Web.Test"></rubicon:domainobjectdatasourcecontrol>
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
    <TD><rubicon:bocbooleanvalue id="BocBooleanValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BooleanProperty"></rubicon:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocenumvalue id="BocEnumValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BooleanProperty">
        <ListControlStyle RadioButtonListCellPadding="" RadioButtonListCellSpacing=""></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue10" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ByteProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 25px"></TD>
    <TD style="HEIGHT: 25px"><rubicon:boctextvalue id="Boctextvalue12" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocdatetimevalue id="Bocdatetimevalue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateProperty"></rubicon:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 25px"></TD>
    <TD style="HEIGHT: 25px"><rubicon:boctextvalue id="Boctextvalue7" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateTimeProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocdatetimevalue id="BocDateTimeValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateTimeProperty"></rubicon:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocdatetimevalue id="BocDateTimeValue7" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ReadOnlyDateTimeProperty"></rubicon:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue16" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DecimalProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue15" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DoubleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocenumvalue id="BocEnumValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="EnumProperty">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD>
      <rubicon:bocenumvalue id="Bocenumvalue5" runat="server" PropertyIdentifier="EnumProperty" DataSourceControl="CurrentObject"
        ReadOnly="True">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue19" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="GuidProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue11" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Int16Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Int32Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue20" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="SingleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <td></td>
    <TD><rubicon:boctextvalue id="BocTextValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringProperty">
        <TextBoxStyle TextMode="MultiLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <td></td>
    <TD><rubicon:boctextvalue id="BocTextValue30" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringPropertyWithoutMaxLength">
        <TextBoxStyle TextMode="MultiLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:BocMultilineTextValue id="BocMultilineTextValue1" runat="server" PropertyIdentifier="StringArray" DataSourceControl="CurrentObject">
<TextBoxStyle TextMode="MultiLine">
</TextBoxStyle>
</rubicon:BocMultilineTextValue></TD></TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Nullable Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><rubicon:bocbooleanvalue id="BocBooleanValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanProperty"
        TrueDescription="Ja" FalseDescription="Nein" NullDescription="Undefiniert"></rubicon:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD style="HEIGHT: 12px"></TD>
    <TD style="HEIGHT: 12px"><rubicon:bocenumvalue id="BocEnumValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanProperty">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaByteProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue13" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocdatetimevalue id="Bocdatetimevalue5" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateProperty"></rubicon:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue8" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocdatetimevalue id="BocDateTimeValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeProperty"></rubicon:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue24" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDecimalProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue25" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDoubleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
<%--  <TR>
    <TD></TD>
    <TD><rubicon:bocenumvalue id="BocEnumValue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaEnumProperty">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD>
      <rubicon:bocenumvalue id="Bocenumvalue7" runat="server" PropertyIdentifier="NaEnumProperty" DataSourceControl="CurrentObject"
        ReadOnly="True">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
--%>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue27" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaGuidProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue22" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt16Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue5" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt32Property">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue28" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaSingleProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Nullable Types with null values</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><rubicon:bocbooleanvalue id="BocBooleanValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanWithNullValueProperty"
        TrueDescription="Ja" FalseDescription="Nein" NullDescription="Undefiniert"></rubicon:bocbooleanvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocenumvalue id="BocEnumValue4" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaBooleanWithNullValueProperty">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue21" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaByteWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue14" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateWithNullValueProperty"
        ValueType="Date">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocdatetimevalue id="Bocdatetimevalue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateWithNullValueProperty"></rubicon:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue9" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeWithNullValueProperty"
        ValueType="Date">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocdatetimevalue id="BocDateTimeValue3" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDateTimeWithNullValueProperty"></rubicon:bocdatetimevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue26" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDecimalWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue18" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaDoubleWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
<%--  <TR>
    <TD></TD>
    <TD><rubicon:bocenumvalue id="BocEnumValue8" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaEnumPropertyWithNullValueProperty">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD>
      <rubicon:bocenumvalue id="Bocenumvalue9" runat="server" PropertyIdentifier="NaEnumPropertyWithNullValueProperty" DataSourceControl="CurrentObject"
        ReadOnly="True">
        <ListControlStyle></ListControlStyle>
      </rubicon:bocenumvalue></TD>
  </TR>
--%>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue17" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaGuidWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue23" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt16WithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue6" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaInt32WithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="Boctextvalue29" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="NaSingleWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boctextvalue id="BocTextValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="StringWithNullValueProperty">
        <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
      </rubicon:boctextvalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:BocMultilineTextValue id="BocMultilineTextValue2" runat="server" PropertyIdentifier="NullStringArray" DataSourceControl="CurrentObject">
<TextBoxStyle TextMode="MultiLine">
</TextBoxStyle>
</rubicon:BocMultilineTextValue></TD></TR>
  <tr>
    <td></td>
    <td><br>
      <h2>Reference Types</h2>
    </td>
  </tr>
  <TR>
    <TD></TD>
    <TD><rubicon:bocreferencevalue id="BocReferenceValue1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassForRelationTestMandatory"
        Select="GetAllRelatedObjects">
        <PersistedCommand>
          <rubicon:BocCommand Type="None"></rubicon:BocCommand>
        </PersistedCommand>
      </rubicon:bocreferencevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:bocreferencevalue id="BocReferenceValue2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassForRelationTestOptional"
        Select="GetAllRelatedObjects">
        <PersistedCommand>
          <rubicon:BocCommand Type="None"></rubicon:BocCommand>
        </PersistedCommand>
      </rubicon:bocreferencevalue></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boclist id="BocList1" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassesForRelationTestMandatoryNavigateOnly">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
          <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
            <PersistedCommand>
              <rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
      </rubicon:boclist></TD>
  </TR>
  <TR>
    <TD></TD>
    <TD><rubicon:boclist id="BocList2" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="ClassesForRelationTestOptionalNavigateOnly">
        <FixedColumns>
          <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="DisplayName">
            <PersistedCommand>
              <rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
          <rubicon:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
            <PersistedCommand>
              <rubicon:BocListItemCommand Type="None"></rubicon:BocListItemCommand>
            </PersistedCommand>
          </rubicon:BocSimpleColumnDefinition>
        </FixedColumns>
      </rubicon:boclist></TD>
  </TR>
</TABLE>
<P><br>
  <asp:button id="SaveButton" Text="Speichern" Runat="server"></asp:button></P>

