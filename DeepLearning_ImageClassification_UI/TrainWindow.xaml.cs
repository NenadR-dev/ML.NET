using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;
using DeepLearning_ImageClassification_UI.Model;
using Microsoft.Win32;

namespace DeepLearning_ImageClassification_UI
{
    /// <summary>
    /// Interaction logic for TrainWindow.xaml
    /// </summary>
    public partial class TrainWindow : Window
    {
        MlContextClass Context { get; set; }
        List<ImageData> Images { get; set; }
        string Label { get; set; }
        public TrainWindow(MlContextClass context)
        {

            InitializeComponent();
            Context = context;
            Images = new List<ImageData>();
        }

        private void Train_Button_Click(object sender, RoutedEventArgs e)
        {

            string projectDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "../../../"));
            string assetsRelativePath = System.IO.Path.Combine(projectDirectory, "assets");
            assetsRelativePath += "\\" + Label;
            if(!Directory.Exists(assetsRelativePath))
            {
                Directory.CreateDirectory(assetsRelativePath);
            }
            foreach(var node in Images)
            {
                if(!File.Exists(assetsRelativePath + "\\" + node.Label))
                {
                    File.Copy(node.ImagePath, assetsRelativePath + "\\" + node.Label);
                }
            }
            bool success = Context.TrainModel();
            if (success)
            {
                MessageBox.Show("Model trained");
            }
            else
            {
                MessageBox.Show("Error occured: Model training failed");
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddImage_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog
            {
                Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*", // only images
                Multiselect = true // Enable multiselect
            };
            diag.ShowDialog();
            foreach(var node in diag.FileNames)
            {
                Images.Add(new ImageData()
                {
                    ImagePath = node,
                    Label = System.IO.Path.GetFileName(node)
                });
                ImagesLixtBox.Items.Add(System.IO.Path.GetFileName(node));
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if(sender.ToString() != string.Empty)
            {
                Images.Remove(Images.Find(x => x.Label == ImagesLixtBox.SelectedItem.ToString()));
                ImagesLixtBox.Items.Remove(ImagesLixtBox.SelectedItem.ToString());
            }
        }

        private void LabelTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Label = ((TextBox)sender).Text;
            if(Label != string.Empty)
            {
                Train_Button.IsEnabled = true;
            }
            else
            {
                Train_Button.IsEnabled = false;
            }
        }
    }
}
