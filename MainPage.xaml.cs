using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Editing;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TennisUtilities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker =
                new() { ViewMode = PickerViewMode.Thumbnail, FileTypeFilter = { ".mp4" }, };

            if (App.m_window != null)
            {
                try
                {
                    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
                    WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

                    StorageFile file = await fileOpenPicker.PickSingleFileAsync();
                    if (file != null)
                    {
                        var mediaClip = await MediaClip.CreateFromFileAsync(file);
                        var videoEncodingProperties = mediaClip.GetVideoEncodingProperties();
                        double frameRate =
                            (double)videoEncodingProperties.FrameRate.Numerator
                            / videoEncodingProperties.FrameRate.Denominator;
                        System.Diagnostics.Debug.WriteLine("Frame rate: " + frameRate);
                        PickAFileOutputTextBlock.Text =
                            "Picked file: " + file.Name + "\nPath: " + file.Path;

                        // Pass the file and frame rate as a tuple
                        Frame.Navigate(typeof(VideoView), (file, frameRate));
                    }
                    else
                    {
                        PickAFileOutputTextBlock.Text = "Operation cancelled.";
                    }
                }
                catch (InvalidCastException ex)
                {
                    System.Diagnostics.Debug.WriteLine("InvalidCastException: " + ex.Message);
                    PickAFileOutputTextBlock.Text = "An error occurred: " + ex.Message;
                }
            }
            else
            {
                PickAFileOutputTextBlock.Text = "Window handle is not initialized.";
            }
        }
    }
}
