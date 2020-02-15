#pragma warning disable 0168
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using nHydrate.Generator.Models;
using nHydrate.DataImport;
using nHydrate.Dsl;
using System.IO;
using nHydrate.DslPackage.Objects;

namespace nHydrate.DslPackage.Forms
{
    public partial class ImportDatabaseForm : Form
    {
        #region Class Members

        public enum ImportReturnConstants
        {
            Aborted,
            Success,
            NoChange,
        }

        private bool _addOnly = false;
        private nHydrateModel _model = null;

        #endregion

        #region Constructor

        public ImportDatabaseForm()
        {
            InitializeComponent();

            var contetextMenu = new ContextMenu();
            var menu = new MenuItem() { Text = "Changes" };
            menu.Click += new EventHandler(ChangeTextMenuClick);
            contetextMenu.MenuItems.Add(menu);
            //txtChanged.ContextMenu = contetextMenu;
            tvwRefresh.AfterSelect += new TreeViewEventHandler(tvwRefresh_AfterSelect);

            grpConnectionStringPostgres.Location = DatabaseConnectionControl1.Location;
            optDatabaseTypeSQL.Click += OptDatabaseTypeSQL_Click;
            optDatabaseTypePostgres.Click += OptDatabaseTypePostgres_Click;
        }

        private void OptDatabaseTypePostgres_Click(object sender, EventArgs e)
        {
            DatabaseConnectionControl1.Visible = false;
            grpConnectionStringPostgres.Visible = true;
        }

        private void OptDatabaseTypeSQL_Click(object sender, EventArgs e)
        {
            DatabaseConnectionControl1.Visible = true;
            grpConnectionStringPostgres.Visible = false;
        }

        public ImportDatabaseForm(
            nHydrateModel model,
            nHydrate.DataImport.Database currentDatabase,
            Microsoft.VisualStudio.Modeling.Shell.ModelingDocData docData)
            : this()
        {
            _model = model;
            this.CurrentDatabase = currentDatabase;

            this.DatabaseConnectionControl1.FileName = Path.Combine((new FileInfo(docData.FileName)).DirectoryName, "importconnection.cache");

            tvwAdd.AfterCheck += new TreeViewEventHandler(treeView1_AfterCheck);
            tvwRefresh.AfterCheck += new TreeViewEventHandler(treeView1_AfterCheck);
            tvwDelete.AfterCheck += new TreeViewEventHandler(treeView1_AfterCheck);

            //tvwAdd.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
            //tvwRefresh.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);
            //tvwDelete.AfterSelect += new TreeViewEventHandler(treeView1_AfterSelect);

            wizard1.BeforeSwitchPages += new nHydrate.Wizard.Wizard.BeforeSwitchPagesEventHandler(wizard1_BeforeSwitchPages);
            wizard1.AfterSwitchPages += new nHydrate.Wizard.Wizard.AfterSwitchPagesEventHandler(wizard1_AfterSwitchPages);
            wizard1.Finish += new EventHandler(wizard1_Finish);
            wizard1.FinishEnabled = false;
            chkSettingPK.CheckedChanged += new EventHandler(chkSettingPK_CheckedChanged);
            cmdTestConnection.Click += new EventHandler(cmdTestConnection_Click);

            this.Settings = new ImportModelSettings();
            this.Settings.OverridePrimaryKey = chkSettingPK.Checked;

            DatabaseConnectionControl1.LoadSettings();

            EnableButtons();
        }

        #endregion

        #region Properties

        private TreeNode SelectedNode
        {
            get
            {
                if (tabControl1.SelectedIndex == 0)
                    return tvwAdd.SelectedNode;
                else if (tabControl1.SelectedIndex == 1)
                    return tvwRefresh.SelectedNode;
                else if (tabControl1.SelectedIndex == 2)
                    return tvwDelete.SelectedNode;
                return null;
            }
        }

        #endregion

        #region Methods

        public nHydrate.DataImport.Database CurrentDatabase { get; set; }
        public nHydrate.DataImport.Database NewDatabase { get; private set; }

        internal ImportModelSettings Settings { get; }
        internal ImportReturnConstants Status { get; private set; }

