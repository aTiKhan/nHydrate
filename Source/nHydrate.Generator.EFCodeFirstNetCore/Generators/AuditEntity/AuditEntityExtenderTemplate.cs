using System.Text;
using nHydrate.Generator.Models;

namespace nHydrate.Generator.EFCodeFirstNetCore.Generators.AuditEntity
{
    public class AuditEntityExtenderTemplate : EFCodeFirstNetCoreBaseTemplate
    {
        private StringBuilder sb = new StringBuilder();
        private Table _currentTable = null;

        public AuditEntityExtenderTemplate(ModelRoot model, Table table)
            : base(model)
        {
            _model = model;
            _currentTable = table;
        }

        public override string FileName => $"{_currentTable.PascalName}Audit.cs";

        public override string FileContent
        {
            get
            {
                GenerateContent();
                return sb.ToString();
            }
        }

        public void GenerateContent()
        {
            sb.AppendLine("namespace " + this.GetLocalNamespace() + ".Audit");
            sb.AppendLine("{");
            sb.AppendLine("	partial class " + _currentTable.PascalName + "Audit");
            sb.AppendLine("	{");
            sb.AppendLine("	}");
            sb.AppendLine("}");
        }

    }
}