//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Rubicon.SecurityManager.Clients.Web.Globalization.Classes {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class SecurableClassDefinitionTreeViewResources {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SecurableClassDefinitionTreeViewResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("Rubicon.SecurityManager.Clients.Web.Globalization.Classes.SecurableClassDefinitio" +
                            "nTreeViewResources", typeof(SecurableClassDefinitionTreeViewResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} ({1} ACLs).
        /// </summary>
        internal static string MultipleAclsText {
            get {
                return ResourceManager.GetString("MultipleAclsText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} (0 ACLs).
        /// </summary>
        internal static string NoAclsText {
            get {
                return ResourceManager.GetString("NoAclsText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} (1 ACL).
        /// </summary>
        internal static string SingleAclText {
            get {
                return ResourceManager.GetString("SingleAclText", resourceCulture);
            }
        }
    }
}
