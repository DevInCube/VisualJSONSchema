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
        ///			&quot;DbPort&quot;: 5432,
        ///			&quot;DbName&quot;:&quot;autocode&quot;,
        ///			&quot;DbUser&quot;:&quot;autocode&quot;,
        ///			&quot;DbPass&quot;: &quot;autocode&quot;,
        ///			&quot;PostName&quot;:&quot;EdgeServer&quot;,
        ///			&quot;Post&quot;:&quot;69e86fa7-e1ad-4e68-96b7-b910f40bdb49&quot;
        ///		}
        ///	},
        ///
        ///	&quot;SocketApi&quot;: 
        ///	{
        ///		&quot;ParamSet&quot;: 
        ///		{
        ///			&quot;Host&quot;: &quot;127.0.0.1&quot;,
        ///			&quot;Port&quot;: 14
        ///		}
        ///	},
        ///	 &quot;FileApi&quot;:
        ///	 {
        ///	   &quot;AddFactReact&quot;:
        ///	   [
        ///		 {
        ///		   &quot;ParamSet&quot;:
        ///		   {
        ///			 &quot;Channel&quot;:0,
        ///			 &quot;Prefix&quot;:&quot;&quot;,
        ///			 &quot;Suffix&quot;:&quot;&quot;,
        ///			 &quot;Dir&quot;:&quot;/tmp/edge/event_store/stor [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Data {
            get {
                return ResourceManager.GetString("Data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  
        ///	&quot;Import&quot;:
        ///	{
        ///		&quot;File&quot;:&quot;/opt/edge/libexec/mediacodec/libimport-file.so&quot;,
        ///		&quot;ParamSet&quot;:
        ///		{
        ///			&quot;FrameMeta&quot;:
        ///			{
        ///				&quot;Prefix&quot;:&quot;&quot;,
        ///				&quot;Suffix&quot;:&quot;.gscene&quot;,
        ///				&quot;Dir&quot;:&quot;&quot;
        ///			},
        ///			&quot;FrameData&quot;:
        ///			{
        ///				&quot;Prefix&quot;:&quot;&quot;,
        ///				&quot;Suffix&quot;:&quot;.gframe&quot;,
        ///				&quot;Dir&quot;:&quot;&quot;
        ///			}
        ///		}
        ///	},
        ///	&quot;Codec&quot;:
        ///	{
        ///		&quot;File&quot;:&quot;/opt/edge/libexec/mediacodec/libcodec-jpegturbo.so&quot;,
        ///		&quot;Source&quot;:&quot;MONO8T&quot;,
        ///		&quot;Target&quot;:&quot;JPEG&quot;,
        ///		&quot;ParamSet&quot;: {}
        ///	},
        ///	&quot;Export&quot;:
        ///	{
        ///		&quot;File&quot;:&quot;/opt/edge/libexec/mediacodec/libexport-file.so&quot;,
        ///		&quot;Par [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MediaCodec {
            get {
                return ResourceManager.GetString("MediaCodec", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///	&quot;id&quot; : &quot;http://vit.com.ua/edgeserver/mediacodec#&quot;,
        ///	&quot;$schema&quot;: &quot;http://json-schema.org/draft-04/schema#&quot;,
        ///
        ///	&apos;title&apos; : &apos;MediaCodec main.conf&apos;,
        ///	&apos;type&apos; : &apos;object&apos;,
        ///
        ///	&apos;definitions&apos; : {
        ///
        ///		&apos;Frame&apos; : {
        ///
        ///			&apos;type&apos; : &apos;object&apos;,
        ///
        ///			&apos;properties&apos; : {
        ///
        ///				&quot;Prefix&quot; : {
        ///					&apos;type&apos; : &apos;string&apos;,
        ///				},
        ///				&quot;Suffix&quot; : {
        ///					&apos;type&apos; : &apos;string&apos;,
        ///				},
        ///				&quot;Dir&quot; : {
        ///					&apos;type&apos; : &apos;string&apos;,
        ///				},
        ///			},
        ///
        ///			&quot;required&quot; : [ &apos;Prefix&apos;, &quot;Suffix&quot;, &apos;Dir&apos; ],
        ///			&quot;additionalProperties&quot;: false,
        ///		}, 
        ///		        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MediaCodec_schema {
            get {
                return ResourceManager.GetString("MediaCodec_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///  &quot;Store&quot;:
        ///  {
        ///    &quot;ParamSet&quot;:
        ///    {
        ///      &quot;RootDir&quot;:&quot;var/opt/edge/mediastore.0/&quot;,        // string
        ///      &quot;Channels&quot;:
        ///      [
        ///        {
        ///          &quot;Name&quot;:&quot;0&quot;,                                // string
        ///          &quot;DepthAsMin&quot;:30                            // uint
        ///        }
        ///      ]
        ///    }
        ///  },
        ///  &quot;SocketApi&quot;:
        ///  {
        ///    &quot;ParamSet&quot;:
        ///    {
        ///      &quot;EndPoint&quot;:&quot;/tmp/edge/mediastore.0.endpoint&quot;,  // string
        ///      &quot;MaxClients&quot;:1                                 // uint
        ///    }
        ///  },
        ///  &quot;FileApi&quot;:
        ///  {
        ///    [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MediaStore {
            get {
                return ResourceManager.GetString("MediaStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///
        ///	&quot;id&quot; : &quot;http://vit.com.ua/edgeserver/mediastore#&quot;,
        ///	&quot;$schema&quot;: &quot;http://json-schema.org/draft-04/schema#&quot;,
        ///
        ///	&apos;title&apos; : &apos;MediaStore main.conf&apos;,
        ///	&apos;type&apos; : &apos;object&apos;,
        ///
        ///	&apos;definitions&apos; : {
        ///
        ///		&apos;FrameReact&apos; : {
        ///			
        ///			&apos;type&apos; : &apos;object&apos;,
        ///
        ///			&apos;properties&apos; : {
        ///
        ///				&quot;Channel&quot;: {
        ///					&apos;type&apos; : &apos;string&apos;,
        ///				},                             
        ///				&quot;Prefix&quot;:{
        ///					&apos;type&apos; : &apos;string&apos;,
        ///				},                            
        ///				&quot;Suffix&quot;:{
        ///					&apos;type&apos; : &apos;string&apos;,
        ///				},                              
        ///				&quot;D [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MediaStore_schema {
            get {
                return ResourceManager.GetString("MediaStore_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        /// {
        ///	&quot;id&quot; : &quot;http://vit.com.ua/edgeserver/eventstore#&quot;,
        ///	&quot;$schema&quot;: &quot;http://json-schema.org/draft-04/schema#&quot;,
        ///
        ///	&quot;title&quot; : &quot;EventStore main.xconf&quot;,
        ///	&quot;type&quot; : &quot;object&quot;,
        ///
        ///	&apos;format&apos; : &apos;tab&apos;,
        ///
        ///	&quot;definitions&quot; : {
        ///
        ///		&quot;DbStore.ParamSet&quot; : {
        ///
        ///			&quot;type&quot; : &quot;object&quot;,
        ///
        ///			&quot;properties&quot;: {
        ///					
        ///				&quot;DbHost&quot;: {
        ///					&quot;title&quot; : &quot;Host&quot;,
        ///					&quot;type&quot;: &quot;string&quot;,
        ///					&quot;format&quot;: &quot;ipv4&quot;,
        ///					&quot;default&quot;: &quot;127.0.0.1&quot;,
        ///					&quot;binding&quot; : {
        ///						&quot;path&quot; : &quot;DbHost&quot;
        ///					}
        ///				},
        ///				&quot;DbPort&quot;: {
        ///					&quot;title&quot; : [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Schema {
            get {
                return ResourceManager.GetString("Schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///	&apos;boolean&apos; : true,
        ///	&apos;string&apos; : &apos;string&apos;
        ///}.
        /// </summary>
        internal static string TestData {
            get {
                return ResourceManager.GetString("TestData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///	&apos;type&apos;:&apos;object&apos;, 
        ///	
        ///
        ///    &apos;title&apos;:&apos;Root Object Title&apos;,
        ///	&apos;description&apos; : &apos;This is Root Object description&apos;,
        ///
        ///    &apos;properties&apos;:{
        ///
        ///		&apos;boolean&apos; : {
        ///            &apos;type&apos;:&apos;boolean&apos;,
        ///			default : true,
        ///			readonly : true
        ///        },
        ///        &apos;string&apos; : {
        ///            &apos;type&apos;:&apos;string&apos;,
        ///			visible : false
        ///        },
        ///		&apos;object&apos; : {
        ///
        ///			type : [ &apos;object&apos;, &apos;null&apos;],
        ///
        ///			title : &apos;This is the object&apos;,
        ///			description : &apos;This is the object description&apos;,
        ///
        ///			&apos;properties&apos; : {
        ///				&apos;bool1&apos; : {
        ///					&apos;type&apos; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TestSchema {
            get {
                return ResourceManager.GetString("TestSchema", resourceCulture);
            }
        }
    }
}
