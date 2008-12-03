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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;


namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// <see cref="IAclExpansionWriter"/> which outputs a <see cref="List{T}"/> of <see cref="AclExpansionEntry"/>
  /// as a single HTML table.
  /// </summary>
  // TODO AE: Globalization is not supported for hard-coded strings. Is this a problem for this application?
  // TODO AE: Split this class! Extract most of the private methods to another class to improve clarity (and to allow for more fine-grained testability).
  public class AclExpansionHtmlWriter : AclExpansionHtmlWriterBase
  {
    private AclExpansionHtmlWriterSettings _settings = new AclExpansionHtmlWriterSettings ();
    private string _statelessAclStateHtmlText = "(stateless)";
    private string _aclWithNoAssociatedStatesHtmlText = "(no associated states)";

    public AclExpansionHtmlWriter (TextWriter textWriter, bool indentXml)
    {
      htmlTagWriter = new HtmlTagWriter (textWriter, indentXml);
    }
   

    public AclExpansionHtmlWriterSettings Settings
    {
      get { return _settings; }
      set { _settings = value; }
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


    public override void WriteAclExpansion (List<AclExpansionEntry> aclExpansion)
    {
      ArgumentUtility.CheckNotNull ("aclExpansion", aclExpansion);
    
      var aclExpansionTree = new AclExpansionTree (aclExpansion);

      WritePageStart ("re-motion ACL Expansion");

      WriteTableStart ("remotion-ACL-expansion-table");
      WriteTableHeaders ();
      WriteTableBody (aclExpansionTree);
      WriteTableEnd ();

      WritePageEnd ();
    }


    private void WriteTableHeaders ()
    {
      htmlTagWriter.Tags.tr ();
      WriteHeaderCell ("User");
      WriteHeaderCell ("Role");
      WriteHeaderCell ("Class");
      WriteHeaderCell ("States");
      WriteHeaderCell ("User Must Own");
      WriteHeaderCell ("Owning Group Equals");
      WriteHeaderCell ("Owning Tenant Equals");
      WriteHeaderCell ("User Must Have Abstract Role");
      WriteHeaderCell ("Access Rights");
      if (Settings.OutputDeniedRights)
      { 
        WriteHeaderCell ("Denied Rights");
      }
      htmlTagWriter.Tags.trEnd ();
    }



    private void WriteTableDataAddendum (object addendum) 
    {
      if (addendum != null)
      {
        htmlTagWriter.Value (" (");
        htmlTagWriter.Value (addendum);
        htmlTagWriter.Value (") ");
      }
    }


    private void WriteTableDataWithRowCount (string value, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow ();
      htmlTagWriter.Tags.td ();
      WriteRowspanAttribute(rowCount);
      htmlTagWriter.Value (value);
      if (Settings.OutputRowCount)
      { 
        WriteTableDataAddendum (rowCount);
      }
      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteRowspanAttribute (int rowCount)
    {
      if (rowCount > 0)
      { 
        htmlTagWriter.Attribute ("rowspan", Convert.ToString (rowCount));
      }
    }

    private void WriteTableDataForRole (Role role, int rowCount)
    {
      WriteTableRowBeginIfNotInTableRow();
      htmlTagWriter.Tags.td ();
      WriteRowspanAttribute (rowCount);
      htmlTagWriter.Value (role.Group.DisplayName);
      htmlTagWriter.Value (", ");
      htmlTagWriter.Value (role.Position.DisplayName);
      if (Settings.OutputRowCount)
      { 
        WriteTableDataAddendum (rowCount);
      }
      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataBodyForSingleState (AclExpansionEntry aclExpansionEntry)
    {
      if (aclExpansionEntry.AccessControlList is StatelessAccessControlList)
      {
        htmlTagWriter.Value (StatelessAclStateHtmlText);
      }
      else
      {
        IOrderedEnumerable<StateDefinition> stateDefinitions = GetAllStatesForAclExpansionEntry(aclExpansionEntry);

        if (!stateDefinitions.Any ())
        { 
          htmlTagWriter.Value (AclWithNoAssociatedStatesHtmlText);
        }
        else
        {
          bool firstElement = true;
          foreach (StateDefinition stateDefiniton in stateDefinitions)
          {
            if (!firstElement)
            {
              htmlTagWriter.Value (", ");
            }

            string stateName = Settings.ShortenNames ? stateDefiniton.ShortName() : stateDefiniton.DisplayName;

            htmlTagWriter.Value (stateName);
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

    private void WriteTableDataForAccessTypes (AccessTypeDefinition[] accessTypeDefinitions)
    {
      var accessTypeDefinitionsSorted = from atd in accessTypeDefinitions
                                        orderby atd.DisplayName
                                        select atd;

      htmlTagWriter.Tags.td ();
      bool firstElement = true;
      foreach (AccessTypeDefinition accessTypeDefinition in accessTypeDefinitionsSorted)
      {
        if (!firstElement)
        { 
          htmlTagWriter.Value (", ");
        }
        htmlTagWriter.Value (accessTypeDefinition.DisplayName);
        firstElement = false;
      }
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForBodyConditions (AclExpansionAccessConditions accessConditions)
    {
      WriteTableDataForBooleanCondition (accessConditions.IsOwningUserRequired);
      WriteTableDataForOwningGroupCondition (accessConditions);
      WriteTableDataForOwningTenantCondition (accessConditions);
      WriteTableDataForAbstractRoleCondition(accessConditions);
    }

    private void WriteTableDataForAbstractRoleCondition (AclExpansionAccessConditions accessConditions)
    {
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (accessConditions.IsAbstractRoleRequired ? accessConditions.AbstractRole.DisplayName : "");
      htmlTagWriter.Tags.tdEnd ();
    }

    private void WriteTableDataForOwningGroupCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.GroupHierarchyCondition == GroupHierarchyCondition.Undefined && conditions.OwningGroup != null);
      htmlTagWriter.Tags.td();
      htmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningGroup = conditions.OwningGroup;
      if (owningGroup != null)
      { 
        htmlTagWriter.Value (owningGroup.DisplayName);
      }

      var groupHierarchyCondition = conditions.GroupHierarchyCondition;

      // Bitwise operation is OK (alas marking GroupHierarchyCondition with [Flags] is not supported). 
      if ((groupHierarchyCondition & GroupHierarchyCondition.Parent) != 0)
      {
        htmlTagWriter.Tags.br ();
        htmlTagWriter.Value ("or its parents");
      }

      // Bitwise operation is OK (alas marking GroupHierarchyCondition with [Flags] is not supported). 
      if ((groupHierarchyCondition & GroupHierarchyCondition.Children) != 0)
      {
        htmlTagWriter.Tags.br ();
        htmlTagWriter.Value ("or its children");
      }

      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForOwningTenantCondition (AclExpansionAccessConditions conditions)
    {
      Assertion.IsFalse (conditions.TenantHierarchyCondition == TenantHierarchyCondition.Undefined && conditions.OwningTenant != null);
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (""); // To force <td></td> instead of <td />
      var owningTenant = conditions.OwningTenant;
      if (owningTenant != null)
      { 
        htmlTagWriter.Value (owningTenant.DisplayName);
      }

      var tenantHierarchyCondition = conditions.TenantHierarchyCondition;
      // Bitwise operation is OK (alas marking TenantHierarchyCondition with [Flags] is not supported). 
      if ((tenantHierarchyCondition & TenantHierarchyCondition.Parent) != 0)
      {
        htmlTagWriter.Tags.br ();
        htmlTagWriter.Value ("or its parents");
      }

      htmlTagWriter.Tags.tdEnd ();
    }


    private void WriteTableDataForBooleanCondition (bool required)
    {
      htmlTagWriter.Tags.td ();
      htmlTagWriter.Value (required ? "X" : ""); // TODO AE: Test missing for one of these cases
      htmlTagWriter.Tags.tdEnd ();
    }



    private void WriteTableBody (AclExpansionTree aclExpansionTree)
    {
      foreach (var userNode in aclExpansionTree.Tree)
      {
        WriteTableBody_ProcessUser(userNode);
      }
    }

    private void WriteTableBody_ProcessUser (AclExpansionTreeNode<User, AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>> userNode)
    {
      WriteTableDataWithRowCount (userNode.Key.DisplayName, userNode.NumberLeafNodes);
  
      foreach (var roleNode in userNode.Children)
      {
        WriteTableBody_ProcessRole(roleNode);
      }
    }

    private void WriteTableBody_ProcessRole (AclExpansionTreeNode<Role, AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>> roleNode)
    {
      WriteTableDataForRole (roleNode.Key, roleNode.NumberLeafNodes);
 
      foreach (var classNode in roleNode.Children)
      {
        WriteTableBody_ProcessClass(classNode);
      }
    }

    private void WriteTableBody_ProcessClass (AclExpansionTreeNode<SecurableClassDefinition, AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> classNode)
    {
      if (classNode.Key != null)
      {
        string className = Settings.ShortenNames ? classNode.Key.ShortName () : classNode.Key.DisplayName;
        WriteTableDataWithRowCount (className, classNode.NumberLeafNodes);
      }
      else
      {
        WriteTableDataWithRowCount ("_NO_CLASSES_DEFINED_", classNode.NumberLeafNodes);
      }

      WriteTableBody_ProcessStates(classNode.Children);
    }


    private void WriteTableBody_ProcessStates (IList<AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>> states)
    {
      // States Output
      foreach (var aclExpansionTreeNode in states)
      {
        WriteTableRowBeginIfNotInTableRow ();

        // Write all states combined into one cell
        WriteTableDataForStates (aclExpansionTreeNode.Children);

        AclExpansionEntry aclExpansionEntry = aclExpansionTreeNode.Key;
        WriteTableDataForBodyConditions (aclExpansionEntry.AccessConditions);
        WriteTableDataForAccessTypes (aclExpansionEntry.AllowedAccessTypes);
        if (Settings.OutputDeniedRights)
        {
          WriteTableDataForAccessTypes (aclExpansionEntry.DeniedAccessTypes);
        }

        WriteTableRowEnd ();
      }
    }


    private void WriteTableDataForStates (IList<AclExpansionEntry> aclExpansionEntriesWhichOnlyDiffersInStates)
    {
      htmlTagWriter.Tags.td ();

      bool firstElement = true;

      foreach (AclExpansionEntry aclExpansionEntry in aclExpansionEntriesWhichOnlyDiffersInStates)
      {
        if (!firstElement)
        {
          htmlTagWriter.Value ("; ");
        }

        WriteTableDataBodyForSingleState (aclExpansionEntry);
        firstElement = false;
      }
      htmlTagWriter.Tags.tdEnd ();
    }

  }
}



