using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDatebook_Model
{
    /// <summary>
    /// A simple class encapsulating a text field for binding.
    /// </summary>
    public class Note : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private string _noteText;
        /// <summary>
        /// The text of this note.
        /// </summary>
        public string NoteText
        {
            get { return _noteText; }
            set { SetProperty(ref _noteText, value); }
        }

        /// <summary>
        /// The default contructor.
        /// </summary>
        public Note() { NoteText = String.Empty; }

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
