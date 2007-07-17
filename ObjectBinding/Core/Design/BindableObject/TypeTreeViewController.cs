using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Design.BindableObject
{
  public class TypeTreeViewController
  {
    private readonly List<Type> _types;
    private readonly TreeView _treeView;

    public TypeTreeViewController (List<Type> types, TreeView treeView)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("types", types);
      ArgumentUtility.CheckNotNull ("treeView", treeView);

      _types = types;
      _treeView = treeView;
    }

    public List<Type> Types
    {
      get { return _types; }
    }

    public TreeView TreeView
    {
      get { return _treeView; }
    }

    public void PopulateTreeNodes (string selectedValue)
    {
      _treeView.BeginUpdate();
      _treeView.Nodes.Clear();

      foreach (Type type in _types)
      {
        TreeNode assemblyNode = GetAssemblyNode (type, _treeView.Nodes);
        TrySelect (assemblyNode, selectedValue);
        
        TreeNode namespaceNode = GetNamespaceNode (type, assemblyNode.Nodes);
        TrySelect (namespaceNode, selectedValue);
        
        TreeNode typeNode = GetTypeNode (type, namespaceNode.Nodes);
        TrySelect (typeNode, selectedValue);
      }

      ExpandTypeTreeView();
      _treeView.EndUpdate();
    }

    public Type GetSelectedType ()
    {
      if (_treeView.SelectedNode == null)
        return null;
      return _treeView.SelectedNode.Tag as Type;
    }

    private TreeNode GetAssemblyNode (Type type, TreeNodeCollection assemblyNodes)
    {
      AssemblyName assemblyName = type.Assembly.GetName();
      TreeNode assemblyNode = assemblyNodes[assemblyName.FullName];
      if (assemblyNode == null)
      {
        assemblyNode = new TreeNode();
        assemblyNode.Name = assemblyName.FullName;
        assemblyNode.Text = assemblyName.Name;
        assemblyNode.ToolTipText = assemblyName.FullName;

        assemblyNodes.Add (assemblyNode);
      }

      return assemblyNode;
    }

    private TreeNode GetNamespaceNode (Type type, TreeNodeCollection namespaceNodes)
    {
      TreeNode namespaceNode = namespaceNodes[type.Namespace];
      if (namespaceNode == null)
      {
        namespaceNode = new TreeNode();
        namespaceNode.Name = type.Namespace;
        namespaceNode.Text = type.Namespace;

        namespaceNodes.Add (namespaceNode);
      }

      return namespaceNode;
    }

    private TreeNode GetTypeNode (Type type, TreeNodeCollection typeNodes)
    {
      TreeNode typeNode = typeNodes[type.FullName];
      if (typeNode == null)
      {
        typeNode = new TreeNode();
        typeNode.Name = TypeUtility.GetPartialAssemblyQualifiedName (type);
        typeNode.Text = type.Name;
        typeNode.Tag = type;

        typeNodes.Add (typeNode);
      }

      return typeNode;
    }

    private void TrySelect (TreeNode node, string selectedValue)
    {
      if (node.Name.Equals (selectedValue, StringComparison.CurrentCultureIgnoreCase))
        _treeView.SelectedNode = node;
    }

    private void ExpandTypeTreeView ()
    {
      if (_treeView.Nodes.Count < 3)
      {
        foreach (TreeNode assemblyNode in _treeView.Nodes)
        {
          assemblyNode.Expand();
          if (assemblyNode.Nodes.Count < 3)
            assemblyNode.ExpandAll();
        }
      }
    }
  }
}