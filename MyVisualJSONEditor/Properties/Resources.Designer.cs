﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyVisualJSONEditor.Properties {
    using System;
    
    
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MyVisualJSONEditor.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to {
        ///	&quot;Store&quot;:
        ///	{
        ///		&quot;ParamSet&quot;:
        ///		{
        ///			&quot;DbHost&quot;:&quot;127.0.0.1&quot;,
        ///			&quot;DbPort&quot;:5432,
        ///			&quot;DbName&quot;:&quot;autocode&quot;,
        ///			&quot;DbUser&quot;:&quot;autocode&quot;,
        ///			&quot;DbPass&quot;:&quot;autocode&quot;,
        ///			&quot;PostName&quot;:&quot;EdgeServer&quot;,
        ///			&quot;Post&quot;:&quot;69e86fa7-e1ad-4e68-96b7-b910f40bdb49&quot;
        ///		}
        ///	}
        /// }.
        /// </summary>
        internal static string Data {
            get {
                return ResourceManager.GetString("Data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///	&quot;id&quot; : &quot;http://vit.com.ua/edgeserver/eventstore#&quot;,
        ///	&quot;$schema&quot;: &quot;http://json-schema.org/draft-04/schema#&quot;,
        ///	
        ///	&quot;definitions&quot; : {
        ///		&quot;Store&quot;: {
        ///			&quot;properties&quot; : { }
        ///		},
        ///		&quot;DbStore&quot;: {
        ///			&quot;extends&quot; : {	
        ///				&quot;$ref&quot; : &quot;#/definitions/Store&quot;
        ///			},
        ///			&quot;properties&quot; : { 
        ///				&quot;ParamSet&quot; : {
        ///					&quot;properties&quot;: {
        ///					
        ///						&quot;DbHost&quot;: {
        ///							&quot;type&quot;: &quot;string&quot;,
        ///							&quot;oneOf&quot;: [
        ///								{ &quot;format&quot;: &quot;host-name&quot; },
        ///								{ &quot;format&quot;: &quot;ipv4&quot; },
        ///								{ &quot;format&quot;: &quot;ipv6&quot; }
        ///							],
        ///							&quot;defau [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Schema {
            get {
                return ResourceManager.GetString("Schema", resourceCulture);
            }
        }
    }
}
