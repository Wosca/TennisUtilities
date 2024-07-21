using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using static TennisUtilities.CourtView;

namespace TennisUtilities
{
    public sealed partial class VideoView : Page
    {
        public VideoView()
        {
            this.InitializeComponent();
        }

        private MediaPlayer mediaPlayer;
        private TimeSpan contactTime;
        private TimeSpan landedTime;
        private StorageFile fileTransfer;
        private double frameRate;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ValueTuple<StorageFile, double> parameters)
            {
                var file = parameters.Item1;

                FileNameTextBlock.Text = "File Name: " + file.Name;
                FilePathTextBlock.Text = "File Path: " + file.Path;
                fileTransfer = file;
                frameRate = parameters.Item2;
                mediaPlayer = new MediaPlayer();
                var mediaSource = MediaSource.CreateFromStorageFile(file);
                mediaPlayer.Source = mediaSource;
                mediaPlayerElement.SetMediaPlayer(mediaPlayer);
            }
            base.OnNavigatedTo(e);
        }

        private void PlayPauseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                mediaPlayer.Pause();
            }
            else
            {
                mediaPlayer.Play();
            }
        }

        private void FrameBackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (mediaPlayer.PlaybackSession.CanSeek)
            {
                mediaPlayer.PlaybackSession.Position -= TimeSpan.FromMilliseconds(1000 / frameRate); // Assuming 30 fps
            }
        }

        private void FrameForwardButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (mediaPlayer.PlaybackSession.CanSeek)
            {
                mediaPlayer.PlaybackSession.Position += TimeSpan.FromMilliseconds(1000 / frameRate); // Assuming 30 fps
            }
        }

        private void SetContactButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            contactTime = mediaPlayer.PlaybackSession.Position;
            ContactTimeTextBlock.Text = contactTime.ToString(@"hh\:mm\:ss\.fff");
        }

        private void SetLandedButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            landedTime = mediaPlayer.PlaybackSession.Position;
            LandedTimeTextBlock.Text = landedTime.ToString(@"hh\:mm\:ss\.fff");
        }

        private void SubmitButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (contactTime.TotalMilliseconds != 0 && landedTime.TotalMilliseconds != 0)
            {
                mediaPlayer.Pause();
                var data = new Data
                {
                    ContactTime = contactTime,
                    LandedTime = landedTime,
                    FileTransfer = fileTransfer
                };
                Frame.Navigate(typeof(CourtView), data);
            }
        }
    }
}
