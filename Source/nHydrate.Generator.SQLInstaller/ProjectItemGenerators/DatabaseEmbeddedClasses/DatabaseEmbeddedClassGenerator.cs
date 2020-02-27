//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 0168
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using nHydrate.Generator.Common.GeneratorFramework;
using nHydrate.Generator.Models;
using nHydrate.Generator.ProjectItemGenerators;
using nHydrate.Generator.Common.Util;
using nHydrate.Generator.SQLInstaller;
using nHydrate.Generator.Common.EventArgs;
using System.IO;

namespace nHydrate.Generator.SQLInstaller.ProjectItemGenerators.DatabaseEmbeddedClasses
{
    [GeneratorItem("DatabaseEmbeddedClassGenerator", typeof(DatabaseProjectGenerator))]
    public class DatabaseEmbeddedClassGenerator : BaseDbScriptGenerator
    {
        private const string EMBEDDED_LOCATION = "ProjectItemGenerators.DatabaseEmbeddedClasses";

        #region Overrides

        public override int FileCount
        {
            get { return 22; }
        }

        public override void Generate()
        {
            try
            {
                GenerateDatabaseInstallerCs();
                GenerateDatabaseInstallerDesignerCs();
                GenerateSqlServersCs();
                //GenerateArchiveReaderCs();
                GenerateXmlHelperCs();
                GenerateProgramCs();
                GenerateUpgradeInstaller();

                //Folder structure
                GenerateFolder1();
                GenerateFolder2();
                GenerateFolder3();
                GenerateFolder4();
                GenerateFolder5();
                GenerateFolder6();

                var gcEventArgs = new ProjectItemGenerationCompleteEventArgs(this);
                OnGenerationComplete(this, gcEventArgs);

                //_model.IncrementGeneratedVersion();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region gen Folder 1-6 Files

        private void GenerateFolder1()
        {
            var foldername = "1_UserDefinedInitialization";
            var fileName = string.Empty;
            ProjectItemGeneratedEventArgs eventArgs = null;

            //Main folder Readme.txt
            fileName = Path.Combine(foldername, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, GetTextFolder1ReadMe(), ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Create Always
            var folderAlways = Path.Combine(foldername, "Always");
            fileName = Path.Combine(folderAlways, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will always run first in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //New Database
            var folderNewDatabase = Path.Combine(foldername, "NewDatabase");
            fileName = Path.Combine(folderNewDatabase, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run first on a database creation in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Unversioned
            var folderUnversioned = Path.Combine(foldername, "UnVersioned");
            fileName = Path.Combine(folderUnversioned, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run first on an existing non-versioned database in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Versioned
            var folderVersioned = Path.Combine(foldername, "Versioned");
            fileName = Path.Combine(folderVersioned, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run first on an existing versioned database in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

        }

        private void GenerateFolder2()
        {
            var foldername = "2_Upgrade Scripts";
            var fileName = string.Empty;
            ProjectItemGeneratedEventArgs eventArgs = null;

            fileName = Path.Combine(foldername, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Scripts in this folder are generated once and can be modified by the user after generation.\r\nEverything under this folder will be run in versioned number order.\r\nThese scripts will only be run on a previous versioned database, not on a create or unversioned database.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);
        }

        private void GenerateFolder3()
        {
            var foldername = "3_GeneratedTablesAndData";
            var fileName = string.Empty;
            ProjectItemGeneratedEventArgs eventArgs = null;

            fileName = Path.Combine(foldername, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Do not modify the generated scripts in this folder.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

        }

        private void GenerateFolder4()
        {
            var foldername = "4_UserDefinedPostTablesAndData";
            var fileName = string.Empty;
            ProjectItemGeneratedEventArgs eventArgs = null;

            //Main folder Readme.txt
            fileName = Path.Combine(foldername, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, GetTextFolder4ReadMe(), ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Create Always
            var folderAlways = Path.Combine(foldername, "Always");
            fileName = Path.Combine(folderAlways, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will always run after schema and upgrades in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //New Database
            var folderNewDatabase = Path.Combine(foldername, "NewDatabase");
            fileName = Path.Combine(folderNewDatabase, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run after schema and upgrades on a database creation in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Unversioned
            var folderUnversioned = Path.Combine(foldername, "UnVersioned");
            fileName = Path.Combine(folderUnversioned, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run after schema and upgrades on an existing non-versioned database in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Versioned
            var folderVersioned = Path.Combine(foldername, "Versioned");
            fileName = Path.Combine(folderVersioned, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run after schema and upgrades on an existing versioned database in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

        }

        private void GenerateFolder5()
        {
            var foldername = "5_Programmability";
            var fileName = string.Empty;
            ProjectItemGeneratedEventArgs eventArgs = null;

            //User-defined Functions
            var folderUD = Path.Combine(Path.Combine(foldername, "Functions"), "User Defined");
            fileName = Path.Combine(folderUD, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add user-defined scripts for database functions in this folder.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //User-defined Stored Procedures
            folderUD = Path.Combine(Path.Combine(foldername, "Stored Procedures"), "User Defined");
            fileName = Path.Combine(folderUD, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add user-defined scripts for database stored procedures in this folder.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //User-defined Views
            folderUD = Path.Combine(Path.Combine(foldername, "Views"), "User Defined");
            fileName = Path.Combine(folderUD, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add user-defined scripts for database views in this folder.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

        }

        private void GenerateFolder6()
        {
            var foldername = "6_UserDefinedFinalize";
            var fileName = string.Empty;
            ProjectItemGeneratedEventArgs eventArgs = null;

            //Main folder Readme.txt
            fileName = Path.Combine(foldername, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, GetTextFolder4ReadMe(), ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Create Always
            var folderAlways = Path.Combine(foldername, "Always");
            fileName = Path.Combine(folderAlways, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will always run after all other scripts in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //New Database
            var folderNewDatabase = Path.Combine(foldername, "NewDatabase");
            fileName = Path.Combine(folderNewDatabase, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run after all other scripts on a database creation in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Unversioned
            var folderUnversioned = Path.Combine(foldername, "UnVersioned");
            fileName = Path.Combine(folderUnversioned, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run after all other scripts on an existing non-versioned database in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

            //Versioned
            var folderVersioned = Path.Combine(foldername, "Versioned");
            fileName = Path.Combine(folderVersioned, "ReadMe.txt");
            eventArgs = new ProjectItemGeneratedEventArgs(fileName, "Add scripts that will be run after all other scripts on an existing versioned database in this folder. Make them embedded resources and they will be run in alphabetical order.", ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);

        }

        #endregion

        #region ReadMe Files

        private string GetTextFolder1ReadMe()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Please place any custom scripts that need to run prior to all generated scripts here. Make sure they are an embedded resource");
            sb.AppendLine();
            sb.AppendLine("There are three ways that a database install happens:");
            sb.AppendLine("\"NewDatabase\" Folder - During the process of installation the user chooses to create a new database. ");
            sb.AppendLine("\"Unversioned\" Folder - During the process of installation the user chooses to upgrade a database that has not been versioned by nHydrate");
            sb.AppendLine("\"Versioned\" Folder - During the process of installation the user chooses to upgrade a database that has been versioned by nHydrate");
            sb.AppendLine("\"Always\" Folder: After the scenario specific scripts have run. Scripts in the \"Always\" folder will be run regardless of scenario.");
            sb.AppendLine();
            sb.AppendLine("For each specific folder custom scripts are run in Alphabetical order. For this reason it is best practice to use a number based prefix as part of your naming standard example:");
            sb.AppendLine("000_Always");
            sb.AppendLine("001_Always");
            sb.AppendLine("002_Always");
            sb.AppendLine("::::::::");
            sb.AppendLine("999_Always");
            return sb.ToString();
        }

        private string GetTextFolder4ReadMe()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Please place any custom scripts that need to run after the generated schema and data files have been run.  Make sure they are an embedded resource.");
            sb.AppendLine();
            sb.AppendLine("There are three ways that an db install happens:");
            sb.AppendLine("\"NewDatabase\" Folder - During the process of installation the user chooses to create a new database. ");
            sb.AppendLine("\"Unversioned\" Folder - During the process of installation the user chooses to upgrade a database that has not been versioned by nHydrate");
            sb.AppendLine("\"Versioned\" Folder - During the process of installation the user chooses to upgrade a database that has been versioned by nHydrate");
            sb.AppendLine("\"Always\" Folder: After the scenario specific scripts have run. Scripts in the \"Always\" folder will be run regardless of scenario.");
            sb.AppendLine();
            sb.AppendLine("For each specific folder custom scripts are run in Alphabetical order. For this reason it is best practice to use a number ");
            sb.AppendLine("based prefix as part of your naming standard example:");
            sb.AppendLine("000_Always");
            sb.AppendLine("001_Always");
            sb.AppendLine("002_Always");
            sb.AppendLine("::::::::");
            sb.AppendLine("999_Always");
            return sb.ToString();
        }

        private string GetTextFolder6ReadMe()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Please place any custom scripts that need to run after everything else here. Make sure they are an embedded resource.");
            sb.AppendLine();
            sb.AppendLine("There are three ways that an db install happens:");
            sb.AppendLine("\"NewDatabase\" Folder - During the process of installation the user chooses to create a new database. ");
            sb.AppendLine("\"Unversioned\" Folder - During the process of installation the user chooses to upgrade a database that has not been versioned by nHydrate");
            sb.AppendLine("\"Versioned\" Folder - During the process of installation the user chooses to upgrade a database that has been versioned by nHydrate");
            sb.AppendLine("\"Always\" Folder: After the scenario specific scripts have run. Scripts in the \"Always\" folder will be run regardless of scenario.");
            sb.AppendLine();
            sb.AppendLine("For each specific folder custom scripts are run in Alphabetical order. For this reason it is best practice to use a number ");
            sb.AppendLine("based prefix as part of your naming standard example:");
            sb.AppendLine("000_Always");
            sb.AppendLine("001_Always");
            sb.AppendLine("002_Always");
            sb.AppendLine("::::::::");
            sb.AppendLine("999_Always");
            return sb.ToString();
        }

        #endregion

        #region UpgradeInstaller.cs

        private void GenerateUpgradeInstaller()
        {
            var fileName = "UpgradeInstaller.cs";
            var fileContent = GetFileContent(new EmbeddedResourceName(this.GetEmbeddedPath() + "." + fileName));
            //TODO - Change the version for _upgradeToVersion and _previousVersion
            var eventArgs = new ProjectItemGeneratedEventArgs(fileName, fileContent, ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);
        }

        #endregion

        #region SqlServers.cs

        private void GenerateSqlServersCs()
        {
            var fileName = "SqlServers.cs";
            var fileContent = GetFileContent(new EmbeddedResourceName(this.GetEmbeddedPath() + "." + fileName));
            var eventArgs = new ProjectItemGeneratedEventArgs(fileName, fileContent, ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);
        }

        #endregion

        #region XmlHelper.cs

        private void GenerateXmlHelperCs()
        {
            var fileName = "XmlHelper.cs";
            var fileContent = GetFileContent(new EmbeddedResourceName(this.GetEmbeddedPath() + "." + fileName));
            var eventArgs = new ProjectItemGeneratedEventArgs(fileName, fileContent, ProjectName, this, false);
            OnProjectItemGenerated(this, eventArgs);
        }

        #endregion

        private void GenerateProgramCs()
        {
            var fileName = "Program.cs";
            var fileContent = GetFileContent(new EmbeddedResourceName(this.GetEmbeddedPath() + "." + fileName));
            var eventArgs = new ProjectItemGeneratedEventArgs(fileName, fileContent, ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);
        }

        #region DatabaseInstaller

        private void GenerateDatabaseInstallerDesignerCs()
        {
            var fullParentName = "DatabaseInstaller.cs";
            var fileName = "DatabaseInstaller.Designer.cs";
            var ern = new EmbeddedResourceName();
            ern.AsmLocation = this.GetEmbeddedPath();
            ern.FileName = "DatabaseInstaller.Designer.embed";
            ern.FullName = this.GetEmbeddedPath() + "." + ern.FileName;
            var fileContent = GetFileContent(ern);
            var eventArgs = new ProjectItemGeneratedEventArgs(fileName, fileContent, ProjectName, fullParentName, this, true);
            OnProjectItemGenerated(this, eventArgs);
        }

        private void GenerateDatabaseInstallerCs()
        {
            var fileName = "DatabaseInstaller.cs";
            var fileContent = GetFileContent(new EmbeddedResourceName(this.GetEmbeddedPath() + "." + fileName));
            var eventArgs = new ProjectItemGeneratedEventArgs(fileName, fileContent, ProjectName, this, true);
            OnProjectItemGenerated(this, eventArgs);
        }

        #endregion

        private string GetResource(EmbeddedResourceName ern)
        {
            var retVal = string.Empty;
            var asm = Assembly.GetExecutingAssembly();
            var manifestStream = asm.GetManifestResourceStream(ern.FullName);
            try
            {
                using (var sr = new System.IO.StreamReader(manifestStream))
                {
                    retVal = sr.ReadToEnd();
                }
            }
            catch
            {
            }
            finally
            {
                manifestStream.Close();
            }
            return retVal;
        }

        private string GetFileContent(EmbeddedResourceName ern)
        {
            try
            {
                string retVal = GetResource(ern);
                retVal = ReplaceNHydrateSpecifics(retVal);
                return retVal;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string ReplaceNHydrateSpecifics(string input)
        {
            var retVal = input;
            retVal = retVal.Replace("Acme", _model.CompanyName);
            retVal = retVal.Replace("%MODELKEY%", _model.Key);
            var versionNumbers = _model.Version.Split('.');

            int major = int.Parse(versionNumbers[0]);
            var minor = int.Parse(versionNumbers[1]);
            var revision = int.Parse(versionNumbers[2]);
            var build = int.Parse(versionNumbers[3]);
            var generated = _model.GeneratedVersion;

            retVal = retVal.Replace("\"UPGRADE_VERSION\"", major + ", " + minor + ", " + revision + ", " + build + ", " + generated);
            retVal = retVal.Replace("DATABASENAME", _model.Database.DatabaseName);
            retVal = retVal.Replace("PROJECTNAMESPACE", this.GetLocalNamespace());
            return retVal;
        }

        private string GetEmbeddedPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "." + EMBEDDED_LOCATION;
        }

        #endregion

    }
}