using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CKAN;

namespace PartManagerPlugin
{
	public partial class PartViewer : Form
	{
		public bool FileFound = false;

		public PartViewer()
		{
			InitializeComponent();
		}

		public PartViewer(KeyValuePair<string, ConfigNode> part, bool disabled)
		{
			string fullPath;

			if (disabled)
				fullPath = Path.Combine(Main.Instance.CurrentInstance.CkanDir(), "PartManager", "cache", part.Key);
			else
				fullPath = Path.Combine(Main.Instance.CurrentInstance.GameDir(), part.Key);

			if (!File.Exists(fullPath)) return;
			FileFound = true;

			InitializeComponent();
			base.Text = part.Value.GetValue("name") + part.Value.GetValue("title");

			using (var sr = new StreamReader(fullPath))
			{
				PartViewTextBox.AppendText(sr.ReadToEnd());
			}
		}
	}
}
