using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using RoundLabelPrinter;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace RoundLabelPrinter.Forms
{
    public partial class MainWindow : Form
    {
        private List<Entry> _entries;
        private Entry CurrentEntry;

        public MainWindow()
        {
            InitializeComponent();

            _entries = new List<Entry>();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        protected void UpdateListView()
        {
            listBox.Items.Clear();

            foreach(var entry in _entries)
            {
                listBox.Items.Add(entry);
            }
        }

        protected void UpdatePreview()
        {
            var pd = CreateDefaultPrintDocument();
            pd.PrintPage += PrintPage;
            preview.Document = pd;
            preview.Zoom = GetCurrentZoomSetting();
        }

        private float GetCurrentZoomSetting()
        {
            var setting = Settings.Instance.PreviewZoom;
            if (setting <= 0 || setting > 10)
                setting = 0.3f;
            return setting;
        }

        private void SetCurrentZoomSetting(float value)
        {
            Settings.Instance.PreviewZoom = value;
            previewZoom.Text = ((int)(value * 1000)) + "%";
            UpdatePreview();
        }


        const int HORIZONTAL_LABELS = 7;
        const int VERTICAL_LABELS = 10;
        const int HORIZONTAL_MARGIN = 27;
        const int VERTICAL_MARGIN = 17;

        private void PrintPage(object o, PrintPageEventArgs e)
        {
            Pen myPen = new Pen(Color.Gray, 1f);
            DrawTemplateCircles(e);

            int index = 0;
            foreach (var entry in _entries)
            {
                for (int item = 0; item < entry.Count; ++item)
                {
                    DrawEntry(e, entry, index++);
                }
            }
        }

        protected void DrawEntry(PrintPageEventArgs e, Entry entry, int index)
        {
            var nameFont = new Font("Arial", 12f, FontStyle.Bold);
            var abvFont = new Font("Arial", 10.0f);
            var dateFont = new Font("Arial", 10.0f);
            var nameBrush = new SolidBrush(entry.TextColor1);
            var abvBrush = new SolidBrush(entry.TextColor2);
            var dateBrush = new SolidBrush(entry.TextColor3);

            var xPos = GetXPos(e, index % HORIZONTAL_LABELS);
            var yPos = GetYPos(e, index / HORIZONTAL_LABELS);
            var labelWidth = GetLabelWidth(e);
            var labelHeight = GetLabelHeight(e);
            var dateString = entry.Date.ToString("dd.MM.yyyy");
            var abvString = entry.Abv.ToString() + "% ABV";
            var nameSize = TextRenderer.MeasureText(entry.Name, nameFont);
            var abvSize = TextRenderer.MeasureText(abvString, dateFont);
            var dateSize = TextRenderer.MeasureText(dateString, abvFont);
            var topTextMargin = labelHeight / 5;
            var center = new PointF(xPos + (labelWidth / 2), yPos + (labelHeight / 2));

            var image = GetImageWithOpacity(entry.Image, entry.ImageOpacity);
            if (image != null)
            {
                var r = new RectangleF(center.X - (labelWidth/2), center.Y - (labelHeight / 2), labelWidth, labelHeight);
                var path = new GraphicsPath();
                path.AddEllipse(r);
                var oldClip = e.Graphics.Clip;
                e.Graphics.Clip = new Region(path);
                e.Graphics.DrawImage(image, xPos, yPos, labelWidth, labelHeight);
                e.Graphics.Clip = oldClip;
            }

            e.Graphics.DrawString(entry.Name, nameFont, nameBrush, new PointF(xPos + (labelWidth - nameSize.Width) / 2, yPos + topTextMargin));
            e.Graphics.DrawString(abvString, abvFont, abvBrush, new PointF(xPos + (labelWidth - abvSize.Width) / 2, yPos + topTextMargin * 2));
            e.Graphics.DrawString(dateString, dateFont, dateBrush, new PointF(xPos + (labelWidth - dateSize.Width) / 2, yPos + topTextMargin * 3));
        }

        protected Image GetImageWithOpacity(Image image, float opacity)
        {
            if (image == null)
                return null;

            //create a Bitmap the size of the image provided  
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            //create a graphics object from the image  
            using (Graphics gfx = Graphics.FromImage(bmp))
            {

                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix();

                //set the opacity  
                matrix.Matrix33 = opacity;

                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        protected void DrawTemplateCircles(PrintPageEventArgs e)
        {
            Pen myPen = new Pen(Color.LightGray, 0.5f);
            //var w = e.PageBounds.Width;
            //var h = e.PageBounds.Height;
            //var leftMargin = e.MarginBounds.Left;
            //var rightMargin = w - e.MarginBounds.Right;
            //var topMArgin = e.MarginBounds.Top;
            //var bottomMargin = h - e.MarginBounds.Bottom;
            var labelWidth = GetLabelWidth(e);
            var labelHeight = GetLabelHeight(e);
            //labelHeight -= Settings.Instance.VerticalSpacing;
            for (int y = 0; y < VERTICAL_LABELS; ++y)
            {
                for (int x = 0; x < HORIZONTAL_LABELS; ++x)
                {
                    var xPos = GetXPos(e, x);
                    var yPos = GetYPos(e, y);
                    e.Graphics.DrawEllipse(myPen, xPos, yPos, labelWidth, labelHeight);
                }
            }
        }

        protected int GetLabelHeight(PrintPageEventArgs e)
        {
            var h = e.PageBounds.Height;
            var topMargin = e.MarginBounds.Top;
            var bottomMargin = h - e.MarginBounds.Bottom;
            var labelHeight = (h - topMargin - bottomMargin) / VERTICAL_LABELS;
            labelHeight -= Settings.Instance.VerticalSpacing;
            return labelHeight;
        }

        protected int GetLabelWidth(PrintPageEventArgs e)
        {
            var w = e.PageBounds.Width;
            var leftMargin = e.MarginBounds.Left;
            var rightMargin = w - e.MarginBounds.Right;
            var labelWidth = (w - leftMargin - rightMargin) / HORIZONTAL_LABELS;
            labelWidth -= Settings.Instance.HorizontalSpacing;
            return labelWidth;
        }

        protected int GetXPos(PrintPageEventArgs e, int column)
        {
            var leftMargin = e.MarginBounds.Left;
            var w = e.PageBounds.Width;
            var rightMargin = w - e.MarginBounds.Right;
            var labelWidth = (w - leftMargin - rightMargin) / HORIZONTAL_LABELS;
            return leftMargin + (column * labelWidth);
        }

        protected int GetYPos(PrintPageEventArgs e, int row)
        {
            var topMArgin = e.MarginBounds.Top;
            var h = e.PageBounds.Height;
            var bottomMargin = h - e.MarginBounds.Bottom;
            var labelHeight = (h - topMArgin - bottomMargin) / VERTICAL_LABELS;
            return topMArgin + (labelHeight * row);
        }

        protected PrintDocument CreateDefaultPrintDocument()
        {
            PrinterSettings ps = new PrinterSettings();
            IEnumerable<PaperSize> paperSizes = ps.PaperSizes.Cast<PaperSize>();
            PaperSize sizeA4 = paperSizes.First<PaperSize>(size => size.Kind == PaperKind.A4); // setting paper size to A4 size

            var retval = new PrintDocument();
            retval.PrinterSettings = ps;
            retval.DefaultPageSettings.PaperSize = sizeA4;
            retval.DefaultPageSettings.Margins.Top = Settings.Instance.TopMargin;
            retval.DefaultPageSettings.Margins.Bottom = Settings.Instance.BottomMargin;
            retval.DefaultPageSettings.Margins.Left = Settings.Instance.LeftMargin;
            retval.DefaultPageSettings.Margins.Right = Settings.Instance.RightMargin;
            return retval;
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var filename = dlg.FileName;
                if (!string.IsNullOrEmpty(filename))
                {
                    SaveCurrentPage(filename);
                    //var filename = "Label-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    Settings.Instance.LastUsedFilename = filename;
                }
            }
        }

        private void SaveCurrentPage(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Entry>));
            using (TextWriter textWriter = new StreamWriter(filename))
            {
                serializer.Serialize(textWriter, _entries);
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = listBox.SelectedItem as Entry;
            if (selectedItem != null)
            {
                txtName.Text = selectedItem.Name;
                txtAbv.Text = selectedItem.Abv.ToString();
                txtCount.Text = selectedItem.Count.ToString();
                dateTimePicker.Value = selectedItem.Date;
                pictureBox1.Image = selectedItem.Image;
            }
        }

        private void UpdateCurrentEntry()
        {
            if (CurrentEntry == null)
                CurrentEntry = new Entry();
            CurrentEntry.Name = this.txtName.Text;
            CurrentEntry.TextColor1 = this.txtName.ForeColor;
            CurrentEntry.TextColor2 = this.txtAbv.ForeColor;
            CurrentEntry.TextColor3 = this.dateTimePicker.CalendarForeColor;
            CurrentEntry.Abv = float.Parse(this.txtAbv.Text, new CultureInfo("en-US"));
            CurrentEntry.Date = dateTimePicker.Value;
            CurrentEntry.Count = int.Parse(txtCount.Text);
            CurrentEntry.Image = pictureBox1.Image;
            CurrentEntry.ImageOpacity = this.imageAlphaSlider.Value / 100f;
        }

        private void SetDefaultNewEntryValues()
        {
            CurrentEntry = null;

            txtName.Text = "";
            SetInputFieldTextColor(txtName, Color.Black);

            txtAbv.Text = "4.6";
            SetInputFieldTextColor(txtAbv, Color.Black);

            dateTimePicker.Value = DateTime.Now.AddDays(-28);
            dateTimePicker.CalendarForeColor = Color.Black;
            dateTimePicker.CalendarTitleBackColor = Color.White;

            txtCount.Text = "25";
            pictureBox1.Image = null;


            // leave image opacity
        }

        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateCurrentEntry();
                _entries.Add(CurrentEntry);
                listBox.SelectedItem = null;
                SetDefaultNewEntryValues();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            UpdateListView();
            UpdatePreview();
        }

        private void zoomSlider_Scroll(object sender, EventArgs e)
        {
            SetCurrentZoomSetting(zoomSlider.Value / 10f);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void setingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var forms = new RoundLabelPrinter.Forms.SettingsScreen();
            forms.OnSettingsSaved = OnSettingSaved;
            forms.ShowDialog();
        }

        private void OnSettingSaved()
        {
            UpdatePreview();
        }

        private void preview_Click(object sender, EventArgs e)
        {

        }

        private void preview_MouseWheel(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var img = Image.FromFile(dlg.FileName);
                pictureBox1.Image = img;
            }
        }

        private void imageAlphaSlider_Scroll(object sender, EventArgs e)
        {
            UpdateCurrentEntry();
            UpdatePreview();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SetInputFieldTextColor(txtName, dlg.Color);
                UpdateCurrentEntry();
            }
        }

        protected void SetInputFieldTextColor(TextBox textBox, Color textColor)
        {
            textBox.ForeColor = textColor;
            if (textColor.GetBrightness() > 0.5f)
                textBox.BackColor = Color.Black;
            else
                textBox.BackColor = Color.White;
        }

        private void lblAbv_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SetInputFieldTextColor(txtAbv, dlg.Color);
                UpdateCurrentEntry();
            }
        }

        private void lblDate_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                lblDate.ForeColor = dlg.Color;

                UpdateCurrentEntry();
            }
        }
    }
}