        private void CreateSummary()
        {
            try
            {
                var BoldStyle = new FastColoredTextBoxNS.TextStyle(null, null, FontStyle.Bold);

                txtSummary.AppendText("Summary\r\n");
                txtSummary.AppendText(string.Empty + "\r\n");

                #region Add

                txtSummary.AppendText("Adding Items\r\n");
                foreach (var item in tvwAdd.Nodes[0].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tEntity: " + item.TargetItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwAdd.Nodes[1].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tView: " + item.TargetItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwAdd.Nodes[2].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tStored Procedure: " + item.TargetItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwAdd.Nodes[3].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tFunction: " + item.TargetItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                txtSummary.AppendText(string.Empty + "\r\n");

                #endregion

                #region Refreshing

                txtSummary.AppendText("Refreshing Items\r\n");
                foreach (var item in tvwRefresh.Nodes[0].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tEntity: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwRefresh.Nodes[1].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tView: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwRefresh.Nodes[2].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tStored Procedure: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwRefresh.Nodes[3].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tFunction: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                txtSummary.AppendText(string.Empty + "\r\n");

                #endregion

                #region Deleting

                txtSummary.AppendText("Deleting Items\r\n");
                foreach (var item in tvwDelete.Nodes[0].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tEntity: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwDelete.Nodes[1].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tView: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwDelete.Nodes[2].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tStored Procedure: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                foreach (var item in tvwDelete.Nodes[3].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList().Select(x => x.Tag).Cast<DataTreeItem>().ToList())
                {
                    txtSummary.AppendText("\tFunction: " + item.SourceItem.Name);
                    txtSummary.AppendText("\r\n");
                }
                txtSummary.AppendText(string.Empty + "\r\n");

                #endregion

                txtSummary.AppendText("Override primary key: " + chkSettingPK.Checked.ToString() + "\r\n");
                txtSummary.AppendText("Assume Inheritance: " + chkInheritance.Checked.ToString() + "\r\n");
                txtSummary.AppendText("Import Relations: " + (!chkIgnoreRelations.Checked).ToString() + "\r\n");
                txtSummary.AppendText("Totals: Adding 0, Refreshing 0, Deleting 0" + "\r\n");

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Populate Methods

        public void Populate()
        {
            this.PopulateAllTrees();
        }

        private void PopulateAllTrees()
        {
            try
            {
                this.tvwAdd.Nodes.Clear();
                this.PopulateAddTree();
                this.PopulateRefreshTree();
                this.PopulateDeleteTree();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void PopulateAddTree()
        {
            try
            {
                this.tvwAdd.Nodes.Clear();

                var tableParentNode = this.tvwAdd.Nodes[this.tvwAdd.Nodes.Add(new TreeNode("Entities"))];
                tableParentNode.Checked = false;
                tableParentNode.ImageIndex = 0;

                var viewParentNode = this.tvwAdd.Nodes[this.tvwAdd.Nodes.Add(new TreeNode("Views"))];
                viewParentNode.Checked = false;
                viewParentNode.ImageIndex = 1;

                var storedProcedureParentNode = this.tvwAdd.Nodes[this.tvwAdd.Nodes.Add(new TreeNode("Stored Procedures"))];
                storedProcedureParentNode.Checked = false;
                storedProcedureParentNode.ImageIndex = 1;

                var functionParentNode = this.tvwAdd.Nodes[this.tvwAdd.Nodes.Add(new TreeNode("Functions"))];
                functionParentNode.Checked = false;
                functionParentNode.ImageIndex = 1;

                this.PopulateDataSchema(tableParentNode);
                this.PopulateViews(viewParentNode);
                this.PopulateStoredProcedures(storedProcedureParentNode);
                this.PopulateFunctions(functionParentNode);


                foreach (TreeNode node in this.tvwAdd.Nodes)
                    this.RemoveAllNodes(node.Nodes, nHydrate.DataImport.ImportStateConstants.Added);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void PopulateRefreshTree()
        {
            try
            {
                this.tvwRefresh.Nodes.Clear();

                var tableParentNode = this.tvwRefresh.Nodes[this.tvwRefresh.Nodes.Add(new TreeNode("Entities"))];
                tableParentNode.Checked = false;
                tableParentNode.ImageIndex = 0;

                var viewParentNode = this.tvwRefresh.Nodes[this.tvwRefresh.Nodes.Add(new TreeNode("Views"))];
                viewParentNode.Checked = false;
                viewParentNode.ImageIndex = 1;

                var storedProcedureParentNode = this.tvwRefresh.Nodes[this.tvwRefresh.Nodes.Add(new TreeNode("Stored Procedures"))];
                storedProcedureParentNode.Checked = false;
                storedProcedureParentNode.ImageIndex = 1;

                var functionParentNode = this.tvwRefresh.Nodes[this.tvwRefresh.Nodes.Add(new TreeNode("Functions"))];
                functionParentNode.Checked = false;
                functionParentNode.ImageIndex = 1;

                this.PopulateDataSchema(tableParentNode);
                this.PopulateViews(viewParentNode);
                this.PopulateStoredProcedures(storedProcedureParentNode);
                this.PopulateFunctions(functionParentNode);

                foreach (TreeNode node in this.tvwRefresh.Nodes)
                    this.RemoveAllNodes(node.Nodes, nHydrate.DataImport.ImportStateConstants.Modified);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void PopulateDeleteTree()
        {
            try
            {
                this.tvwDelete.Nodes.Clear();

                var tableParentNode = this.tvwDelete.Nodes[this.tvwDelete.Nodes.Add(new TreeNode("Entities"))];
                tableParentNode.Checked = false;
                tableParentNode.ImageIndex = 0;

                var viewParentNode = this.tvwDelete.Nodes[this.tvwDelete.Nodes.Add(new TreeNode("Views"))];
                viewParentNode.Checked = false;
                viewParentNode.ImageIndex = 1;

                var storedProcedureParentNode = this.tvwDelete.Nodes[this.tvwDelete.Nodes.Add(new TreeNode("Stored Procedures"))];
                storedProcedureParentNode.Checked = false;
                storedProcedureParentNode.ImageIndex = 1;

                var functionParentNode = this.tvwDelete.Nodes[this.tvwDelete.Nodes.Add(new TreeNode("Functions"))];
                functionParentNode.Checked = false;
                functionParentNode.ImageIndex = 1;

                this.PopulateDataSchema(tableParentNode);
                this.PopulateViews(viewParentNode);
                this.PopulateStoredProcedures(storedProcedureParentNode);
                this.PopulateFunctions(functionParentNode);

                foreach (TreeNode node in this.tvwDelete.Nodes)
                    this.RemoveAllNodes(node.Nodes, nHydrate.DataImport.ImportStateConstants.Deleted);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private void RemoveAllNodes(TreeNodeCollection nodeList, nHydrate.DataImport.ImportStateConstants excludeType)
        {
            var delList = new List<TreeNode>();
            foreach (TreeNode node in nodeList)
            {
                if (((DataTreeItem)node.Tag).State != excludeType)
                {
                    delList.Add(node);
                }
                RemoveAllNodes(node.Nodes, excludeType);
            }

            foreach (var node in delList)
            {
                this.tvwAdd.Nodes.Remove(node);
            }
        }

        private bool AreChanges()
        {
            if (_addOnly)
            {
                return tvwAdd.GetNodeCount(true) > 2;
            }
            else
            {
                return (tvwAdd.GetNodeCount(true) > 2) ||
                       (tvwDelete.GetNodeCount(true) > 2) ||
                       (tvwRefresh.GetNodeCount(true) > 2);
            }
        }

        #endregion

        #region PopulateDataSchema

        private void PopulateDataSchema(TreeNode parentNode)
        {
            try
            {
                var itemCache = new SortedList<string, DataTreeItem>();

                //Current Tables
                foreach (var t in this.CurrentDatabase.EntityList)
                {
                    var dti = new DataTreeItem(t.Name, t) { SourceItem = t };
                    if (this.NewDatabase.EntityList.Count(x => x.Name.ToLower() == t.Name.ToLower()) > 0)
                        dti.Name = this.NewDatabase.EntityList.First(x => x.Name.ToLower() == t.Name.ToLower()).Name;

                    //Check for deleted status
                    if (this.NewDatabase.EntityList.Count(x => x.Name.ToLower() == t.Name.ToLower()) == 0)
                        dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                    else if (this.NewDatabase.EntityList.First(x => x.Name.ToLower() == t.Name.ToLower()).CorePropertiesHash != t.CorePropertiesHash)
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.EntityList.First(x => x.Name.ToLower() == t.Name.ToLower());
                    }
                    else if (this.NewDatabase.EntityList.First(x => x.Name.ToLower() == t.Name.ToLower()).FieldList.GetCorePropertiesHash() != t.FieldList.GetCorePropertiesHash())
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.EntityList.First(x => x.Name.ToLower() == t.Name.ToLower());
                    }

                    if (!itemCache.ContainsKey(t.Name.ToLower()))
                        itemCache.Add(t.Name.ToLower(), dti);
                }

                //Added Tables
                foreach (var t in this.NewDatabase.EntityList)
                {
                    if (this.CurrentDatabase.EntityList.Count(x => x.Name.ToLower() == t.Name.ToLower()) == 0)
                        itemCache.Add(t.Name.ToLower(), new DataTreeItem(t.Name, nHydrate.DataImport.ImportStateConstants.Added) { TargetItem = t });
                }

                //Now add to tree
                foreach (var tableKey in itemCache.Keys)
                {
                    var tableNode = new TreeNode(itemCache[tableKey].Name);
                    tableNode.Checked = false;
                    tableNode.ImageIndex = 3;
                    tableNode.Tag = itemCache[tableKey];

                    parentNode.Nodes.Add(tableNode);

                    var oldTable = this.CurrentDatabase.EntityList.FirstOrDefault(x => x.Name.ToLower() == tableKey.ToLower());
                    var newTable = this.NewDatabase.EntityList.FirstOrDefault(x => x.Name.ToLower() == tableKey.ToLower());
                    if (oldTable == null) oldTable = new nHydrate.DataImport.Entity();
                    if (newTable == null) newTable = new nHydrate.DataImport.Entity();

                    //Create list of all columns (new and old)
                    var columns = new SortedList<string, DataTreeItem>();
                    foreach (var column in oldTable.FieldList)
                    {
                        var dti = new DataTreeItem(column.Name);

                        //Check for deleted status
                        if (this.NewDatabase.EntityList.Count(x => x.Name.ToLower() == itemCache[oldTable.Name.ToLower()].Name.ToLower()) > 0)
                        {
                            var statusTable = this.NewDatabase.EntityList.FirstOrDefault(x => x.Name.ToLower() == itemCache[oldTable.Name.ToLower()].Name.ToLower());
                            if (statusTable != null)
                            {
                                var statusColumn = statusTable.FieldList.FirstOrDefault(x => x.Name.ToLower() == column.Name.ToLower());
                                if (statusColumn == null)
                                    dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                                else if (column.CorePropertiesHash != statusColumn.CorePropertiesHash)
                                    dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                            }
                        }

                        columns.Add(column.Name.ToLower(), dti);

                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region PopulateStoredProcedures

        private void PopulateStoredProcedures(TreeNode parentNode)
        {
            try
            {
                var itemCache = new SortedList<string, DataTreeItem>();

                //Current Items
                foreach (var storedProcedure in this.CurrentDatabase.StoredProcList)
                {
                    var dti = new DataTreeItem(storedProcedure.Name, storedProcedure) { SourceItem = storedProcedure };
                    if (this.NewDatabase.StoredProcList.Contains(storedProcedure.Name))
                        dti.Name = this.NewDatabase.StoredProcList.GetItem(storedProcedure.Name).Name;

                    //Check for deleted status
                    if (!this.NewDatabase.StoredProcList.Contains(storedProcedure.Name))
                        dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                    else if (this.NewDatabase.StoredProcList.GetItem(storedProcedure.Name).CorePropertiesHash != storedProcedure.CorePropertiesHash)
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.StoredProcList.GetItem(storedProcedure.Name);
                    }
                    else if (this.NewDatabase.StoredProcList.GetItem(storedProcedure.Name).FieldList.GetCorePropertiesHash() != storedProcedure.FieldList.GetCorePropertiesHash())
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.StoredProcList.GetItem(storedProcedure.Name);
                    }

                    if (!itemCache.ContainsKey(storedProcedure.Name.ToLower()))
                        itemCache.Add(storedProcedure.Name.ToLower(), dti);
                }

                //Added Items
                foreach (var t in this.NewDatabase.StoredProcList)
                {
                    if (!this.CurrentDatabase.StoredProcList.Contains(t.Name))
                        itemCache.Add(t.Name.ToLower(), new DataTreeItem(t.Name, nHydrate.DataImport.ImportStateConstants.Added) { TargetItem = t });
                }

                //Now add to tree
                foreach (var storedProcedureKey in itemCache.Keys)
                {
                    var newNode = new TreeNode(itemCache[storedProcedureKey].Name);
                    newNode.Checked = false;
                    newNode.Tag = itemCache[storedProcedureKey];
                    parentNode.Nodes.Add(newNode);

                    var oldStoredProcedure = this.CurrentDatabase.StoredProcList.GetItem(storedProcedureKey);
                    var newStoredProcedure = this.NewDatabase.StoredProcList.GetItem(storedProcedureKey);
                    if (oldStoredProcedure == null) oldStoredProcedure = new nHydrate.DataImport.StoredProc();
                    if (newStoredProcedure == null) newStoredProcedure = new nHydrate.DataImport.StoredProc();

                    //Create list of all columns (new and old)
                    var columns = new SortedList<string, DataTreeItem>();
                    foreach (var column in oldStoredProcedure.FieldList)
                    {
                        var dti = new DataTreeItem(column.Name);

                        //Check for deleted status
                        if (this.NewDatabase.StoredProcList.Contains(itemCache[oldStoredProcedure.Name.ToLower()].Name))
                        {
                            var statusColumn = this.NewDatabase.StoredProcList.GetItem(itemCache[oldStoredProcedure.Name.ToLower()].Name).FieldList.FirstOrDefault(x => x.Name == column.Name);
                            if (statusColumn == null)
                                dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                            else if (column.CorePropertiesHash != statusColumn.CorePropertiesHash)
                                dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        }

                        columns.Add(column.Name.ToLower(), dti);

                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region PopulateViews

        private void PopulateViews(TreeNode parentNode)
        {
            try
            {
                var itemCache = new SortedList<string, DataTreeItem>();

                //Current Items
                foreach (var view in this.CurrentDatabase.ViewList)
                {
                    var dti = new DataTreeItem(view.Name, view) { SourceItem = view };
                    if (this.NewDatabase.ViewList.Count(x => x.Name == view.Name) > 0)
                        dti.Name = this.NewDatabase.ViewList.FirstOrDefault(x => x.Name == view.Name).Name;

                    //Check for deleted status
                    if (this.NewDatabase.ViewList.Count(x => x.Name == view.Name) == 0)
                        dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                    else if (this.NewDatabase.ViewList.First(x => x.Name == view.Name).CorePropertiesHash != view.CorePropertiesHash)
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.ViewList.First(x => x.Name == view.Name);
                    }
                    else if (this.NewDatabase.ViewList.First(x => x.Name == view.Name).FieldList.GetCorePropertiesHash() != view.FieldList.GetCorePropertiesHash())
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.ViewList.First(x => x.Name == view.Name);
                    }

                    if (!itemCache.ContainsKey(view.Name.ToLower()))
                        itemCache.Add(view.Name.ToLower(), dti);
                }

                //Added Items
                foreach (var t in this.NewDatabase.ViewList)
                {
                    if (this.CurrentDatabase.ViewList.Count(x => x.Name == t.Name) == 0)
                        itemCache.Add(t.Name.ToLower(), new DataTreeItem(t.Name, nHydrate.DataImport.ImportStateConstants.Added) { TargetItem = t });
                }

                //Now add to tree
                foreach (var viewKey in itemCache.Keys)
                {
                    var viewNode = new TreeNode(itemCache[viewKey].Name);
                    viewNode.Checked = false;
                    viewNode.ImageIndex = 4;
                    viewNode.Tag = itemCache[viewKey];

                    parentNode.Nodes.Add(viewNode);

                    var oldView = this.CurrentDatabase.ViewList.FirstOrDefault(x => x.Name == viewKey);
                    var newView = this.NewDatabase.ViewList.FirstOrDefault(x => x.Name == viewKey);
                    if (oldView == null) oldView = new nHydrate.DataImport.View();
                    if (newView == null) newView = new nHydrate.DataImport.View();

                    //Create list of all columns (new and old)
                    var columns = new SortedList<string, DataTreeItem>();
                    foreach (var column in oldView.FieldList)
                    {
                        var dti = new DataTreeItem(column.Name);

                        //Check for deleted status
                        if (this.NewDatabase.ViewList.Count(x => x.Name == itemCache[oldView.Name.ToLower()].Name) > 0)
                        {
                            var statusColumn = this.NewDatabase.ViewList.First(x => x.Name == itemCache[oldView.Name.ToLower()].Name).FieldList.FirstOrDefault(x => x.Name == column.Name);
                            if (statusColumn == null)
                                dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                            else if (column.CorePropertiesHash != statusColumn.CorePropertiesHash)
                                dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        }

                        columns.Add(column.Name.ToLower(), dti);

                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region PopulateFunctions

        private void PopulateFunctions(TreeNode parentNode)
        {
            try
            {
                var itemCache = new SortedList<string, DataTreeItem>();

                //Current Items
                foreach (var function in this.CurrentDatabase.FunctionList)
                {
                    var dti = new DataTreeItem(function.Name, function) { SourceItem = function };
                    if (this.NewDatabase.FunctionList.Contains(function.Name))
                        dti.Name = this.NewDatabase.FunctionList.GetItem(function.Name).Name;

                    //Check for deleted status
                    if (!this.NewDatabase.FunctionList.Contains(function.Name))
                        dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                    else if (this.NewDatabase.FunctionList.GetItem(function.Name).CorePropertiesHash != function.CorePropertiesHash)
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.FunctionList.GetItem(function.Name);
                    }
                    else if (this.NewDatabase.FunctionList.GetItem(function.Name).FieldList.GetCorePropertiesHash() != function.FieldList.GetCorePropertiesHash())
                    {
                        dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        dti.TargetItem = this.NewDatabase.FunctionList.GetItem(function.Name);
                    }

                    if (!itemCache.ContainsKey(function.Name.ToLower()))
                        itemCache.Add(function.Name.ToLower(), dti);
                }

                //Added Items
                foreach (var t in this.NewDatabase.FunctionList)
                {
                    if (!this.CurrentDatabase.FunctionList.Contains(t.Name))
                        itemCache.Add(t.Name.ToLower(), new DataTreeItem(t.Name, nHydrate.DataImport.ImportStateConstants.Added) { TargetItem = t });
                }

                //Now add to tree
                foreach (var functionKey in itemCache.Keys)
                {
                    var newNode = new TreeNode(itemCache[functionKey].Name);
                    newNode.Checked = false;
                    newNode.Tag = itemCache[functionKey];
                    parentNode.Nodes.Add(newNode);

                    var oldFunction = this.CurrentDatabase.FunctionList.GetItem(functionKey);
                    var newFunction = this.NewDatabase.FunctionList.GetItem(functionKey);
                    if (oldFunction == null) oldFunction = new nHydrate.DataImport.Function();
                    if (newFunction == null) newFunction = new nHydrate.DataImport.Function();

                    //Create list of all columns (new and old)
                    var columns = new SortedList<string, DataTreeItem>();
                    foreach (var column in oldFunction.FieldList)
                    {
                        var dti = new DataTreeItem(column.Name);

                        //Check for deleted status
                        if (this.NewDatabase.FunctionList.Contains(itemCache[oldFunction.Name.ToLower()].Name))
                        {
                            var statusColumn = this.NewDatabase.FunctionList.GetItem(itemCache[oldFunction.Name.ToLower()].Name).FieldList.FirstOrDefault(x => x.Name == column.Name);
                            if (statusColumn == null)
                                dti.State = nHydrate.DataImport.ImportStateConstants.Deleted;
                            else if (column.CorePropertiesHash != statusColumn.CorePropertiesHash)
                                dti.State = nHydrate.DataImport.ImportStateConstants.Modified;
                        }

                        columns.Add(column.Name.ToLower(), dti);

                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #endregion

        #region DataTreeItem

        private class DataTreeItem
        {
            public string Name = string.Empty;
            public nHydrate.DataImport.ImportStateConstants State = nHydrate.DataImport.ImportStateConstants.Unchanged;
            public DatabaseBaseObject SourceItem { get; set; }
            public DatabaseBaseObject TargetItem { get; set; }

            public DataTreeItem(string name)
            {
                this.Name = name;
            }

            public DataTreeItem(string name, nHydrate.DataImport.ImportStateConstants state)
                : this(name)
            {
                this.State = state;
            }

            public DataTreeItem(string name, DatabaseBaseObject source)
                : this(name, nHydrate.DataImport.ImportStateConstants.Unchanged)
            {
                this.SourceItem = source;
            }

            public string GetChangeText()
            {
                if (this.SourceItem == null) return string.Empty;
                if (this.TargetItem == null) return string.Empty;
                return this.SourceItem.GetChangedText(this.TargetItem);
            }

        }

        #endregion

        #region Event Handlers

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = e.Node.Checked;
            }
        }

        //private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        //{
        //  var d = e.Node.Tag as DataTreeItem;
        //  if (d == null) txtChanged.Text = "";
        //  else txtChanged.Text = d.GetChangeText();
        //}

        private void wizard1_BeforeSwitchPages(object sender, nHydrate.Wizard.Wizard.BeforeSwitchPagesEventArgs e)
        {
            if (wizard1.WizardPages[e.OldIndex] == pageConnection)
            {
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    var auditFields = new List<SpecialField>();
                    auditFields.Add(new SpecialField { Name = _model.CreatedByColumnName, Type = SpecialFieldTypeConstants.CreatedBy });
                    auditFields.Add(new SpecialField { Name = _model.CreatedDateColumnName, Type = SpecialFieldTypeConstants.CreatedDate });
                    auditFields.Add(new SpecialField { Name = _model.ModifiedByColumnName, Type = SpecialFieldTypeConstants.ModifiedBy });
                    auditFields.Add(new SpecialField { Name = _model.ModifiedDateColumnName, Type = SpecialFieldTypeConstants.ModifedDate });
                    auditFields.Add(new SpecialField { Name = _model.TimestampColumnName, Type = SpecialFieldTypeConstants.Timestamp });
                    auditFields.Add(new SpecialField { Name = _model.TenantColumnName, Type = SpecialFieldTypeConstants.Tenant });

                    //Setup new model
                    var project = new nHydrate.DataImport.Database();

                    var pkey = ProgressHelper.ProgressingStarted("Importing...", true);
                    try
                    {
                        if (optDatabaseTypeSQL.Checked)
                        {
                            DatabaseConnectionControl1.PersistSettings();
                            var connectionString = DatabaseConnectionControl1.ImportOptions.GetConnectionString();
                            var schemaModelHelper = new nHydrate.DataImport.SqlClient.SchemaModelHelper();

                            if (!schemaModelHelper.IsValidConnectionString(connectionString))
                            {
                                this.Cursor = Cursors.Default;
                                e.Cancel = true;
                                MessageBox.Show("This not a valid connection string!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            var errorCount = LoadSqlServer(connectionString, auditFields);
                            ProgressHelper.ProgressingComplete(pkey);
                            if (errorCount > 0)
                                MessageBox.Show("There were " + errorCount + " stored procedure(s) that could not be imported.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (optDatabaseTypePostgres.Checked)
                        {
                            var connectionString = txtConnectionStringPostgres.Text;

                            if (!DslPackage.Objects.Postgres.ImportDomain.TestConnection(connectionString))
                            {
                                this.Cursor = Cursors.Default;
                                e.Cancel = true;
                                MessageBox.Show("This not a valid connection string!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            var errorCount = LoadPostgres(connectionString, auditFields);
                            ProgressHelper.ProgressingComplete(pkey);
                            if (errorCount > 0)
                                MessageBox.Show("There were " + errorCount + " error on import.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                            MessageBox.Show("Unknown database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        ProgressHelper.ProgressingComplete(pkey);
                    }

                    if (!this.AreChanges())
                    {
                        this.Cursor = Cursors.Default;
                        e.Cancel = true;
                        MessageBox.Show("This modelRoot is up-to-date. There are no changes to refresh.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
            else if (wizard1.WizardPages[e.OldIndex] == pageConnection && wizard1.WizardPages[e.NewIndex] == pageEntities)
            {
            }
            else if (wizard1.WizardPages[e.OldIndex] == pageEntities && wizard1.WizardPages[e.NewIndex] == pageSummary)
            {
                //If there are no entities selected and relations are still checked then prompt
                var nodeCheckedList = tvwAdd.Nodes[0].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList();
                nodeCheckedList.AddRange(tvwRefresh.Nodes[0].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList());
                nodeCheckedList.AddRange(tvwDelete.Nodes[0].Nodes.AsEnumerable<TreeNode>().Where(x => x.Checked).ToList());

                if (nodeCheckedList.Count == 0 && !chkIgnoreRelations.Checked)
                {
                    var result = MessageBox.Show("There are no entities selected but relations will be refreshed. Do you want to turn off relation refreshing?", "Ignore Relations", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        chkIgnoreRelations.Checked = true;
                    }
                    else if (result == System.Windows.Forms.DialogResult.No)
                    {
                        //Do Nothing
                    }
                    else if (result == System.Windows.Forms.DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                //Moving the to the summary page
                CreateSummary();
            }

        }

        private int LoadSqlServer(string connectionString, List<SpecialField> auditFields)
        {
            var importDomain = new nHydrate.DataImport.SqlClient.ImportDomain();
            this.NewDatabase = importDomain.Import(connectionString, auditFields);
            this.NewDatabase.CleanUp();
            this.NewDatabase.SetupIdMap(this.CurrentDatabase);
            //nHydrate.DataImport.ImportDomain.ReMapIDs(this.CurrentDatabase, this.NewDatabase);

            //Remove tenant views
            NewDatabase.ViewList.Remove(x => x.Name.ToLower().StartsWith(_model.TenantPrefix.ToLower() + "_"));

            var errorCount = NewDatabase.StoredProcList.Count(x => x.InError);
            NewDatabase.StoredProcList.Remove(x => x.InError);

            //Load the tree
            this.Populate();

            return errorCount;
        }

        private int LoadPostgres(string connectionString, List<SpecialField> auditFields)
        {
            var tlist = DslPackage.Objects.Postgres.ImportDomain.GetTables(connectionString);
            var db = new DataImport.Database();
            this.NewDatabase = db;

            var allIndexes = DslPackage.Objects.Postgres.ImportDomain.GetIndexes(connectionString);
            var allRelations = DslPackage.Objects.Postgres.ImportDomain.GetRelations(connectionString);

            foreach (var entity in tlist)
            {
                //Create table
                var newEntity = new DataImport.Entity
                {
                    AllowCreateAudit = entity.Columns.Any(x => x.ColumnName == _model.CreatedByColumnName),
                    AllowModifyAudit = entity.Columns.Any(x => x.ColumnName == _model.ModifiedByColumnName),
                    AllowTimestamp = entity.Columns.Any(x => x.ColumnName == _model.TimestampColumnName),
                    IsTenant = entity.Columns.Any(x => x.ColumnName == _model.TenantColumnName),
                    Name = entity.TableName,
                    Schema = entity.SchemaName,
                };
                db.EntityList.Add(newEntity);

                //Load fields
                foreach(var column in entity.Columns)
                {
                    var isIndexed = allIndexes.Any(x => x.TableName == newEntity.Name && newEntity.FieldList.Any(z => z.Name == column.ColumnName) && newEntity.FieldList.Count == 1);
                    newEntity.FieldList.Add(new DataImport.Field
                    {
                        DataType = SqlDbType.BigInt,
                        Identity = column.IsIdentity,
                        IsIndexed = isIndexed,
                        Name = column.ColumnName,
                        Nullable = column.AllowNull,
                        PrimaryKey = false,
                    });
                }

                //TODO: Add multi-column indexes

                #region Load relations

                foreach (var rowRelationship in allRelations)
                {
                    var constraintName = rowRelationship.IndexName;
                    var parentTableName = rowRelationship.TableName;
                    var childTableName = rowRelationship.FKTableName;
                    var parentTable = db.EntityList.FirstOrDefault(x => x.Name == parentTableName);
                    var childTable = db.EntityList.FirstOrDefault(x => x.Name == childTableName);
                    if (parentTable != null && childTable != null)
                    {
                        Relationship newRelation = null;
                        var isAdd = false;
                        if (db.RelationshipList.Count(x => x.ConstraintName == constraintName) == 0)
                        {
                            newRelation = new Relationship();
                            //if (rowRelationship["object_id"] != System.DBNull.Value)
                            //    newRelation.ImportData = rowRelationship["object_id"].ToString();
                            newRelation.SourceEntity = parentTable;
                            newRelation.TargetEntity = childTable;
                            newRelation.ConstraintName = constraintName;
                            var search = ("_" + childTable.Name + "_" + parentTable.Name).ToLower();
                            var roleName = constraintName.ToLower().Replace(search, string.Empty);
                            if (roleName.Length >= 3) roleName = roleName.Remove(0, 3);
                            var v = roleName.ToLower();
                            if (v != "fk") newRelation.RoleName = v;
                            isAdd = true;
                        }
                        else
                        {
                            newRelation = db.RelationshipList.First(x => x.ConstraintName == constraintName);
                        }

                        //add the column relationship to the relation
                        var columnRelationship = new RelationshipDetail();
                        var parentColumnName = rowRelationship.ColumnName;
                        var childColumnName = rowRelationship.FKColumnName;
                        if (parentTable.FieldList.Count(x => x.Name == parentColumnName) == 1 && (childTable.FieldList.Count(x => x.Name == childColumnName) == 1))
                        {
                            columnRelationship.ParentField = parentTable.FieldList.First(x => x.Name == parentColumnName);
                            columnRelationship.ChildField = childTable.FieldList.First(x => x.Name == childColumnName);
                            newRelation.RelationshipColumnList.Add(columnRelationship);

                            //ONLY ADD THIS RELATION IF ALL WENT WELL
                            if (isAdd)
                                parentTable.RelationshipList.Add(newRelation);
                        }
                    }
                }
                #endregion

                this.NewDatabase.CleanUp();
                this.NewDatabase.SetupIdMap(this.CurrentDatabase);

                //Remove tenant views
                NewDatabase.ViewList.Remove(x => x.Name.ToLower().StartsWith(_model.TenantPrefix.ToLower() + "_"));
                NewDatabase.StoredProcList.Remove(x => x.InError);

                //Load the tree
                this.Populate();
            }

            return 0;
        }

        private void wizard1_AfterSwitchPages(object sender, nHydrate.Wizard.Wizard.AfterSwitchPagesEventArgs e)
        {
            wizard1.FinishEnabled = (wizard1.SelectedIndex == wizard1.WizardPages.Count - 1);
        }

        private void wizard1_Finish(object sender, EventArgs e)
        {
            var modifiedState = ImportStateConstants.Modified;

            #region Entities

            var usedItems = new List<string>();
            foreach (var node in tvwAdd.Nodes[0].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.EntityList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Added;
                }
            }

            foreach (var node in tvwRefresh.Nodes[0].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.EntityList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = modifiedState;
                }
            }

            foreach (var node in tvwDelete.Nodes[0].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked) //if NOT checked then used
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    //this.NewDatabase.EntityList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Deleted;
                    this.NewDatabase.EntityList.Add(new nHydrate.DataImport.Entity() { Name = ((DataTreeItem)node.Tag).Name, ImportState = ImportStateConstants.Deleted });
                }
            }

            //Remove all other items from the NewDatbase
            //this.NewDatabase.EntityList.RemoveAll(x => !usedItems.Contains(x.Name.ToLower()));

            #endregion

            #region Views

            usedItems = new List<string>();
            foreach (var node in tvwAdd.Nodes[1].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.ViewList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Added;
                }
            }

            foreach (var node in tvwRefresh.Nodes[1].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.ViewList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = modifiedState;
                }
            }

            foreach (var node in tvwDelete.Nodes[1].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    //this.NewDatabase.ViewList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Deleted;
                    this.NewDatabase.ViewList.Add(new nHydrate.DataImport.View() { Name = ((DataTreeItem)node.Tag).Name, ImportState = ImportStateConstants.Deleted });
                }
            }

            //Remove all other items from the NewDatbase
            //this.NewDatabase.ViewList.RemoveAll(x => !usedItems.Contains(x.Name.ToLower()));

            #endregion

            #region Stored Procedures

            usedItems = new List<string>();
            foreach (var node in tvwAdd.Nodes[2].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.StoredProcList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Added;
                }
            }

            foreach (var node in tvwRefresh.Nodes[2].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.StoredProcList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = modifiedState;
                }
            }

            foreach (var node in tvwDelete.Nodes[2].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    //this.NewDatabase.StoredProcList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Deleted;
                    this.NewDatabase.StoredProcList.Add(new nHydrate.DataImport.StoredProc() { Name = ((DataTreeItem)node.Tag).Name, ImportState = ImportStateConstants.Deleted });
                }
            }

            //Remove all other items from the NewDatbase
            //this.NewDatabase.StoredProcList.RemoveAll(x => !usedItems.Contains(x.Name.ToLower()));

            #endregion

            #region Functions

            usedItems = new List<string>();
            foreach (var node in tvwAdd.Nodes[3].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.FunctionList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Added;
                }
            }

            foreach (var node in tvwRefresh.Nodes[3].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    this.NewDatabase.FunctionList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = modifiedState;
                }
            }

            foreach (var node in tvwDelete.Nodes[3].Nodes.AsEnumerable<TreeNode>())
            {
                if (node.Checked)
                {
                    usedItems.Add(((DataTreeItem)node.Tag).Name.ToLower());
                    //this.NewDatabase.FunctionList.FirstOrDefault(x => x.Name.ToLower() == ((DataTreeItem)node.Tag).Name.ToLower()).ImportState = ImportStateConstants.Deleted;
                    this.NewDatabase.FunctionList.Add(new nHydrate.DataImport.Function() { Name = ((DataTreeItem)node.Tag).Name, ImportState = ImportStateConstants.Deleted });
                }
            }

            //Remove all other items from the NewDatbase
            //this.NewDatabase.FunctionList.RemoveAll(x => !usedItems.Contains(x.Name.ToLower()));

            #endregion

            this.NewDatabase.IgnoreRelations = chkIgnoreRelations.Checked;

            this.Status = ImportReturnConstants.Success;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void cmdTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                //SQL
                if (optDatabaseTypeSQL.Checked)
                {
                    DatabaseConnectionControl1.RefreshOptions();
                    var connectString = DatabaseConnectionControl1.ImportOptions.GetConnectionString();
                    var importDomain = new nHydrate.DataImport.SqlClient.ImportDomain();
                    var databaseHelper = importDomain.DatabaseDomain;

                    var valid = databaseHelper.TestConnectionString(connectString);
                    if (valid)
                    {
                        MessageBox.Show("Connection Succeeded.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The information does not describe a valid connection string.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (optDatabaseTypePostgres.Checked) //Postgres
                {
                    if (DslPackage.Objects.Postgres.ImportDomain.TestConnection(txtConnectionStringPostgres.Text))
                    {
                        MessageBox.Show("Connection Succeeded.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The information does not describe a valid connection string.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Unknown database type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkSettingPK_CheckedChanged(object sender, EventArgs e)
        {
            this.Settings.OverridePrimaryKey = chkSettingPK.Checked;
        }

        private void ChangeTextMenuClick(object sender, EventArgs e)
        {
            var node = SelectedNode;
            if (node != null)
            {
                var d = node.Tag as DataTreeItem;
                if (d == null) return;
                if (d.SourceItem is SQLObject)
                {
                    var F = new DBObjectDifferenceForm(d.SourceItem as SQLObject, d.TargetItem as SQLObject);
                    if (F.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        node.Checked = false;
                    }
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void EnableButtons()
        {
            if (tabControl1.SelectedIndex == 1)
            {
                if (tvwRefresh.SelectedNode != null && tvwRefresh.SelectedNode.Parent != null)
                {
                    //An item (not a folder) is selected in the refresh tree
                    cmdViewDiff.Enabled = true;
                    return;
                }
            }
            cmdViewDiff.Enabled = false;
        }

        private void tvwRefresh_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EnableButtons();
        }

        private void cmdViewDiff_Click(object sender, EventArgs e)
        {
            var _auditFields = new List<SpecialField>();
            var oldItem = (tvwRefresh.SelectedNode.Tag as DataTreeItem).SourceItem as SQLObject;
            var newItem = (tvwRefresh.SelectedNode.Tag as DataTreeItem).TargetItem as SQLObject;

            //Setup new model
            _auditFields.Add(new SpecialField { Name = _model.CreatedByColumnName, Type = SpecialFieldTypeConstants.CreatedBy });
            _auditFields.Add(new SpecialField { Name = _model.CreatedDateColumnName, Type = SpecialFieldTypeConstants.CreatedDate });
            _auditFields.Add(new SpecialField { Name = _model.ModifiedByColumnName, Type = SpecialFieldTypeConstants.ModifiedBy });
            _auditFields.Add(new SpecialField { Name = _model.ModifiedDateColumnName, Type = SpecialFieldTypeConstants.ModifedDate });
            _auditFields.Add(new SpecialField { Name = _model.TimestampColumnName, Type = SpecialFieldTypeConstants.Timestamp });
            _auditFields.Add(new SpecialField { Name = _model.TenantColumnName, Type = SpecialFieldTypeConstants.Tenant });

            var F = new DBObjectDifferenceForm(oldItem, newItem);
            if (F.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
            }

        }

        #endregion

    }
}