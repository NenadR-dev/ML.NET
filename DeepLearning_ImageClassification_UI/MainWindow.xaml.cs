using DeepLearning_ImageClassification_UI.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace DeepLearning_ImageClassification_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputModel InputImage { get; set; } //Input Model used for making predictions
        private MlContextClass Context { get; set; } //MLcontext class where the logic for prediction is located

        public MainWindow()
        {
            //Loads background image on MainWindow
            #region load background img
            var imgDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "../../"));
            imgDirectory = System.IO.Path.Combine(imgDirectory, "UI_Assets");
            var files = System.IO.Directory.GetFiles(imgDirectory, "*",
                searchOption: System.IO.SearchOption.AllDirectories);
            var label = string.Empty;
            foreach (var img in files)
            {
                label = System.IO.Path.GetFullPath(img);
            }
            ImageBrush image = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(label))
            };
            this.Background = image;
            #endregion load background img

            InitializeComponent();
            Context = new MlContextClass(); //Initialize MLContext
            InputImage = new InputModel(); //Initialize InputModel
        }

        /// <summary>
        /// Button click method used for selecing an image you wish the program to predict
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select_Image_Button_Click(object sender, RoutedEventArgs e)
        {
            //Opens new window for selecting an image.
            //Filter is used for making the dialog only to select images
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*" // only images
            };

            dialog.ShowDialog();
            if(dialog.FileName != null) // check if selected img isnt null
            {
                InputImage.ImagePath = dialog.FileName;
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(InputImage.ImagePath); //Create Uri to the image
                img.EndInit();
                Selected_Image.Source = img;

                //Convert image to bytes
                using (MemoryStream mStream = new MemoryStream())
                {
                    System.Drawing.Image imgTest = System.Drawing.Image.FromFile(InputImage.ImagePath);//load image from file
                    imgTest.Save(mStream, imgTest.RawFormat); //Save image to a MemoryStream later used for byte extraction
                    var bytes = mStream.ToArray(); // extract bytes of the image
                    InputImage.Image = bytes;
                }
            }
        }
        /// <summary>
        /// Predict button method used for making predictions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Predict_Button_Click(object sender, RoutedEventArgs e)
        {
            Prediction_Label.Content = string.Empty;
            Prediction_Label.Content ="Prediction: " + Context.OutputPrediction(InputImage).PredictedLabel;
        }

        /// <summary>
        /// Event that occures when text is changed in LabelBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InputImage.Label = ((TextBox)sender).Text;
        }

        private void Train_Button_Click(object sender, RoutedEventArgs e)
        {
            TrainWindow train = new TrainWindow(this.Context);
            //Loads background image on MainWindow
            #region load background img
            var imgDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "../../"));
            imgDirectory = System.IO.Path.Combine(imgDirectory, "UI_Assets");
            var files = System.IO.Directory.GetFiles(imgDirectory, "*",
                searchOption: System.IO.SearchOption.AllDirectories);
            var label = string.Empty;
            foreach (var img in files)
            {
                label = System.IO.Path.GetFullPath(img);
            }
            ImageBrush image = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(label))
            };
            train.Background = image;
            #endregion load background img
            train.ShowDialog();
        }
    }
}
