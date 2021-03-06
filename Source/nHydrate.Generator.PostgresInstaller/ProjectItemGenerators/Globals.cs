#pragma warning disable 0168
using nHydrate.Generator.Common.Models;
using nHydrate.Generator.Common.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nHydrate.Generator.PostgresInstaller.ProjectItemGenerators
{
    internal static class Globals
    {
        public static string GetDateTimeNowCode(ModelRoot model)
        {
            return model.UseUTCTime ? "DateTime.UtcNow" : "DateTime.Now";
        }

        public static string BuildSelectList(Table table, ModelRoot model)
        {
            return BuildSelectList(table, model, false);
        }

        public static string BuildSelectList(Table table, ModelRoot model, bool useFullHierarchy)
        {
            var index = 0;
            var output = new StringBuilder();
            var columnList = new List<Column>();
            if (useFullHierarchy)
                columnList.AddRange(table.GetColumnsFullHierarchy().OrderBy(x => x.Name));
            else
                columnList.AddRange(table.GetColumns());

            foreach (var column in columnList.OrderBy(x => x.Name))
            {
                output.AppendFormat("\t[{2}].[{0}].[{1}]", column.ParentTable.DatabaseName, column.DatabaseName, column.ParentTable.GetPostgresSchema());
                if ((index < columnList.Count - 1) || (table.AllowCreateAudit) || (table.AllowModifiedAudit) || (table.AllowConcurrencyCheck))
                    output.Append(",");
                output.AppendLine();
                index++;
            }

            if (table.AllowCreateAudit)
            {
                output.AppendFormat("	[{2}].[{0}].[{1}],", table.DatabaseName, model.Database.CreatedByColumnName, table.GetPostgresSchema());
                output.AppendLine();

                output.AppendFormat("	[{2}].[{0}].[{1}]", table.DatabaseName, model.Database.CreatedDateColumnName, table.GetPostgresSchema());
                if ((table.AllowModifiedAudit) || (table.AllowConcurrencyCheck))
                    output.Append(",");
                output.AppendLine();
            }

            if (table.AllowModifiedAudit)
            {
                output.AppendFormat("	[{2}].[{0}].[{1}],", table.DatabaseName, model.Database.ModifiedByColumnName, table.GetPostgresSchema());
                output.AppendLine();

                output.AppendFormat("	[{2}].[{0}].[{1}]", table.DatabaseName, model.Database.ModifiedDateColumnName, table.GetPostgresSchema());
                if (table.AllowConcurrencyCheck)
                    output.Append(",");
                output.AppendLine();
            }

            if (table.AllowConcurrencyCheck)
            {
                output.AppendFormat("	[{2}].[{0}].[{1}]", table.GetAbsoluteBaseTable().DatabaseName, model.Database.ConcurrencyCheckColumnName, table.GetAbsoluteBaseTable().GetPostgresSchema());
                output.AppendLine();
            }

            return output.ToString();
        }

        public static Column GetColumnByKey(ReferenceCollection referenceCollection, string columnKey)
            => referenceCollection.Select(x => x.Object as Column).Where(x => x != null).Where(x => x.Key.Match(columnKey)).FirstOrDefault();

        public static void AppendCreateAudit(Table table, ModelRoot model, StringBuilder sb)
        {
            sb.AppendLine($"--APPEND AUDIT TRAIL CREATE FOR TABLE [{table.DatabaseName}]");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" ADD COLUMN IF NOT EXISTS \"{model.Database.CreatedByColumnName}\" Varchar (50) NULL;");
            var dfName = $"DF__{table.DatabaseName}_{model.Database.CreatedDateColumnName}".ToUpper();
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" ADD COLUMN IF NOT EXISTS \"{model.Database.CreatedDateColumnName}\" timestamp CONSTRAINT \"" + dfName + "\" DEFAULT current_timestamp NULL;");
            sb.AppendLine("--GO");
            sb.AppendLine();
        }

        public static void AppendModifiedAudit(Table table, ModelRoot model, StringBuilder sb)
        {
            sb.AppendLine($"--APPEND AUDIT TRAIL MODIFY FOR TABLE [{table.DatabaseName}]");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" ADD COLUMN IF NOT EXISTS \"{model.Database.ModifiedByColumnName}\" Varchar (50) NULL;");
            var dfName = $"DF__{table.DatabaseName}_{model.Database.ModifiedDateColumnName}".ToUpper();
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" ADD COLUMN IF NOT EXISTS \"{model.Database.ModifiedDateColumnName}\" timestamp CONSTRAINT \"" + dfName + "\" DEFAULT current_timestamp NULL;");
            sb.AppendLine("--GO");
            sb.AppendLine();
        }

        public static void AppendConcurrencyAudit(Table table, ModelRoot model, StringBuilder sb)
        {
            sb.AppendLine($"--APPEND AUDIT TRAIL CONCURRENCY FOR TABLE [{table.DatabaseName}]");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" ADD COLUMN IF NOT EXISTS \"" + model.Database.ConcurrencyCheckColumnName + "\" int NOT NULL;");
            sb.AppendLine("--GO");
            sb.AppendLine();
        }
        public static void DropCreateAudit(Table table, ModelRoot model, StringBuilder sb)
        {
            sb.AppendLine($"--REMOVE AUDIT TRAIL CREATE FOR TABLE [{table.DatabaseName}]");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" DROP COLUMN IF EXISTS \"{model.Database.CreatedByColumnName}\";");
            var dfName = $"DF__{table.DatabaseName}_{model.Database.CreatedDateColumnName}".ToUpper();
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" DROP CONSTRAINT IF EXISTS \"{dfName}\";");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" DROP COLUMN IF EXISTS \"{model.Database.CreatedDateColumnName}\";");
            sb.AppendLine("--GO");
            sb.AppendLine();
        }

        public static void DropModifiedAudit(Table table, ModelRoot model, StringBuilder sb)
        {
            sb.AppendLine($"--REMOVE AUDIT TRAIL MODIFY FOR TABLE [{table.DatabaseName}]");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" DROP COLUMN IF EXISTS \"{model.Database.ModifiedByColumnName}\";");
            var dfName = $"DF__{table.DatabaseName}_{model.Database.ModifiedDateColumnName}".ToUpper();
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" DROP CONSTRAINT IF EXISTS \"{dfName}\";");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" DROP COLUMN IF EXISTS \"{model.Database.ModifiedDateColumnName}\";");
            sb.AppendLine("--GO");
            sb.AppendLine();
        }

        public static void DropTimestampAudit(Table table, ModelRoot model, StringBuilder sb)
        {
            sb.AppendLine($"--REMOVE AUDIT TRAIL CONCURRENCY FOR TABLE [{table.DatabaseName}]");
            sb.AppendLine($"ALTER TABLE {table.GetPostgresSchema()}.\"{table.DatabaseName}\" DROP COLUMN IF EXISTS \"{model.Database.ConcurrencyCheckColumnName}\";");
            sb.AppendLine("--GO");
            sb.AppendLine();
        }

    }
}
