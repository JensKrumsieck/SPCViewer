using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace SPCViewer.Core
{
    public sealed class Settings
    {
        [JsonIgnore]
        private static readonly Lazy<Settings> Lazy = new Lazy<Settings>(() => new Settings());

        /// <summary>
        /// Returns Global instance
        /// </summary>
        /// <returns></returns>
        public static Settings Instance => Lazy.Value;

        #region Font
        public string Font { get; set; }
        public int FontWeight { get; set; }
        public int FontSize { get; set; }
        #endregion

        #region PlotArea
        public double BorderThickness { get; set; }
        public double Padding { get; set; }
        #endregion

        #region Axis
        public string AxisFormat { get; set; }
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        private Settings()
        {
        }

        public void Load(string path = "")
        {
            //TODO: Handle Deserialization
            if (string.IsNullOrEmpty(path))
            {
                // ReSharper disable once InconsistentNaming
                var _default = Default();
                var properties = typeof(Settings).GetProperties();
                foreach (var p in properties.Where(s => s.PropertyType != typeof(Settings))) p.SetValue(this, p.GetValue(_default));
            }
        }

        /// <summary>
        /// Load Default Settings
        /// </summary>
        /// <returns></returns>
        private static Settings Default() =>
            new Settings()
            {
                FontWeight = 200,
                Font = "Arial",
                FontSize = 14,
                BorderThickness = 1.5,
                Padding = 1.5,
                AxisFormat = "{0} / {1}"
            };
    }
}
