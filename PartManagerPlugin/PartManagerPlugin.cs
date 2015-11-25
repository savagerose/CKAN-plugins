using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CKAN;
using Version = CKAN.Version;

namespace PartManagerPlugin
{

    public class PartManagerConfig
    {
        public List<KeyValuePair<string, string>> DisabledParts;
    }

    public class PartManagerPlugin : IGUIPlugin
    {

        private readonly Version _version = new Version("v1.1.0");

        private PartManagerUI _mUi = null;

        public override void Initialize()
        {
            var tabPage = new TabPage {Name = "Part Manager", Text = "Part Manager"};

	        _mUi = new PartManagerUI {Dock = DockStyle.Fill};
	        tabPage.Controls.Add(_mUi);

			Main.modChangedCallback += _mUi.OnModChanged;
            Main.Instance.m_TabController.m_TabPages.Add("Part Manager", tabPage);
            Main.Instance.m_TabController.ShowTab("Part Manager", 1, false);
        }

        public override void Deinitialize()
        {
			Main.modChangedCallback -= _mUi.OnModChanged;
            Main.Instance.m_TabController.HideTab("Part Manager");
            Main.Instance.m_TabController.m_TabPages.Remove("Part Manager");
        }

        public override string GetName()
        {
            return "Part Manager by nlight" + Environment.NewLine + "Updated by Gribbleshnibit8";
        }

        public override Version GetVersion()
        {
            return _version;
        }

    }

}
