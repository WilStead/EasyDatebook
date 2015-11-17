using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDatebook_Model
{
    /// <summary>
    /// An address book entry object. A collection of these can represent an address book.
    /// </summary>
    public class AddressBookEntry : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        /// <summary>
        /// The name for this addressbook entry.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _phoneHome;
        /// <summary>
        /// The home phone number for this addressbook entry.
        /// </summary>
        /// <remarks>
        /// Stored as a string, with no special format restrictions.
        /// Users may enter anything they like, even if it's not a valid phone number.
        /// This allows for entries with such extras as "ext. 123" or "extension 4" or
        /// even "(only for emergencies)" without forcing the user to conform to a
        /// rigid format.
        /// </remarks>
        public string PhoneHome
        {
            get { return _phoneHome; }
            set { SetProperty(ref _phoneHome, value); }
        }

        private string _phoneCell;
        /// <summary>
        /// The cell phone number for this addressbook entry.
        /// </summary>
        /// <remarks>
        /// Stored as a string, with no special format restrictions.
        /// Users may enter anything they like, even if it's not a valid phone number.
        /// This allows for entries with such extras as "ext. 123" or "extension 4" or
        /// even "(only for emergencies)" without forcing the user to conform to a
        /// rigid format.
        /// </remarks>
        public string PhoneCell
        {
            get { return _phoneCell; }
            set { SetProperty(ref _phoneCell, value); }
        }

        private string _address;
        /// <summary>
        /// The address for this addressbook entry.
        /// </summary>
        /// <remarks>
        /// Stored as a string, with no special format restrictions.
        /// Users may enter anything they like, even if it's not a valid address.
        /// This allows for such entries as "next door" if a user wishes, without
        /// forcing the user to conform to a rigid format.
        /// </remarks>
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _email;
        /// <summary>
        /// The email address for this addressbook entry.
        /// </summary>
        /// <remarks>
        /// Stored as a string, with no special format restrictions.
        /// Users may enter anything they like, even if it's not a valid
        /// email address. This allows for such entries as "abc123" without
        /// the @XXXX.XXX; this can be common among older AOL users who aren't
        /// used to needing the full address within the AOL ecosystem.
        /// </remarks>
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private DateTime _birthday;
        /// <summary>
        /// The birthday for this addressbook entry.
        /// </summary>
        /// <remarks>
        /// Unlike most entries, this is a well-formatted DateTime. This
        /// is not expected to be taxing for users since it is picked from
        /// a GUI calendar control, not parsed from text entry.
        /// </remarks>
        public DateTime Birthday
        {
            get { return _birthday; }
            set { SetProperty(ref _birthday, value); }
        }

        /// <summary>
        /// The default contructor.
        /// </summary>
        public AddressBookEntry()
        {
            Name = "<name>";
            Birthday = new DateTime(1950, 1, 1);
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
