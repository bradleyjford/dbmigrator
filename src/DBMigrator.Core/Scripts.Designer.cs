﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DbMigrator.Core {
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
    internal class Scripts {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Scripts() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DbMigrator.Core.Scripts", typeof(Scripts).Assembly);
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
        ///   Looks up a localized string similar to BACKUP DATABASE [{0}] TO DISK = &apos;{1}&apos;.
        /// </summary>
        internal static string BackupDatabase {
            get {
                return ResourceManager.GetString("BackupDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE DATABASE [{0}].
        /// </summary>
        internal static string CreateDatabase {
            get {
                return ResourceManager.GetString("CreateDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF  EXISTS (SELECT name FROM sys.databases WHERE name = N&apos;{0}&apos;)
        ///BEGIN
        ///    ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
        ///    ALTER DATABASE [{0}] SET SINGLE_USER
        ///
        ///    DROP DATABASE [{0}]
        ///END.
        /// </summary>
        internal static string DropDatabase {
            get {
                return ResourceManager.GetString("DropDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;[dbo].[_SchemaMigration]&apos;) AND type = N&apos;U&apos;)
        ///BEGIN
        /// 
        ///	CREATE TABLE dbo.[_SchemaMigration]
        ///	(
        ///		[SchemaMigrationId]	INT					NOT NULL IDENTITY (1, 1),
        ///		[Filename]			NVARCHAR(200)		NOT NULL
        ///	)
        /// 
        ///	ALTER TABLE dbo.[_SchemaMigration] ADD CONSTRAINT [PK_SchemaMigration] PRIMARY KEY CLUSTERED ( [SchemaMigrationId] )
        /// 
        ///END.
        /// </summary>
        internal static string EnsureMigrationTableExists {
            get {
                return ResourceManager.GetString("EnsureMigrationTableExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF NOT EXISTS (SELECT 1 FROM dbo._SchemaMigration WHERE Filename = &apos;[FILE];[BATCH]&apos;) 
        ///	BEGIN
        ///         [SCRIPT]
        /// 
        ///         SET NOCOUNT ON
        ///         INSERT dbo._SchemaMigration VALUES (&apos;[FILE];[BATCH]&apos;)
        /// 
        ///         PRINT &apos;Successfully applied script &apos;&apos;[FILE];[BATCH]&apos;&apos;.&apos;
        ///	END.
        /// </summary>
        internal static string ScriptBatchTemplate {
            get {
                return ResourceManager.GetString("ScriptBatchTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N&apos;dbo._SchemaMigration&apos;) AND type = N&apos;U&apos;)
        ///BEGIN
        /// 
        ///	CREATE TABLE dbo._SchemaMigration
        ///	(
        ///		SchemaMigrationId	INT					NOT NULL IDENTITY(1, 1),
        ///		Filename			NVARCHAR(200)		NOT NULL
        ///	)
        /// 
        ///	ALTER TABLE dbo._SchemaMigration ADD CONSTRAINT PK_SchemaMigration PRIMARY KEY CLUSTERED (SchemaMigrationId)
        /// 
        ///END
        ///
        ///SET XACT_ABORT ON
        ///
        ///BEGIN TRANSACTION
        /// 
        ///BEGIN TRY
        /// 
        ///	[BEGIN LOOP]
        ///	IF NOT EXISTS (SELECT 1 FROM dbo._SchemaMigration WHERE F [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ScriptTemplate {
            get {
                return ResourceManager.GetString("ScriptTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALTER DATABASE [{0}] SET MULTI_USER.
        /// </summary>
        internal static string SetDatabaseMultiUser {
            get {
                return ResourceManager.GetString("SetDatabaseMultiUser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
        ///ALTER DATABASE [{0}] SET SINGLE_USER.
        /// </summary>
        internal static string SetDatabaseSingleUser {
            get {
                return ResourceManager.GetString("SetDatabaseSingleUser", resourceCulture);
            }
        }
    }
}
