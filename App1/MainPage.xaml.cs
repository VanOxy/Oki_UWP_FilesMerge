using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilesMerge
{
    public sealed partial class MainPage : Page
    {
        private string destinationFolderPath;
        private List<string> folder1FileList = new List<string>();
        private List<string> folder2FileList = new List<string>();
        private List<string> orderedPathsList = new List<string>();
        private FileOpenPicker filePicker = new FileOpenPicker();

        public MainPage()
        {
            InitializeComponent();
            SetWindowSize();
            InitFilePicker();
        }

        private void SetWindowSize()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 300));
            ApplicationView.PreferredLaunchViewSize = new Size(500, 300);
            ApplicationView.PreferredLaunchWindowingMode =
                ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void InitFilePicker()
        {
            filePicker.ViewMode = PickerViewMode.List;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
        }

        /// <summary>
        /// Get all files paths from chosen folder into "List<string> folder1FileList"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_folder1_Click(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<StorageFile> fileList = await filePicker.PickMultipleFilesAsync();

            if (fileList != null)
            {
                // clear files list
                if (folder1FileList.Count > 0)
                    folder1FileList.Clear();

                // fill collection
                foreach (var item in fileList)
                    folder1FileList.Add(item.Path);

                // affect label
                string fileParent = Directory.GetParent(fileList[0].Path).FullName.ToString();
                label_folder1.Text = fileParent + "   // Files : " + fileList.Count;
            }
            else
                await new MessageDialog("Something gone wrong, try again...").ShowAsync();
        }

        /// <summary>
        /// Get all files paths from chosen folder into "List<string> folder2FileList"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_folder2_Click(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<StorageFile> fileList = await filePicker.PickMultipleFilesAsync();

            if (fileList != null)
            {
                // clear files list
                if (folder2FileList.Count > 0)
                    folder2FileList.Clear();

                // fill collection
                foreach (var item in fileList)
                    folder2FileList.Add(item.Path);

                // affect label
                string fileParent = Directory.GetParent(fileList[0].Path).FullName.ToString();
                label_folder2.Text = fileParent + "   // Files : " + fileList.Count;
            }
            else
                await new MessageDialog("Something gone wrong, try again...").ShowAsync();
        }

        /// <summary>
        /// Chose your destination folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_destination_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                label_destination.Text = folder.Path;
                destinationFolderPath = folder.Path;
            }
        }

        private async void btn_run_Click(object sender, RoutedEventArgs e)
        {
            // longueur de la liste
            int length = folder1FileList.Count;
            int counter = 0;

            // remplir la liste commune
            while (length > 0)
            {
                try
                {
                    orderedPathsList.Add(folder1FileList[counter]);
                    orderedPathsList.Add(folder2FileList[counter]);
                    counter++;
                    length--;
                }
                catch (Exception)
                {
                    if (folder1FileList.Count > folder2FileList.Count)
                    {
                        await new MessageDialog("First array is > than second. Make attention that it shouldn't be greather than 1!!!").ShowAsync();
                        break;
                    }
                    else {
                        await new MessageDialog("Something went wrong...!!!").ShowAsync();
                        break;
                    }
                }
            }

            // reinitialiser le counter et copier les nouveaux fichiers dans le dossier
            // destination en leur affectant le nom qui correspond à la val du counter
            StringBuilder destinationFileName = new StringBuilder();
            StorageFolder destinationFolder = await StorageFolder.GetFolderFromPathAsync(destinationFolderPath + "\\");
            StorageFile originalFile;
            counter = 1;

            foreach (string path in orderedPathsList)
            {
                // create new file name
                destinationFileName.Clear();
                destinationFileName.Append(counter + Path.GetExtension(path));

                // copy file
                originalFile = await StorageFile.GetFileFromPathAsync(path);
                await originalFile.CopyAsync(destinationFolder, destinationFileName.ToString(), NameCollisionOption.ReplaceExisting);

                counter++;
            }

            await new MessageDialog("Done.. !!! ^^").ShowAsync();
        }
    }
}