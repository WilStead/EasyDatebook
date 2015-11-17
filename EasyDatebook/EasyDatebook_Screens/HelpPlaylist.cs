using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDatebook_Screens
{
    /// <summary>
    /// Contains a collection of URIs to media files which can be played in a sequence.
    /// </summary>
    public class HelpPlaylist : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private EmbeddedMediaPlayer player;

        /// <summary>
        /// An ordered list of URIs for audio help files.
        /// </summary>
        public List<Uri> Playlist { get; set; }

        private int currentTrack = 0;

        /// <summary>
        /// Retrieves the current track position of the Playlist. Read-only.
        /// </summary>
        public int CurrentTrack { get { return currentTrack; } }

        private bool isAutoplay;
        /// <summary>
        /// Get or set whether this HelpPlaylist will automatically start the next
        /// audio file when the current one is finished.
        /// </summary>
        public bool IsAutoplay
        {
            get { return isAutoplay; }
            set { SetProperty(ref isAutoplay, value); }
        }

        private bool isPlaying = false;

        private bool nextTrackQueued = false;

        /// <summary>
        /// Construct a new HelpPlaylist with the given playlist of URIs.
        /// </summary>
        public HelpPlaylist(EmbeddedMediaPlayer player, List<Uri> uris)
        {
            this.player = player;
            Playlist = uris;
            player.MediaEnded += player_MediaEnded;
        }

        /// <summary>
        /// Starts playback of the current URI in the playlist.
        /// </summary>
        public void Start()
        {
            player.Stop(); // Stop anything already playing first.
            player.PlayResource(Playlist[currentTrack]);
            isPlaying = true;
        }

        /// <summary>
        /// Resets the playlist back to the first URI in the list.
        /// </summary>
        public void Reset() { SetProperty(ref currentTrack, 0); }

        /// <summary>
        /// Advances to the next track in the playlist.
        /// If the current track is the last one, the position is reset to the start of the playlist.
        /// </summary>
        public void Advance()
        {
            SetProperty(ref currentTrack, currentTrack + 1);
            if (currentTrack >= Playlist.Count) Reset();
        }

        /// <summary>
        /// Stops playback of the current audio track and resets the current position to the start of the list.
        /// </summary>
        public void Stop()
        {
            player.Stop();
            Reset();
        }

        /// <summary>
        /// Resets the current position to the start of the list, turns autoplay off, and starts the player.
        /// </summary>
        public void PlayFirstTrack()
        {
            Reset();
            IsAutoplay = false;
            Start();
        }

        /// <summary>
        /// Notifies the playlist that the next track in the list should be played
        /// when the currently playing track is finished.
        /// The next track will play immediately if no track is currently playing.
        /// </summary>
        public void QueueNextTrack()
        {
            if (!isPlaying)
            {
                Advance();
                Start();
            }
            else nextTrackQueued = true;
        }

        /// <summary>
        /// Resets the current position to the start of the list, turns autoplay on, and starts the player.
        /// </summary>
        public void PlayEntireList()
        {
            Reset();
            IsAutoplay = true;
            Start();
        }

        private void player_MediaEnded(object sender, EventArgs e)
        {
            if (!isPlaying) return;
            isPlaying = false;

            if (nextTrackQueued || IsAutoplay)
            {
                nextTrackQueued = false;
                Advance();
                // If advancing has caused the playlist to loop, do not restart it.
                if (currentTrack != 0) Start();
            }
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
