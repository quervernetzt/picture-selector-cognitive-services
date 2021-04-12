using Azure.AI.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace PictureSelector.Models
{
    public class ImageDescriptionWithPathAndSentimentModel
    {
        public ImageDescription Description { get; set; }

        public DocumentSentiment Sentiment { get; set; }

        public string FilePath { get; set; }
    }
}
