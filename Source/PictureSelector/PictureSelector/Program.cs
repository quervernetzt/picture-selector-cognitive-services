using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PictureSelector.Models;
using PictureSelector.Selectors;
using PictureSelector.Util;

namespace PictureSelector
{
    public class Program
    {
        private static ConfigModel Config;

        /// <summary>
        ///     Main method.
        /// </summary>
        /// <param name="args">
        ///     Arguments passed to the main method.
        /// </param>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the Picture Selection App...\n");

            string baseDirectory = AppContext.BaseDirectory;
            string configFullFilePath = $"{baseDirectory}config.json";
            string picturesFullPath = $"{baseDirectory}Pictures";
            string resultFullPath = $"{baseDirectory}Result";

            BuildConfigObject(configFullFilePath);
            List<string> picturePaths = FileOperations.GetFilePaths(picturesFullPath, Config.AllowedPictureFormats);
            Console.WriteLine($"Working with { picturePaths.Count } pictures in '{ picturesFullPath }'...\n");

            string resultPath = "";

            Console.WriteLine("Do you want to select the picture randomly or using Cognitive Services? Please enter 'random' or 'cognitive'...");
            string selectType = Console.ReadLine().ToLower();

            if (selectType == "random")
            {
                // Random Selection
                Console.WriteLine("You selected the random selection...\n");
                RandomSelect randomSelect = new RandomSelect();
                List<string> randomSelectResults = randomSelect.GetTopRandomFiles(picturePaths, 1);
                resultPath = randomSelectResults.First();

                Console.WriteLine($"The picture randomly selected is: '{ resultPath }'...\n");
            }
            else if (selectType == "cognitive")
            {
                // Cognitive Services Selection based on image description and sentiment analysis
                Console.WriteLine("You selected the cognitive selection. Depending on the number and size of pictures this may take a while...\n");
                CognitiveServicesSelect cognitiveServicesSelect = new CognitiveServicesSelect(Config);
                List<ImageDescriptionWithPathAndSentimentModel> imageDescriptions = await cognitiveServicesSelect.GetImagesDescriptions(picturePaths);
                ImageDescriptionWithPathAndSentimentModel imageDescriptionsWithPathAndSentimentMostPositive = await cognitiveServicesSelect.GetMostPositiveImagePath(imageDescriptions);
                resultPath = imageDescriptionsWithPathAndSentimentMostPositive.FilePath;

                Console.WriteLine($"The picture selected using Cognitive Services has the path '{ imageDescriptionsWithPathAndSentimentMostPositive.FilePath }', " +
                    $"the description '{  imageDescriptionsWithPathAndSentimentMostPositive.Description.Captions[0].Text }' " +
                    $"and the following sentiment scores:");
                Console.WriteLine($"Positive score: {imageDescriptionsWithPathAndSentimentMostPositive.Sentiment.ConfidenceScores.Positive:0.00}");
                Console.WriteLine($"Negative score: {imageDescriptionsWithPathAndSentimentMostPositive.Sentiment.ConfidenceScores.Negative:0.00}");
                Console.WriteLine($"Neutral score: {imageDescriptionsWithPathAndSentimentMostPositive.Sentiment.ConfidenceScores.Neutral:0.00}\n");
            }
            else
            {
                Console.WriteLine("You didn't provide a valid choice. Goodbye...");
                return;
            }

            Console.WriteLine("Writing result to Result folder...\n");
            FileOperations.WriteResultToFolder(resultFullPath, resultPath);

            Console.WriteLine("Done and Goodbye...");

            Console.ReadKey();
        }

        /// <summary>
        ///     Map config.json to ConfigModel object.
        /// </summary>
        /// <param name="configFullFilePath">
        ///     The full file path of the config.json file.
        /// </param>
        private static void BuildConfigObject(string configFullFilePath)
        {
            string configFileContent = File.ReadAllText(configFullFilePath);
            Config = JsonSerializer.Deserialize<ConfigModel>(configFileContent);
        }
    }
}
