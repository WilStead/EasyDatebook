using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace EasyDatebook_Screens
{
    /// <summary>
    /// Wraps MediaPlayer and allows playing embedded media resources.
    /// </summary>
    public class EmbeddedMediaPlayer : MediaPlayer, IDisposable
    {
        private string tempFolder;

        /// <summary>
        /// The default constructor.
        /// </summary>
        public EmbeddedMediaPlayer() { }

        private Uri CreateTempFileFromResource(Uri uri)
        {
            try
            {
                // Create the temporary folder if it does not already exist.
                if (String.IsNullOrEmpty(tempFolder))
                {
                    tempFolder = Path.Combine(Path.GetTempPath(), "EasyDatebook");
                    if (!Directory.Exists(tempFolder))
                        Directory.CreateDirectory(tempFolder);
                }

                string file = Path.Combine(tempFolder, Path.GetFileName(uri.ToString()));
                // Copy the resource to the temporary folder if it is not already there.
                if (!File.Exists(file))
                {
                    var stream = Application.GetResourceStream(uri).Stream;
                    var fileStream = File.Create(file);
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                }

                return new Uri(file, UriKind.Absolute);
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Plays the resource file specified by the provided URI.
        /// </summary>
        /// <param name="uri">A URI to open and play. It will be copied to a temporary folder before opening.</param>
        public void PlayResource(Uri uri)
        {
            Uri _uri = CreateTempFileFromResource(uri);
            Open(_uri);
            Play();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (String.IsNullOrEmpty(tempFolder)) return;

            try
            {
                Close();
                Directory.Delete(tempFolder, true);
            }
            catch (Exception)
            {
                /* No way to feasibly recover if deleting the temporary folder fails.
                 * It will be left in place, and possibly re-used on future runs of the program. */
            }
        }
    }
}
