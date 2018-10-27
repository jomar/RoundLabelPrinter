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
            txtLeftMargin.Text = Settings.Instance.LeftMargin.ToString();
            txtRightMargin.Text = Settings.Instance.RightMargin.ToString();
            txtBottomMargin.Text = Settings.Instance.BottomMargin.ToString();
            txtTopMargin.Text = Settings.Instance.TopMargin.ToString();
            bckColorWidget.BackColor = Settings.Instance.GridBackgroundColor;
            txtGridSize.Text = Settings.Instance.GridSize.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Instance.HorizontalSpacing = int.Parse(txtHorizontalSpacing.Text);
            Settings.Instance.VerticalSpacing = int.Parse(txtVerticalSpacing.Text);
            Settings.Instance.LeftMargin = int.Parse(txtLeftMargin.Text);
            Settings.Instance.RightMargin = int.Parse(txtRightMargin.Text);
            Settings.Instance.TopMargin = int.Parse(txtTopMargin.Text);
            Settings.Instance.BottomMargin = int.Parse(txtBottomMargin.Text);
            Settings.Instance.GridBackgroundColor = bckColorWidget.BackColor;
            Settings.Instance.GridSize = float.Parse(txtGridSize.Text, System.Globalization.NumberStyles.AllowDecimalPoint);
            Settings.Instance.Save();

            if (OnSettingsSaved != null)
                OnSettingsSaved.Invoke();
        }

        private void txtLeftMargin_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void bckColor_Clicked(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = bckColorWidget.BackColor;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                bckColorWidget.BackColor = dlg.Color;
            }
        }

        private void bckColorWidget_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
