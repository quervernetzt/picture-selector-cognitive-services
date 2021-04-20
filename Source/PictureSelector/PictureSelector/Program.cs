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

            List<ImageDescriptionExtended> resultingImages = new List<ImageDescriptionExtended>();

            Console.WriteLine("Do you want to select the picture randomly or using Cognitive Services? Please enter 'random' or 'cognitive'...");
            string selectType = Console.ReadLine().ToLower();

            if (selectType == "random")
            {
                // Random Selection
                Console.WriteLine("You selected the random  selection. How many pictures shall be selected? (please insert a valid integer)");
                int numberOfPictures = Int32.Parse(Console.ReadLine());
                Console.WriteLine($"You entered '{ numberOfPictures }'...\n");

                RandomSelect randomSelect = new RandomSelect();
                resultingImages = randomSelect.GetTopRandomFiles(picturePaths, numberOfPictures);
            }
            else if (selectType == "cognitive")
            {
                // Cognitive Services Selection based on image description and sentiment analysis
                Console.WriteLine("You selected the cognitive selection. How many pictures shall be selected? (please insert a valid integer)");
                int numberOfPictures = Int32.Parse(Console.ReadLine());
                Console.WriteLine($"You entered '{ numberOfPictures }'. Depending on the number and size of all pictures this may take a while...\n");

                CognitiveServicesSelect cognitiveServicesSelect = new CognitiveServicesSelect(Config);
                List<ImageDescriptionExtended> imageDescriptions = await cognitiveServicesSelect.GetImagesDescriptions(picturePaths);
                resultingImages = 
                    await cognitiveServicesSelect.GetTopMostPositiveImages(imageDescriptions, numberOfPictures);
            }
            else
            {
                Console.WriteLine("You didn't provide a valid choice. Goodbye...");
                return;
            }

            if (resultingImages.Count > 0)
            {
                Console.WriteLine("Writing results to Result folder...\n");

                List<string> resultPaths = resultingImages.Select(i => i.FilePath).ToList();
                FileOperations.WriteResultsToFolder(resultFullPath, resultPaths);
            }
            else
            {
                Console.WriteLine("No pictures to write...");
            }

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
