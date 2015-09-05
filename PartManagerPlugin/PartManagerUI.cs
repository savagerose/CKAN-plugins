using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CKAN;
using Newtonsoft.Json;

namespace PartManagerPlugin
{

    public enum FilterType
    {
        Path,
        Name,
        Title
    }

    public partial class PartManagerUI : UserControl
    {

        private readonly Dictionary<string, ConfigNode> _mDisabledParts = new Dictionary<string, ConfigNode>();

	    private const string ConfigPath = "PartManager/PartManager.json";

	    private string _mFilter = null;
        private bool _mFilterRegex = false;
        private FilterType _mFilterType;
	    private readonly List<PartViewer> _partViewers = new List<PartViewer>();

        private void LoadConfig()
        {
            var fullPath = Path.Combine(Main.Instance.CurrentInstance.CkanDir(), ConfigPath);
            if (!File.Exists(fullPath))
            {
                return;
            }

            var partManagerPath = Path.Combine(Main.Instance.CurrentInstance.CkanDir(), "PartManager");
            if (!Directory.Exists(partManagerPath))
            {
                Directory.CreateDirectory(partManagerPath);
            }

            var cachePath = Path.Combine(partManagerPath, "cache");
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var json = File.ReadAllText(fullPath);
            var config = (PartManagerConfig) JsonConvert.DeserializeObject<PartManagerConfig>(json);
            foreach (var item in config.DisabledParts)
            {
                _mDisabledParts.Add(item.Key, ConfigNodeReader.FileToConfigNode(Path.Combine(cachePath, item.Key)));
            }
        }

        private void SaveConfig()
        {
            var fullPath = Path.Combine(Main.Instance.CurrentInstance.CkanDir(), ConfigPath);
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            var config = new PartManagerConfig();
            config.DisabledParts = new List<KeyValuePair<string, string>>();
            foreach (var part in _mDisabledParts)
            {
                config.DisabledParts.Add(new KeyValuePair<string, string>(part.Key, null));
            }

            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText(fullPath, json);
        }

        public PartManagerUI()
        {
            InitializeComponent();
        }

        private void PartManagerUI_Load(object sender, EventArgs e)
        {
            LoadConfig();
            RefreshInstalledModsList();
        }

        public void OnModChanged(Module module, GUIModChangeType changeType)
        {
            if (changeType == GUIModChangeType.Update || changeType == GUIModChangeType.Install)
            {
	            var parts = GetInstalledModParts(module.identifier);
	            foreach (var part in parts.Where(part => _mDisabledParts.ContainsKey(part.Key)))
	            {
		            Cache.RemovePartFromCache(part.Key);
		            Cache.MovePartToCache(part.Key);
	            }
            }

	        RefreshInstalledModsList();
        }

        private void RefreshInstalledModsList()
        {
            var installedMods = Main.Instance.CurrentInstance.Registry.Installed();

            InstalledModsListBox.Items.Clear();

            foreach (var mod in installedMods)
            {
                var parts = GetInstalledModParts(mod.Key);
	            if (parts != null && parts.Any())
		            InstalledModsListBox.Items.Add(String.Format("{0} | {1}", mod.Key, mod.Value));
            }
        }

        private Dictionary<string, ConfigNode> GetInstalledModParts(string identifier)
        {
            var registry = Main.Instance.CurrentInstance.Registry;
            var module = registry.InstalledModule(identifier);

	        if (module == null)
		        return null;

	        var parts = new Dictionary<string, ConfigNode>();

            foreach (var item in module.Files)
            {
                if (_mDisabledParts.ContainsKey(item))
                {
                    parts.Add(item, _mDisabledParts[item]);
                    continue;
                }

                var filename = Path.GetFileName(item);

	            if (filename != null && !filename.EndsWith(".cfg")) continue;

	            var configNode = LoadPart(item);
	            if (configNode != null)
		            parts.Add(item, configNode);
            }

            return parts;
        }

        private ConfigNode LoadPart(string part)
        {
            var kspDir = Main.Instance.CurrentInstance.GameDir();
            var fullPath = Path.Combine(kspDir, part);
            if (!File.Exists(fullPath))
            {
                var partManagerPath = Path.Combine(Main.Instance.CurrentInstance.CkanDir(), "PartManager");
                var cachePath = Path.Combine(partManagerPath, "cache");
                fullPath = Path.Combine(cachePath, part);
	            if (!File.Exists(fullPath))
		            return null;
            }

            var configNode = ConfigNodeReader.FileToConfigNode(fullPath);
            return configNode;
        }

        private void InstalledModsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
	        if (InstalledModsListBox.SelectedItems.Count == 0)
		        return;

	        PartsGridView.Rows.Clear();

