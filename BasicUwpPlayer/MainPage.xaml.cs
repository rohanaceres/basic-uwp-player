using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StonePlayer
{
    /// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
    {
        List<IRandomAccessStream> RecordingFiles;
        int recordingId;

        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.TurnOn();
        }
        private async void TurnOn()
        {
            var displayRequest = new DisplayRequest();
            displayRequest.RequestActive();
            this.RecordingFiles = await this.LoadRecordingFiles();
            this.Play();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.btnPlay)
            {
                if (player.CurrentState == MediaElementState.Playing)
                {
                    //this.ImageSource = new BitmapImage(new Uri("Assets/pause.png", UriKind.Relative));
                    player.Pause();
                }
                else
                {
                    player.Play();
                }
            }
            else if (sender == this.btnNext)
            {
                if (this.recordingId < this.RecordingFiles.Count - 1)
                {
                    this.recordingId++;
                }
                else { this.recordingId = 0; }

                this.Play();
            }
            else if (sender == this.btnPrev)
            {
                if (this.recordingId > 0)
                {
                    this.recordingId--;
                }
                else { this.recordingId = this.RecordingFiles.Count - 1; }

                this.Play();
            }
        }
        private void Play()
        {
            var displayRequest = new DisplayRequest();
            displayRequest.RequestActive();
            this.txtSongTitle.Text = "Gravação " + (this.recordingId + 1);
            this.player.SetSource(this.RecordingFiles.ElementAt(this.recordingId), ".wav");
            this.player.Play();
        }
        private async Task<List<IRandomAccessStream>> LoadRecordingFiles()
        {
            recordingId = 0;

            List<IRandomAccessStream> streams = new List<IRandomAccessStream>();

            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder folder = await appInstalledFolder.GetFolderAsync(@"Assets\recordings");

            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                streams.Add(await file.OpenAsync(FileAccessMode.Read));
            }

            return streams;
        }
        private void MusicPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(this.player.CurrentState);
        }
        private string GetNextRecordingName()
        {
            string recordingName;

            Random recordingId = new Random();
            recordingName = "Assets/recordings/" + recordingId.Next(1, 7) + ".wav";

            return recordingName;
        }
        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (this.recordingId < this.RecordingFiles.Count - 1)
            {
                this.recordingId++;
            }
            else { this.recordingId = 0; }

            this.Play();
        }
    }
}
