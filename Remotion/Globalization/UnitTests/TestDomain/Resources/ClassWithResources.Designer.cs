﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace Remotion.Globalization.UnitTests.TestDomain.Resources {
  /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ClassWithResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ClassWithResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Remotion.UnitTests.Globalization.TestDomain.Resources.ClassWithResources", typeof(ClassWithResources).Assembly);
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
        ///   Looks up a localized string similar to Short Property ID.
        /// </summary>
        internal static string property_PropertyWithShortIdentifier {
            get {
                return ResourceManager.GetString("property:PropertyWithShortIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Long Property ID.
        /// </summary>
        internal static string property_Remotion_UnitTests_Globalization_TestDomain_ClassWithProperties_PropertyWithLongIdentifier {
            get {
                return ResourceManager.GetString("property:Remotion.UnitTests.Globalization.TestDomain.ClassWithProperties.Property" +
                        "WithLongIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value 1.
        /// </summary>
        internal static string property_Value1 {
            get {
                return ResourceManager.GetString("property:Value1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Short Type ID.
        /// </summary>
        internal static string type_ClassWithShortResourceIdentifier {
            get {
                return ResourceManager.GetString("type:ClassWithShortResourceIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Long Type ID.
        /// </summary>
        internal static string type_Remotion_UnitTests_Globalization_TestDomain_ClassWithLongResourceIdentifier {
            get {
                return ResourceManager.GetString("type:Remotion.UnitTests.Globalization.TestDomain.ClassWithLongResourceIdentifier", resourceCulture);
            }
        }
    }
}
