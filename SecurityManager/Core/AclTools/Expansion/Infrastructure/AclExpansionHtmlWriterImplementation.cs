/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Linq;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.Infrastructure
{
  public class AclExpansionHtmlWriterImplementation : AclExpansionHtmlWriterImplementationBase
  {
    private readonly AclExpansionHtmlWriterSettings _settings;
    private string _statelessAclStateHtmlText = "(stateless)";
    private string _aclWithNoAssociatedStatesHtmlText = "(no associated states)";

    public AclExpansionHtmlWriterImplementation (TextWriter textWriter, bool indentXml, AclExpansionHtmlWriterSettings settings)
        : base (textWriter, indentXml)
    {
      _settings = settings;
    }

    public string StatelessAclStateHtmlText
    {
      get { return _statelessAclStateHtmlText; }
      set { _statelessAclStateHtmlText = value; }
    }
    
    public string AclWithNoAssociatedStatesHtmlText
    {
      get { return _aclWithNoAssociatedStatesHtmlText; }
      set { _aclWithNoAssociatedStatesHtmlText = value; }
    }


    private void WriteTableDataAddendum (object addendum) 
    {
      if (addendum != null)
      {
        HtmlTagWriter.Value (" (");
        HtmlTagWriter.Value (addendum);
        HtmlTagWriter.Value (") ");
      }
    }


    public void WriteTableDataWithRowCount (string value, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow ();
      HtmlTagWriter.Tags.td ();
      WriteRowspanAttribute(rowCount);
      HtmlTagWriter.Value (value);
      if (_settings.OutputRowCount)
      { 
        WriteTableDataAddendum (rowCount);
      }
      HtmlTagWriter.Tags.tdEnd ();
    }


    private void WriteRowspanAttribute (int rowCount)
    {
      if (rowCount > 0)
      { 
        HtmlTagWriter.Attribute ("rowspan", Convert.ToString (rowCount));
      }
    }

    public void WriteTableDataForRole (Role role, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow();
      HtmlTagWriter.Tags.td ();
      WriteRowspanAttribute (rowCount);
      HtmlTagWriter.Value (role.Group.DisplayName);
      HtmlTagWriter.Value (", ");
      HtmlTagWriter.Value (role.Position.DisplayName);
      if (_settings.OutputRowCount)
      { 
        WriteTableDataAddendum (rowCount);
      }
      HtmlTagWriter.Tags.tdEnd ();
    }


    public void WriteTableDataBodyForSingleState (AclExpansionEntry aclExpansionEntry)
    {
      if (aclExpansionEntry.AccessControlList is StatelessAccessControlList)
      {
        HtmlTagWriter.Value (StatelessAclStateHtmlText);
      }
      else
      {
        IOrderedEnumerable<StateDefinition> stateDefinitions = GetAllStatesForAclExpansionEntry(aclExpansionEntry);

        if (!stateDefinitions.Any ())
        { 
          HtmlTagWriter.Value (AclWithNoAssociatedStatesHtmlText);
        }
        else
        {
          bool firstElement = true;
          foreach (StateDefinition stateDefiniton in stateDefinitions)
          {
            if (!firstElement)
            {
              HtmlTagWriter.Value (", ");
            }

            string stateName = _settings.ShortenNames ? stateDefiniton.ShortName () : stateDefiniton.DisplayName;

            HtmlTagWriter.Value (stateName);
            firstElement = false;
          }
        }
      }
    }

    private IOrderedEnumerable<StateDefinition> GetAllStatesForAclExpansionEntry (AclExpansionEntry aclExpansionEntry)
    {
      // Get all states for AclExpansionEntry by flattening the StateCombinations of the AccessControlEntry ACL.
      return aclExpansionEntry.GetStateCombinations().SelectMany (x => x.GetStates ()).OrderBy (x => x.DisplayName);
    }


    public void WriteTableDataForAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      var accessTypeDefinitionsSorted = from atd in accessTypeDefinitions
                                        orderby atd.DisplayName
                                        select atd;

      HtmlTagWriter.Tags.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitionsSorted)
      {
        if (!firstElement)
        { 
          HtmlTagWriter.Value (", ");
        }
        HtmlTagWriter.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      HtmlTagWriter.Tags.tdEnd ();
    }

    public void WriteTableDataForBodyConditions (AclExpansionAccessConditions accessConditions)
    {
      WriteTableDataForBooleanCondition (accessConditions.IsOwningUserRequired);
      WriteTableDataForOwningGroupCondition (accessConditions);
      WriteTableDataForOwningTenantCondition (accessConditions);
      WriteTableDataForAbstractRoleCondition(accessConditions);
    }

    private void WriteTableDataForAbstractRoleCondition (AclExpansionAccessConditions accessConditions)
    {
      HtmlTagWriter.Tags.td ();
      HtmlTagWriter.Value (accessConditions.IsAbstractRoleRequired ? accessConditions.AbstractRole.DisplayName : "");
      HtmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForOwningGroupCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.GroupHierarchyCondition == GroupHierarchyCondition.Undefined && conditions.OwningGroup != null);
      HtmlTagWriter.Tags.td();
      HtmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningGroup = conditions.OwningGroup;
      if (owningGroup != null)
      { 
        HtmlTagWriter.Value (owningGroup.DisplayName);
      }

      var groupHierarchyCondition = conditions.GroupHierarchyCondition;

      // Bitwise operation is OK (alas marking GroupHierarchyCondition with [Flags] is not supported). 
      if ((groupHierarchyCondition & GroupHierarchyCondition.Parent) != 0)
      {
        HtmlTagWriter.Tags.br ();
        HtmlTagWriter.Value ("or its parents");
      }

      // Bitwise operation is OK (alas marking GroupHierarchyCondition with [Flags] is not supported). 
      if ((groupHierarchyCondition & GroupHierarchyCondition.Children) != 0)
      {
        HtmlTagWriter.Tags.br ();
        HtmlTagWriter.Value ("or its children");
      }

      HtmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForOwningTenantCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.TenantHierarchyCondition == TenantHierarchyCondition.Undefined && conditions.OwningTenant != null);
      HtmlTagWriter.Tags.td ();
      HtmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningTenant = conditions.OwningTenant;
      if (owningTenant != null)
      { 
        HtmlTagWriter.Value (owningTenant.DisplayName);
      }

      var tenantHierarchyCondition = conditions.TenantHierarchyCondition;
      // Bitwise operation is OK (alas marking TenantHierarchyCondition with [Flags] is not supported). 
      if ((tenantHierarchyCondition & TenantHierarchyCondition.Parent) != 0)
      {
        HtmlTagWriter.Tags.br ();
        HtmlTagWriter.Value ("or its parents");
      }

      HtmlTagWriter.Tags.tdEnd ();
    }


    public void WriteTableDataForBooleanCondition (bool required)
    {
      HtmlTagWriter.Tags.td ();
      HtmlTagWriter.Value (required ? "X" : ""); 
      HtmlTagWriter.Tags.tdEnd ();
    }

  }
}