# Picture Selector

- This application provides several ways to select pictures from a set of provided pictures

- Ways
    - Random: a picture is randomly selected
    - Cognitive Services: using image description and sentiment analysis to find out the top x most positive pictures


# How to use it

- Create a "Computer Vision" instance in Azure
    - Get subscription key and endpoint

- Create a "Text Analytics" instance in Azure
    - Get subscription key and endpoint

- Go to `./Application`

- Copy the relevant pictures to `./Pictures`
    - Make sure all files in the folder have appropriate file extensions

- Check values of `./config.json` and update where necessary

- Run `PictureSelector.exe` and follow the instructions

- The resulting image is stored in `./Result`


# Resources

- [What are Azure Cognitive Services?](https://docs.microsoft.com/en-us/azure/cognitive-services/what-are-cognitive-services)

- [Vision Quickstart](https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts-sdk/client-library?pivots=programming-language-csharp&tabs=visual-studio)
