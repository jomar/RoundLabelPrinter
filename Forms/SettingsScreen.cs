using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoundLabelPrinter.Forms
{
    public partial class SettingsScreen : Form
    {
        public SettingsScreen()
        {
            InitializeComponent();

            LoadValues();
        }

        public Action OnSettingsSaved { get; set; }

        private void LoadValues()
        {
            txtHorizontalSpacing.Text = Settings.Instance.HorizontalSpacing.ToString();
            txtVerticalSpacing.Text = Settings.Instance.VerticalSpacing.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Instance.HorizontalSpacing = int.Parse(txtHorizontalSpacing.Text);
            Settings.Instance.VerticalSpacing = int.Parse(txtVerticalSpacing.Text);
            Settings.Instance.Save();

            if (OnSettingsSaved != null)
                OnSettingsSaved.Invoke();
        }
    }
}
