using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;

namespace MusicPlayer
{
    public partial class MainWindow : Window
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private DispatcherTimer timer;
        private string musicFolder;
        private ObservableCollection<Playlist> playlists;
        private Playlist currentPlaylist;

        public class Playlist
        {
            public string Name { get; set; }
            public ObservableCollection<string> Tracks { get; set; }

            public Playlist(string name)
            {
                Name = name;
                Tracks = new ObservableCollection<string>();
            }


        }

        public MainWindow()
        {
            InitializeComponent();

            musicFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "MusicPlayer");
            if (!Directory.Exists(musicFolder))
                Directory.CreateDirectory(musicFolder);

            playlists = new ObservableCollection<Playlist>();
            playlistList.ItemsSource = playlists;

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;

            LoadPlaylists();

            volumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            progressSlider.ValueChanged += ProgressSlider_ValueChanged;
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.wma",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    currentPlaylist?.Tracks.Add(file);
                }
                SavePlaylists();
            }
        }

        private void DeleteTrack_Click(object sender, RoutedEventArgs e)
        {
            if (trackList.SelectedItem is string selectedTrack && currentPlaylist != null)
            {
                currentPlaylist.Tracks.Remove(selectedTrack);
                SavePlaylists();
            }
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (playlistList.SelectedItem is Playlist selectedPlaylist)
            {
                playlists.Remove(selectedPlaylist);
                File.Delete(Path.Combine(musicFolder, $"{selectedPlaylist.Name}.json"));
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (trackList.SelectedItem != null)
            {
                if (outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    outputDevice.Stop();
                }

                var selectedFile = trackList.SelectedItem.ToString();
                audioFile = new AudioFileReader(selectedFile);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                timer.Start();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice != null)
            {
                if (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    outputDevice.Pause();
                }
                else if (outputDevice.PlaybackState == PlaybackState.Paused)
                {
                    outputDevice.Play();
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                audioFile.Position = 0;
                timer.Stop();
                progressSlider.Value = 0;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audioFile != null)
            {
                audioFile.Volume = (float)volumeSlider.Value;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (audioFile != null)
            {
                progressSlider.Maximum = audioFile.TotalTime.TotalSeconds;
                progressSlider.Value = audioFile.CurrentTime.TotalSeconds;
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audioFile != null && Math.Abs(audioFile.CurrentTime.TotalSeconds - progressSlider.Value) > 1)
            {
                audioFile.CurrentTime = TimeSpan.FromSeconds(progressSlider.Value);
            }
        }

        private void LoadPlaylists()
        {
            foreach (var file in Directory.GetFiles(musicFolder, "*.json"))
            {
                string json = File.ReadAllText(file);
                var playlist = JsonConvert.DeserializeObject<Playlist>(json);
                playlists.Add(playlist);
            }
        }

        private void NewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = playlistNameTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(playlistName))
            {
                Playlist newPlaylist = new Playlist(playlistName);
                playlists.Add(newPlaylist);
                SavePlaylists();
                playlistNameTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Введите название плейлиста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PlaylistList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistList.SelectedItem is Playlist selectedPlaylist)
            {
                currentPlaylist = selectedPlaylist;
                trackList.ItemsSource = currentPlaylist.Tracks;
            }
        }

        private void SavePlaylists()
        {
            foreach (var playlist in playlists)
            {
                var filePath = Path.Combine(musicFolder, $"{playlist.Name}.json");
                string json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
        }

        private void PlaylistNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (playlistNameTextBox.Text == "Введите название плейлиста")
            {
                playlistNameTextBox.Text = "";
                playlistNameTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void PlaylistNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(playlistNameTextBox.Text))
            {
                playlistNameTextBox.Text = "Введите название плейлиста";
                playlistNameTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void DisplayTrackInfo(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);
                trackInfo.Text = $"Playing: {file.Tag.Title ?? Path.GetFileName(filePath)} - {file.Tag.Performers?[0] ?? "Unknown Artist"}";
            }
            catch
            {
                trackInfo.Text = $"Playing: {Path.GetFileName(filePath)}";
            }
        }
    }
}