            foreach (var selectedItem in InstalledModsListBox.SelectedItems)
            {
                var item = (selectedItem as string).Split('|');
                var identifier = item[0].Trim();

                var parts = GetInstalledModParts(identifier);

                foreach (var part in parts)
                {
	                if (_mFilterType == FilterType.Path && !FilterString(part.Key))
		                continue;

	                var row = new DataGridViewRow();
                    row.Tag = part;

                    var enabledCheckbox = new DataGridViewCheckBoxCell();
                    enabledCheckbox.Value = !_mDisabledParts.ContainsKey(part.Key);
                    row.Cells.Add(enabledCheckbox);

                    var titleTextbox = new DataGridViewTextBoxCell();
                    var title = part.Value.GetValue("title");

	                if (_mFilterType == FilterType.Title && !FilterString(title))
		                continue;

	                titleTextbox.Value = title;

                    row.Cells.Add(titleTextbox);

                    var nameTextbox = new DataGridViewTextBoxCell {Value = part.Value.GetValue("name")};
	                row.Cells.Add(nameTextbox);

	                if (_mFilterType == FilterType.Name && !FilterString(part.Value.GetValue("name")))
		                continue;

	                var pathTextbox = new DataGridViewTextBoxCell {Value = part.Key};
	                row.Cells.Add(pathTextbox);

                    PartsGridView.Rows.Add(row);
                }
            }
        }

        private bool FilterString(string value)
        {
	        if (_mFilter == null)
		        return true;

	        if (value == null)
		        return false;

	        if (_mFilter.Length == 0)
		        return true;

	        if (!_mFilterRegex) 
				return value.ToLower().Contains(_mFilter.ToLower());
	        try
	        {
		        return Regex.IsMatch(value, _mFilter);
	        }
	        catch (Exception)
	        {
		        return false;
	        }
        }

        private void PartsGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
	        if (e.RowIndex < 0) return;
	        if (e.ColumnIndex < 0) return;

	        var grid = sender as DataGridView;
            var row = grid.Rows[e.RowIndex];
            var columnIndex = e.ColumnIndex;

	        if (columnIndex != 0) return;

	        var part = (KeyValuePair<string, ConfigNode>) row.Tag;

            var gridViewCell = row.Cells[columnIndex] as DataGridViewCheckBoxCell;
            var state = (bool)gridViewCell.Value;
            if (state == false)
            {
	            if (_mDisabledParts.ContainsKey(part.Key)) return;

	            _mDisabledParts.Add(part.Key, part.Value);
                Cache.MovePartToCache(part.Key);
                SaveConfig();
            }
            else
            {
	            if (!_mDisabledParts.ContainsKey(part.Key)) return;

	            _mDisabledParts.Remove(part.Key);
                Cache.MovePartFromCache(part.Key);
                SaveConfig();
            }
        }

	    private void PartsGridView_KeyUp(object sender, KeyEventArgs e)
	    {
		    var grid = sender as DataGridView;
			var checkBoxes = (from DataGridViewRow selectedRow in grid.SelectedRows select selectedRow.Cells[0] as DataGridViewCheckBoxCell).ToList();

		    switch (e.KeyCode)
		    {
			    case Keys.Space:
				    foreach (var box in checkBoxes)
					    box.Value = (bool) box.Value == false;
				    break;
		    }
	    }

		private void PartsGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;

			var grid = sender as DataGridView;
			var row = grid.Rows[e.RowIndex];

			var part = (KeyValuePair<string, ConfigNode>)row.Tag;

			var isDisabled = _mDisabledParts.ContainsKey(part.Key);

			_partViewers.Add(new PartViewer(part, isDisabled));
			if (_partViewers.Last().FileFound)
				_partViewers.Last().Show();

			for (var index = 0; index < _partViewers.Count; index++)
			{
				if (_partViewers[index].IsDisposed)
					_partViewers.Remove(_partViewers[index]);
			}
		}

	    private void ApplyFilterButton_Click(object sender, EventArgs e)
        {
            _mFilter = FilterTextBox.Text;
            _mFilterRegex = RegexCheckbox.Checked;
            ClearFilterbutton.Enabled = true;

            try
            {
                _mFilterType = (FilterType)Enum.Parse(typeof(FilterType), FilterTypeCombobox.Text, true);
            }
            catch (Exception)
            {
                FilterTypeCombobox.Text = "Path";
                _mFilterType = FilterType.Path;                
            }
            InstalledModsListBox_SelectedIndexChanged(null, new EventArgs());
        }

        private void ClearFilterbutton_Click(object sender, EventArgs e)
        {
            ClearFilterbutton.Enabled = false;
            _mFilter = null;
            InstalledModsListBox_SelectedIndexChanged(null, new EventArgs());
        }

        private void EnableAllButton_Click(object sender, EventArgs e)
        {
	        foreach (DataGridViewRow row in PartsGridView.Rows)
		        (row.Cells[0] as DataGridViewCheckBoxCell).Value = true;
        }

        private void DisableAllButton_Click(object sender, EventArgs e)
        {
	        foreach (DataGridViewRow row in PartsGridView.Rows)
		        (row.Cells[0] as DataGridViewCheckBoxCell).Value = false;
		}

		private void ClosePartWindowsButton_Click(object sender, EventArgs e)
		{
			foreach (var partViewer in _partViewers)
			{
				partViewer.Close();
			}
		}


    }
}
