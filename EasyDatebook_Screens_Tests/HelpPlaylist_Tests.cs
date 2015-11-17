using EasyDatebook_Screens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyDatebook_Screens_Tests
{
    [TestClass]
    public class HelpPlaylist_Tests
    {
        /// <summary>
        /// Verifies that the track position can be manipulated correctly.
        /// </summary>
        [TestMethod]
        public void TrackControl_Test()
        {
            EmbeddedMediaPlayer mp = new EmbeddedMediaPlayer();
            HelpPlaylist hp = new HelpPlaylist(mp, new List<Uri>()
            {
                new Uri("EasyDatebook_Screens_Tests;component/Test1.mp3", UriKind.Relative),
                new Uri("EasyDatebook_Screens_Tests;component/Test2.mp3", UriKind.Relative),
                new Uri("EasyDatebook_Screens_Tests;component/Test3.mp3", UriKind.Relative)
            });
            Assert.IsTrue(hp.CurrentTrack == 0);
            hp.Advance();
            Assert.IsTrue(hp.CurrentTrack == 1);
            hp.Reset();
            Assert.IsTrue(hp.CurrentTrack == 0);
            for (int i = 0; i < hp.Playlist.Count; i++)
            {
                hp.Advance();
            }
            Assert.IsTrue(hp.CurrentTrack == 0);
        }

        /// <summary>
        /// Verifies that tracks may be played, either singly or in complete lists, and also that they may be stopped.
        /// </summary>
        [TestMethod]
        public void Play_Test()
        {
            EmbeddedMediaPlayer mp = new EmbeddedMediaPlayer();
            HelpPlaylist hp = new HelpPlaylist(mp, new List<Uri>()
            {
                new Uri("EasyDatebook_Screens_Tests;component/Test1.mp3", UriKind.Relative),
                new Uri("EasyDatebook_Screens_Tests;component/Test2.mp3", UriKind.Relative),
                new Uri("EasyDatebook_Screens_Tests;component/Test3.mp3", UriKind.Relative)
            });
            hp.PlayFirstTrack();
            hp.Stop();
            hp.PlayEntireList();
            Assert.IsTrue(hp.CurrentTrack == 0);
        }
    }
}
