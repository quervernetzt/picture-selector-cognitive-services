using System.Collections.Generic;

namespace PictureSelector.Models
{
    public class ConfigModel
    {
        public List<string> AllowedPictureFormats { get; set; }

        public string ComputerVisionServiceEndpoint { get; set; }

        public string ComputerVisionServiceKey { get; set; }

        public string TextAnalyticsServiceEndpoint { get; set; }

        public string TextAnalyticsServiceKey { get; set; }
    }
}
