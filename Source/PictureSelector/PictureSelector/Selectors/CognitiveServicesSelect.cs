using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using PictureSelector.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PictureSelector.Selectors
{
    public class CognitiveServicesSelect
    {
        private ComputerVisionClient VisionClient;
        private TextAnalyticsClient TextClient;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="config">
        ///     The config object.
        /// </param>
        public CognitiveServicesSelect(ConfigModel config)
        {
            VisionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(config.ComputerVisionServiceKey)){ Endpoint = config.ComputerVisionServiceEndpoint };

            AzureKeyCredential textAnalyticsCredential = new AzureKeyCredential(config.TextAnalyticsServiceKey);
            Uri textAnalyticsCredentialUri = new Uri(config.TextAnalyticsServiceEndpoint);
            TextClient = new TextAnalyticsClient(textAnalyticsCredentialUri, textAnalyticsCredential);
        }

        /// <summary>
        ///     Get image descriptions from the Azure Computer Vision API.
        /// </summary>
        /// <param name="filePaths">
        ///     The paths of the images to describe.
        /// </param>
        /// <returns>
        ///     Return the image descriptions.
        /// </returns>
        public async Task<List<ImageDescriptionWithPathAndSentimentModel>> GetImagesDescriptions(List<string> filePaths)
        {
            List<ImageDescriptionWithPathAndSentimentModel> imageDescriptionsWithPath = new List<ImageDescriptionWithPathAndSentimentModel>();

            foreach (string filePath in filePaths)
            {
                using (Stream fileStream = File.OpenRead(filePath))
                {
                    ImageDescriptionWithPathAndSentimentModel imageDescriptionWithPath = new ImageDescriptionWithPathAndSentimentModel();

                    imageDescriptionWithPath.Description = await VisionClient.DescribeImageInStreamAsync(fileStream, maxCandidates: 1, language: "en");
                    imageDescriptionWithPath.FilePath = filePath;
                    imageDescriptionsWithPath.Add(imageDescriptionWithPath);
                }
            }

            return imageDescriptionsWithPath;
        }

        /// <summary>
        ///     Extract the image with the most positive sentiment based on its description.
        /// </summary>
        /// <param name="imageDescriptionsWithPathAndSentiment">
        ///     The image descriptions including the images paths.
        /// </param>
        /// <returns>
        ///     Returns the description of the most positive image.
        /// </returns>
        public async Task<ImageDescriptionWithPathAndSentimentModel> GetMostPositiveImagePath(List<ImageDescriptionWithPathAndSentimentModel> imageDescriptionsWithPathAndSentiment)
        {
            foreach (ImageDescriptionWithPathAndSentimentModel imageDescriptionWithPathAndSentiment in imageDescriptionsWithPathAndSentiment)
            {
                string imageDescriptionText = imageDescriptionWithPathAndSentiment.Description.Captions[0].Text;
                DocumentSentiment sentimentResult = await TextClient.AnalyzeSentimentAsync(imageDescriptionText);
                imageDescriptionWithPathAndSentiment.Sentiment = sentimentResult;
            }

            List<ImageDescriptionWithPathAndSentimentModel> imageDescriptionsWithPathAndSentimentMostPositive = imageDescriptionsWithPathAndSentiment
                .Where(i => i.Sentiment.ConfidenceScores.Positive == imageDescriptionsWithPathAndSentiment.Max(p => p.Sentiment.ConfidenceScores.Positive))
                .ToList();

            if (imageDescriptionsWithPathAndSentimentMostPositive.Count > 1)
            {
                imageDescriptionsWithPathAndSentimentMostPositive = imageDescriptionsWithPathAndSentimentMostPositive
                    .Where(i => i.Sentiment.ConfidenceScores.Neutral == imageDescriptionsWithPathAndSentimentMostPositive.Max(p => p.Sentiment.ConfidenceScores.Neutral))
                    .ToList();
            }

            if (imageDescriptionsWithPathAndSentimentMostPositive.Count > 1)
            {
                Random random = new Random();
                return imageDescriptionsWithPathAndSentimentMostPositive[random.Next(0, imageDescriptionsWithPathAndSentimentMostPositive.Count())];
            }

            return imageDescriptionsWithPathAndSentimentMostPositive.FirstOrDefault();
        }
    }
}
