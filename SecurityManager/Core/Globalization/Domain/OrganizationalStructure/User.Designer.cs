﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class User {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal User() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.User", typeof(User).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name.
        /// </summary>
        internal static string property_DisplayName {
            get {
                return ResourceManager.GetString("property:DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Firstname.
        /// </summary>
        internal static string property_FirstName {
            get {
                return ResourceManager.GetString("property:FirstName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lastname.
        /// </summary>
        internal static string property_LastName {
            get {
                return ResourceManager.GetString("property:LastName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Group.
        /// </summary>
        internal static string property_OwningGroup {
            get {
                return ResourceManager.GetString("property:OwningGroup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Roles.
        /// </summary>
        internal static string property_Roles {
            get {
                return ResourceManager.GetString("property:Roles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Title.
        /// </summary>
        internal static string property_Title {
            get {
                return ResourceManager.GetString("property:Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Username.
        /// </summary>
        internal static string property_UserName {
            get {
                return ResourceManager.GetString("property:UserName", resourceCulture);
            }
        }
    }
}
