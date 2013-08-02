// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Remotion.Logging;
using Remotion.Text;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Globalization
{
  /// <summary>
  ///   Combines one or more <see cref="IResourceManager"/> instances to a set that can be accessed using a single interface.
  /// </summary>
  public class ResourceManagerSet : ReadOnlyCollection<IResourceManager>, IResourceManager
  {//TODO AO: change readonlycollection from baseclass to member
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ResourceManagerSet));

    private readonly string _name;

    /// <summary>
    ///   Combines several IResourceManager instances to a single ResourceManagerSet, starting with the first entry of the first set.
    /// </summary>
    /// <remarks>
    ///   For parameters that are ResourceManagerSet instances, the contained IResourceManagers are added directly.
    /// </remarks>
    /// <example>
    ///   <para>
    ///     Given the following parameter list of resource managers (rm) and resource manager sets (rmset):
    ///   </para><para>
    ///     rm1, rm2, rmset (rm3, rm4, rm5), rm6, rmset (rm7, rm8)
    ///   </para><para>
    ///     The following resource manager set is created:
    ///   </para><para>
    ///     rmset (rm1, rm2, rm3, rm4, rm5, rm6, rm7, rm8)
    ///   </para>
    /// </example>
    /// <param name="resourceManagers"> The resource manager, starting with the least specific. </param>
    public static ResourceManagerSet Create (IEnumerable<IResourceManager> resourceManagers)
    {
      ArgumentUtility.CheckNotNull ("resourceManagers", resourceManagers);
      
      return new ResourceManagerSet (CreateFlatList(resourceManagers));
    }

    protected ResourceManagerSet (IEnumerable<IResourceManager> resourceManagers) : base(resourceManagers.ToList())
    {
      SeparatedStringBuilder sb = new SeparatedStringBuilder (", ", 30 * Count);
      foreach (IResourceManager rm in this)
        sb.Append (rm.Name);
      _name = sb.ToString ();
    }

    [Obsolete("User ResourceManagerSet.Create instead", true)]
    public ResourceManagerSet (params IResourceManager[] resourceManagers)
        : base (null)
    {
      throw new InvalidOperationException ("User ResourceManagerSet.Create instead");
    }

    public NameValueCollection GetAllStrings ()
    {
      return GetAllStrings (string.Empty);
    }

    /// <summary>
    ///   Searches for all string resources inside the resource manager whose name is prefixed with a matching tag.
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetAllStrings(System.String)"/>
    public NameValueCollection GetAllStrings (string prefix)
    {
      var result = new NameValueCollection();
      foreach (var resourceManager in this)
      {
        var strings = resourceManager.GetAllStrings (prefix);
        for (var i = 0; i < strings.Count; i++)
        {
          var key = strings.Keys[i];
          if(result[key]==null)
            result[key] = strings[i];
        }
      }
      return result;
    }

    /// <summary>
    ///   Gets the value of the specified string resource. 
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetString(System.String)"/>
    public string GetString (string id)
    {
      for (var i = 0; i < Count; i++)
      {
        var s = this[i].GetString (id);
        if (s != null && s != id)
          return s;
      }

      s_log.Debug ("Could not find resource with ID '" + id + "' in any of the following resource containers " + _name + ".");
      return id;
    }

    /// <summary>
    ///   Gets the value of the specified string resource. 
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)"/>
    public string GetString (Enum enumValue)
    {
      return GetString (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    /// <summary>Tests whether the <see cref="ResourceManagerSet"/> contains the specified resource.</summary>
    /// <param name="id">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="ResourceManagerSet"/> contains the specified resource.</returns>
    public bool ContainsResource (string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      for (var i = 0; i < Count; i++)
      {
        if (this[i].ContainsResource (id))
          return true;
      }
      return false;
    }

    /// <summary>Tests whether the <see cref="ResourceManagerSet"/> contains the specified resource.</summary>
    /// <param name="enumValue">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="ResourceManagerSet"/> contains the specified resource.</returns>
    public bool ContainsResource (Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);
      return ContainsResource (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    private static IEnumerable<IResourceManager> CreateFlatList (IEnumerable<IResourceManager> resourceManagers)
    {
      foreach (var resourceManager in resourceManagers)
      {
        var rmset = resourceManager as ResourceManagerSet;
        if (rmset != null)
        {
          foreach (var rm in rmset)
            yield return rm;
        }
        else if (resourceManager != null && !resourceManager.IsNull)
        {
          yield return resourceManager;
        }
      }
    }

    public string Name
    {
      get { return _name; }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
