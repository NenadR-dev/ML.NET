using DeepLearning_ImageClassification_UI.Model;
using Microsoft.ML;
using Microsoft.ML.Vision;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ML.DataOperationsCatalog;
//FIX TRAINING
namespace DeepLearning_ImageClassification_UI
{
    public class MlContextClass
    {
        private string ProjectDirectory { get; set; } // main directory
        private string WorkspaceRelativePath { get; set; } // workspace dir for deep mind
        private string AssetsRelativePath { get; set; } //Assets of images
        public PredictionEngine<InputModel, OutputModel> PredictionEngine { get; set; } //Prediction engine used for making predictions

        public MlContextClass()
        {
            if (TrainModel())
                Console.WriteLine("Model Trained");
            else
                Console.WriteLine("Error occured. Model failed to build");
        }

        /// <summary>
        /// Main method used for training the prediction model
        /// </summary>
        /// <returns>True if successful otherwise false if some error occures</returns>
        public bool TrainModel()
        {
            try
            {
                ProjectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../")); //set main directory path
                WorkspaceRelativePath = Path.Combine(ProjectDirectory, "workspace"); // set workspace path
                AssetsRelativePath = Path.Combine(ProjectDirectory, "assets"); //set assets path
                ClearWorkspace();

                MLContext context = new MLContext(); // Machine Learning Context
                IEnumerable<ImageData> images = ImageLoader.LoadImagesFromDirectory(folder: AssetsRelativePath, useFolderNameAsLabel: true); // Load images from assets dir

                IDataView imageData = context.Data.LoadFromEnumerable(images);//fundamental pipeline
                IDataView shuffledData = context.Data.ShuffleRows(imageData); // shuffle rows od the pipeline
                
                //Creates a Estimator which converts categorical values into numerical keys from InputModel class
                var preprocessingPipeline = context.Transforms.Conversion.MapValueToKey(
                    inputColumnName: "Label",
                    outputColumnName: "LabelAsKey")
                .Append(context.Transforms.LoadRawImageBytes(
                    outputColumnName: "Image",
                    imageFolder: AssetsRelativePath,
                    inputColumnName: "ImagePath"));

                //Pre processsed data used for training/testing/validating the model
                IDataView preProcessedData = preprocessingPipeline
                        .Fit(shuffledData)
                        .Transform(shuffledData);

                //Declare to split into 3 categories train / test / validate
                TrainTestData trainSplit = context.Data.TrainTestSplit(data: preProcessedData, testFraction: 0.3);
                TrainTestData validationTestSplit = context.Data.TrainTestSplit(trainSplit.TestSet);

                IDataView trainSet = trainSplit.TrainSet; // get the training set
                IDataView validationSet = validationTestSplit.TrainSet;// get the validation set
                IDataView testSet = validationTestSplit.TestSet; // get the test set
                
                //Image trainer options used for training the image classification model
                var classifierOptions = new ImageClassificationTrainer.Options()
                {
                    FeatureColumnName = "Image",
                    LabelColumnName = "LabelAsKey",
                    ValidationSet = validationSet,
                    Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
                    MetricsCallback = (metrics) => Console.WriteLine(metrics),
                    TestOnTrainSet = false,
                    Epoch = 100,
                    BatchSize = 10,
                    ReuseTrainSetBottleneckCachedValues = true,
                    ReuseValidationSetBottleneckCachedValues = true,
                    WorkspacePath = WorkspaceRelativePath
                };

                //create a training pipeline and append output of prediction as PredictedLabel from OutputModel class
                var trainingPipeline = context.MulticlassClassification.Trainers.ImageClassification(classifierOptions)
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                //Main trained model used for making image predictions
                ITransformer trainedModel = trainingPipeline.Fit(trainSet);

                //Prediction engine used for prediction. Created from the trained model
                PredictionEngine = context.Model.CreatePredictionEngine<InputModel, OutputModel>(trainedModel);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes existing prediction model with all of its trained parameters
        /// </summary>
        private void ClearWorkspace()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(WorkspaceRelativePath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        /// Method used for making predictions
        /// </summary>
        /// <param name="input"></param>
        /// <returns>OutputMOdel as a prediction</returns>
        public OutputModel OutputPrediction(InputModel input)
        {
            try
            {
                OutputModel output = PredictionEngine.Predict(input);//Calling the Predict method for getting the prediction from our trained model
                return output;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }
    }
}
