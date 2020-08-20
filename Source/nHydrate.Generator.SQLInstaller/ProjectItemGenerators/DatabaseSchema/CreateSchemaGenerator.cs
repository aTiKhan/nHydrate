using nHydrate.Generator.Common.EventArgs;
using nHydrate.Generator.Common.GeneratorFramework;

namespace nHydrate.Generator.SQLInstaller.ProjectItemGenerators.DatabaseSchema
{
    [GeneratorItem("CreateSchema", typeof(SqlDatabaseProjectGenerator))]
    public class CreateSchemaGenerator : BaseDbScriptGenerator
    {
        #region Class Members

        private const string PARENT_ITEM_NAME = @"3_GeneratedTablesAndData";

        #endregion

        #region Overrides

        public override int FileCount => 1;

        public override void Generate()
        {
            var template = new CreateSchemaTemplate(_model);
            var fullFileName = template.FileName;
            var eventArgs = new ProjectItemGeneratedEventArgs(fullFileName, template.FileContent, ProjectName, PARENT_ITEM_NAME, ProjectItemType.Folder, this, true);
            eventArgs.Properties.Add("BuildAction", 3);
            OnProjectItemGenerated(this, eventArgs);
            var gcEventArgs = new ProjectItemGenerationCompleteEventArgs(this);
            OnGenerationComplete(this, gcEventArgs);

            //Delete the old one
            var delEventArgs = new ProjectItemDeletedEventArgs($@"\{PARENT_ITEM_NAME}\{template.OldFileName}", ProjectName, this);
            delEventArgs.DeleteFile = true;
            OnProjectItemDeleted(this, delEventArgs);
        }

        #endregion

    }
}
