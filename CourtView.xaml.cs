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
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace TennisUtilities
{
    public sealed partial class CourtView : Page
    {
        public CourtView()
        {
            this.InitializeComponent();
        }

        double newValue = 200;
        double pointX = 0;
        double pointY = 0;
        TimeSpan contactTime;
        TimeSpan landedTime;
        private MediaPlayer mediaPlayer;
        private MediaPlayer mediaPlayer2;

        public class Data
        {
            public TimeSpan ContactTime { get; set; }
            public TimeSpan LandedTime { get; set; }
            public StorageFile FileTransfer { get; set; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Data data)
            {
                contactTime = data.ContactTime;
                landedTime = data.LandedTime;
                mediaPlayer = new MediaPlayer();
                mediaPlayer2 = new MediaPlayer();
                var mediaSource = MediaSource.CreateFromStorageFile(data.FileTransfer);
                mediaPlayer.Source = mediaSource;
                mediaPlayerElement.SetMediaPlayer(mediaPlayer);
                mediaPlayer.PlaybackSession.Position = contactTime;

                var mediaSource2 = MediaSource.CreateFromStorageFile(data.FileTransfer);
                mediaPlayer2.Source = mediaSource2;
                mediaPlayerElement2.SetMediaPlayer(mediaPlayer2);
                mediaPlayer2.PlaybackSession.Position = landedTime;

                // Update TextBlocks with initial times
                ContactTimeTextBlock.Text = $"Contact Time: {contactTime}";
                LandedTimeTextBlock.Text = $"Landed Time: {landedTime}";
            }
            base.OnNavigatedTo(e);
        }

        private void TennisCourtCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(TennisCourtCanvas);
            if (point.X > 614 || point.Y > 348 || point.Y < 50 || point.X < 400)
                return;
            pointX = point.X;
            pointY = point.Y;

            BallLandedPoint.SetValue(Canvas.LeftProperty, point.X - BallLandedPoint.Width / 2);
            BallLandedPoint.SetValue(Canvas.TopProperty, point.Y - BallLandedPoint.Height / 2);
            BallLandedPointOutline.SetValue(
                Canvas.LeftProperty,
                point.X - BallLandedPoint.Width / 2
            );
            BallLandedPointOutline.SetValue(
                Canvas.TopProperty,
                point.Y - BallLandedPoint.Height / 2
            );
        }

        private void ServePositionSlider_ValueChanged(
            object sender,
            RangeBaseValueChangedEventArgs e
        )
        {
            newValue = e.NewValue;
            ServePositionArrow.Y1 = 400 - newValue;
            ServePositionArrow.Y2 = 400 - newValue;
        }

        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            double servePosition = 400 - newValue;
            double ballLandedX = (double)BallLandedPoint.GetValue(Canvas.LeftProperty);
            double ballLandedY = (double)BallLandedPoint.GetValue(Canvas.TopProperty);

            double opposite = Math.Abs(Math.Abs(servePosition - pointY) / 36.46308113035552);
            double adjacent = pointX / 33.65586874211191;

            double distance = Math.Sqrt(Math.Pow(opposite, 2) + Math.Pow(adjacent, 2));
            double totalTime = (landedTime.TotalMilliseconds - contactTime.TotalMilliseconds);
            double speedMs = distance / (totalTime / 1000);
            double speedKmh = speedMs * 3.6;

            DistanceTextBlock.Text = $"Distance: {distance:F2} meters";
            SpeedTextBlock.Text = $"Speed: {speedMs:F2} m/s, {speedKmh:F2} km/h";
            ContactTimeTextBlock.Text = $"Contact Time: {contactTime}";
            LandedTimeTextBlock.Text = $"Landed Time: {landedTime}";
        }
    }
}
