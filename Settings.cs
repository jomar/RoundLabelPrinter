using System;
using System.Collections.Generic;
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
            _previewZoom = LoadFloatSetting("PreviewZoom", 1.0f);
        }

        protected string LoadSetting(string name)
        {
            try
            {
                return Properties.Settings.Default[name] as string;
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }

        protected float LoadFloatSetting(string name, float defaultValue)
        {
            var retval = defaultValue;
            var setting = LoadSetting(name);
            try
            {
                float.TryParse(setting, out retval);
            }
            catch (Exception)
            {
            }
            return retval;
        }

        private void Save()
        {
            Properties.Settings.Default["LastUsedFilename"] = _filename;
            Properties.Settings.Default["PreviewZoom"] = _previewZoom;
            Properties.Settings.Default.Save();
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
            
    }
}
