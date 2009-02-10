// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionTreeTest : AclToolsTestBase
  {
    [Test]
    public void SingleAclSingleUserExpansionTest ()
    {
      using (new CultureScope ("de-DE"))
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.ListMother.New (User);

        var acls = ListMother.New<AccessControlList> (Acl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList (users, acls, false);

        //WriteAclExpansionAsHtmlToDisk(aclExpansion, "c:\\temp\\aaa.html");

        var aclExpansionTree = new AclExpansionTree (aclExpansion);

        //WriteAclExpansionTreeToConsole(aclExpansionTree);

        var userNodes = aclExpansionTree.Tree;
        Assert.That (userNodes.Count, Is.EqualTo (1)); // # users
        Assert.That (userNodes[0].Key, Is.EqualTo (User));
        
        var roleNodes = userNodes[0].Children;
        Assert.That (roleNodes.Count, Is.EqualTo (1)); // # roles
        Assert.That (roleNodes[0].Key, Is.EqualTo (User.Roles[0]));
        
        var classNodes = roleNodes[0].Children;
        Assert.That (classNodes.Count, Is.EqualTo (1)); // # classes
        Assert.That (classNodes[0].Key.StatefulAccessControlLists, List.Contains (Acl));

        var stateNodes = classNodes[0].Children;
        Assert.That (stateNodes.Count, Is.EqualTo (2)); // # states

        Assert.That (stateNodes[0].Children.Count, Is.EqualTo (2)); // # states in group with same AclExpansionEntry ignoring StateCombinations
        Assert.That (stateNodes[1].Children.Count, Is.EqualTo (1)); // # states in group with same AclExpansionEntry ignoring StateCombinations

        foreach (var aclExpansionEntryTreeNode in stateNodes)
        {
          foreach (AclExpansionEntry aee in aclExpansionEntryTreeNode.Children)
          {
            Assert.That (aee.GetStateCombinations(), Is.SubsetOf (Acl.StateCombinations));
          }
        }
      }
    }


    [Test]
    public void StatelessAccessControlListSortOrderTest ()
    {
      using (new CultureScope ("de-DE"))
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.ListMother.New (User);
        
        var statelessAcl = CreateStatelessAcl (Ace);
        var acls = ListMother.New (Acl, statelessAcl);

        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList (users, acls, false);

        var aclExpansionTreeInverseSorted = new AclExpansionTree (
            aclExpansionEntryList,
            (classEntry => (classEntry.AccessControlList is StatefulAccessControlList) ? "A" : "B")); // sort stateful before stateless
        Assert.That (aclExpansionTreeInverseSorted.Tree[0].Children[0].Children.Count, Is.EqualTo (2));
        Assert.That (aclExpansionTreeInverseSorted.Tree[0].Children[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (Acl));

        var aclExpansionTreeDefaultSorted = new AclExpansionTree (aclExpansionEntryList);
        Assert.That (aclExpansionTreeDefaultSorted.Tree[0].Children[0].Children.Count, Is.EqualTo (2));
        Assert.That (aclExpansionTreeDefaultSorted.Tree[0].Children[0].Children[0].Children[0].Children[0].AccessControlList, Is.EqualTo (statelessAcl));

      }
    }


    [Test]
    public void AclExpansionEntryIgnoreStateEqualityComparerTest ()
    {
      var comparer = AclExpansionTree.AclExpansionEntryIgnoreStateEqualityComparer;
      var aclExpansionEntry1 =
          new AclExpansionEntry (
              User, Role, Acl, new AclExpansionAccessConditions (), new[] { WriteAccessType }, new[] { ReadAccessType, DeleteAccessType });
      var aclExpansionEntry2 =
          new AclExpansionEntry (
              User, Role, Acl2, new AclExpansionAccessConditions (), new[] { WriteAccessType }, new[] { ReadAccessType, DeleteAccessType });
      var aclExpansionEntry3 =
         new AclExpansionEntry (
             User, Role, Acl, new AclExpansionAccessConditions (), new[] { ReadAccessType, WriteAccessType }, new[] { DeleteAccessType });
      Assert.That (comparer.Equals (aclExpansionEntry1, aclExpansionEntry1), Is.True);
      Assert.That (comparer.Equals (aclExpansionEntry1, aclExpansionEntry2), Is.True);
      Assert.That (comparer.Equals (aclExpansionEntry1, aclExpansionEntry3), Is.False);
    }


    [Test]
    public void AclExpansionEntryIgnoreStateEqualityComparerTest2 ()
    {
      var comparer = AclExpansionTree.AclExpansionEntryIgnoreStateEqualityComparer;

      var aclSameClassDiffernenStates = TestHelper.CreateStatefulAcl (Acl.Class, new StateDefinition[] { });
      var aclDifferentClass = TestHelper.CreateStatefulAcl (TestHelper.CreateClassDefinition ("2008-11-26, 16:41"), new StateDefinition[] { });

      var a =
          new AclExpansionEntry (
              User, Role, Acl, new AclExpansionAccessConditions (), new[] { WriteAccessType }, new[] { ReadAccessType, DeleteAccessType });
      var aDifferent =
          new AclExpansionEntry (
              User2, Role2, aclDifferentClass, new AclExpansionAccessConditions { OwningTenant = Tenant }, new[] { ReadAccessType }, new[] { DeleteAccessType });

      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.True);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, aclSameClassDiffernenStates, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.True);

      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (aDifferent.User, a.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, aDifferent.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, aDifferent.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, aDifferent.AccessConditions, a.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, a.AccessConditions, aDifferent.AllowedAccessTypes, a.DeniedAccessTypes)), Is.False);
      Assert.That (comparer.Equals (a,
        new AclExpansionEntry (a.User, a.Role, a.AccessControlList, a.AccessConditions, a.AllowedAccessTypes, aDifferent.DeniedAccessTypes)), Is.False);
    }



    [Test]
    public void ToTextTest ()
    {
      using (new CultureScope ("de-DE"))
      {
        var users = Remotion.Development.UnitTesting.ObjectMother.ListMother.New (User);
        var statelessAcl = CreateStatelessAcl (Ace);
        var acls = ListMother.New (Acl, statelessAcl);
        List<AclExpansionEntry> aclExpansionEntryList = GetAclExpansionEntryList (users, acls, false);
        var aclExpansionTree = new AclExpansionTree (aclExpansionEntryList);
        var result = To.String.e (aclExpansionTree).ToString();
        To.ConsoleLine.e (result);
        var resultExpected =
        #region
 @"{
([""DaUs""],2,
{
  ([""DaUs"",""Da Group"",""Supreme Being""],2,
  {
    ([""Remotion.SecurityManager.UnitTests.TestDomain.Order""],1,
    {
      ([user=""DaUs"",role=[""DaUs"",""Da Group"",""Supreme Being""],allowed={[""Write""]},denied={},conditions=[tenantMustOwn=True]],1,
      [
        [user=""DaUs"",role=[""DaUs"",""Da Group"",""Supreme Being""],allowed={[""Write""]},denied={},conditions=[tenantMustOwn=True]],
        ])}),
    ([""Bestellung""],1,
    {
      ([user=""DaUs"",role=[""DaUs"",""Da Group"",""Supreme Being""],allowed={[""FirstAccessType""],[""Read""],[""Delete""]},denied={},conditions=[]],2,
      [
        [user=""DaUs"",role=[""DaUs"",""Da Group"",""Supreme Being""],allowed={[""FirstAccessType""],[""Read""],[""Delete""]},denied={},conditions=[]],
        [user=""DaUs"",role=[""DaUs"",""Da Group"",""Supreme Being""],allowed={[""FirstAccessType""],[""Read""],[""Delete""]},denied={},conditions=[]],
        ])})})})}";
        #endregion
        Assert.That (result, Is.EqualTo (resultExpected));
      }
    }




    public static void LogAclExpansionTree (AclExpansionTree aclExpansionTree)
    {
      To.Console.IndentationString = "  ";
      To.Console.AllowNewline = true;
      To.ConsoleLine.nl (2).e (aclExpansionTree.Tree);
    }

    public void WriteAclExpansionTreeToConsole (AclExpansionTree aclExpansionTree)
    {
      To.Console.IndentationString = "  ";
      To.Console.AllowNewline = true;
      To.ConsoleLine.nl (2).e (aclExpansionTree.Tree);
    }

    public void WriteAclExpansionAsHtmlToDisk (List<AclExpansionEntry> aclExpansion, string fileName)
    {
      using (var textWriter = new StreamWriter (fileName))
      {
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter (textWriter, true,
            new AclExpansionHtmlWriterSettings { OutputRowCount = true });
        aclExpansionHtmlWriter.WriteAclExpansion (aclExpansion);
      }
    }


    private AccessControlList CreateStatelessAcl (params AccessControlEntry[] aces)
    {
      SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition ();
      var statlessAcl = TestHelper.CreateStatelessAcl (classDefinition);
      foreach (AccessControlEntry ace in aces)
      {
        TestHelper.AttachAces (statlessAcl, ace);
      }
      return statlessAcl;
    }


 
  }
}
