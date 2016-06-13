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

namespace BasicUwpPlayer
{
    /// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Lista da arquivos para reprodução.
        /// </summary>
        List<IHearable> RecordingFiles;
        /// <summary>
        /// Índice (em <see cref="RecordingFiles"/>) do próximo arquivo a ser reproduzido.
        /// </summary>
        int recordingId;

        public MainPage()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// Quando o layout é carregado, esse método é chamado.
        /// Ele dá início ao player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.TurnOn();
        }
        /// <summary>
        /// Inicia o player.
        /// </summary>
        private async void TurnOn()
        {
            // Mantém o display ligado:
            var displayRequest = new DisplayRequest();
            displayRequest.RequestActive();

            // Carrega as musicas da pasta "recordings"
            this.RecordingFiles = await this.LoadRecordingFiles();

            // Começa a tocar as musicas
            this.Play();
        }
        /// <summary>
        /// Chamado quando um dos botões de controle do player (next, back, play)
        /// é pressionado.
        /// </summary>
        /// <param name="sender">Botão de controle.</param>
        /// <param name="e"></param>
        private void OnPlayerButtonClick(object sender, RoutedEventArgs e)
        {
            // Faz o controle de play/pause:
            if (sender == this.btnPlay)
            {
                if (player.CurrentState == MediaElementState.Playing)
                {
                    player.Pause();
                }
                else
                {
                    player.Play();
                }
            }

            // Passa para a próxima música:
            else if (sender == this.btnNext)
            {
                if (this.recordingId < this.RecordingFiles.Count - 1)
                {
                    this.recordingId++;
                }
                else { this.recordingId = 0; }

                this.Play();
            }

            // Volta para a musica anterior:
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
        /// <summary>
        /// Toca uma musica.
        /// </summary>
        private void Play()
        {
            // Mantém o display ativo:
            var displayRequest = new DisplayRequest();
            displayRequest.RequestActive();

            // Define o título da música:
            this.txtSongTitle.Text = this.RecordingFiles.ElementAt(this.recordingId).Name;

            // Pega a musica a ser reproduzida:
            this.player.SetSource(this.RecordingFiles.ElementAt(this.recordingId).File, ".mp3");
            
            // Toca a música:
            this.player.Play();
        }
        /// <summary>
        /// Carrega todos os arquivos da pasta "recordings".
        /// </summary>
        /// <returns>Retorna uma lista de streams contendo os audios da pasta "recordings".</returns>
        private async Task<List<IHearable>> LoadRecordingFiles()
        {
            recordingId = 0;

            List<IHearable> streams = new List<IHearable>();

            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder folder = await appInstalledFolder.GetFolderAsync(@"Assets\recordings");

            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                streams.Add(new Hearable(file.Name.Split('.').FirstOrDefault(), await file.OpenAsync(FileAccessMode.Read)));
            }

            return streams;
        }
        /// <summary>
        /// Chamado quando o audio chegou ao fim.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (this.recordingId < this.RecordingFiles.Count - 1)
            {
                this.recordingId++;
            }
            else { this.recordingId = 0; }

            this.Play();
        }

        // Just for debug:
        private void MusicPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(this.player.CurrentState);
        }
    }
}
