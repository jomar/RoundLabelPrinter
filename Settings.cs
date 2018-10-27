using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundLabelPrinter
{
    public class Settings
    {
        private static Settings _instance;

        private Settings()
        {
            Load();
        }

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();
                return _instance;
            }
        }

        private void Load()
        {
            _filename = LoadSetting("LastUsedFilename");
            _previewZoom = LoadFloatSetting("PreviewZoom", 0.6f);
            HorizontalSpacing = LoadIntSetting("HorizontalSpacing", 5);
            VerticalSpacing = LoadIntSetting("HorizontalSpacing", 5);
            LeftMargin = LoadIntSetting("LeftMargin", 50);
            RightMargin = LoadIntSetting("RightMargin", 50);
            TopMargin = LoadIntSetting("TopMargin", 50);
            BottomMargin = LoadIntSetting("BottomMargin", 50);
            GridBackgroundColor = LoadColorSetting("GridBackgroundColor", Color.Gray);
            GridSize = LoadFloatSetting("GridSize", 1.5f);
        }

        protected string LoadSetting(string name)
        {
            try
            {
                var value = Properties.Settings.Default[name];
                if (value is int)
                    return value.ToString();
                else if (value is float)
                    return value.ToString();
                else if (value is Color)
                    return (value as Color?).Value.ToArgb().ToString();
                else
                    return value as string;
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }

        protected int LoadIntSetting(string name, int defaultValue)
        {
            int retval;
            var setting = LoadSetting(name);
            if (!int.TryParse(setting, out retval))
                retval = defaultValue;
            return retval;
        }

        protected Color LoadColorSetting(string name, Color defaultValue)
        {
            Color retval = defaultValue;
            try
            {
                var setting = Properties.Settings.Default[name] as System.Drawing.Color?;
                if (setting != null)
                    retval = setting.Value;
            }
            catch(Exception ex)
            {

            }
            return retval;
        }

        protected float LoadFloatSetting(string name, float defaultValue)
        {
            float retval;
            var setting = LoadSetting(name);
            if (!float.TryParse(setting, out retval))
                retval = defaultValue;
            return retval;
        }

        public void Save()
        {
            Properties.Settings.Default["LastUsedFilename"] = _filename;
            Properties.Settings.Default["PreviewZoom"] = _previewZoom;
            Properties.Settings.Default["VerticalSpacing"] = VerticalSpacing;
            Properties.Settings.Default["HorizontalSpacing"] = HorizontalSpacing;
            Properties.Settings.Default["LeftMargin"] = LeftMargin;
            Properties.Settings.Default["RightMargin"] = RightMargin;
            Properties.Settings.Default["TopMargin"] = TopMargin;
            Properties.Settings.Default["BottomMargin"] = BottomMargin;
            Properties.Settings.Default["GridBackgroundColor"] = GridBackgroundColor;
            Properties.Settings.Default["GridSize"] = GridSize;

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private string _filename;
        public string LastUsedFilename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                Save();
            }
        }

        private float _previewZoom;
        public float PreviewZoom
        {
            get { return _previewZoom; }
            set
            {
                _previewZoom = value;
                Save();
            }
        }

        public float GridSize { get; set; }
        public Color GridBackgroundColor { get; set; }
        public int VerticalSpacing { get; set; }
        public int HorizontalSpacing { get; set; }
        public int LeftMargin { get; set; }
        public int RightMargin { get; set; }
        public int TopMargin { get; set; }
        public int BottomMargin { get; set; }
    }
}
