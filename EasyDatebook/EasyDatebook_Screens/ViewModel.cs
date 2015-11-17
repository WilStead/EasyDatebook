using EasyDatebook_Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace EasyDatebook_Screens
{
    /// <summary>
    /// The main ViewModel class for EasyDatebook.
    /// </summary>
	public class ViewModel : INotifyPropertyChanged
	{
        /// <summary>
        /// The data object represented by this ViewModel.
        /// </summary>
        private Datebook datebook;

        /// <summary>
        /// A collection of notes indexed by date.
        /// </summary>
        public Dictionary<DateTime, Note> CalendarNotes
        {
            get { return datebook.CalendarNotes; }
            private set { datebook.CalendarNotes = value; }
        }

        private DateTime _currentDate;
        /// <summary>
        /// Indicates the currently seected date.
        /// </summary>
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                SetProperty(ref _currentDate, value);
                OnPropertyChanged("CurrentDateNote");
                OnPropertyChanged("CurrentBirthday");
            }
        }

        /// <summary>
        /// Indicates the Note associated with <see cref="CurrentDate"/>.
        /// </summary>
        public Note CurrentDateNote
        {
            get
            {
                if (CurrentDate == null) return null;
                if (!CalendarNotes.ContainsKey(CurrentDate))
                    CalendarNotes.Add(CurrentDate, new Note());
                return CalendarNotes[CurrentDate];
            }
        }

        /// <summary>
        /// A collection of AddressBookEntries representing an address book.
        /// </summary>
        public ListCollectionView AddressBookEntries { get; private set; }

        private string _addressBookTab;
        /// <summary>
        /// Indicates the currently selected alphabetic tab in the address book interface.
        /// </summary>
        public string AddressBookTab
        {
            get { return _addressBookTab; }
            set
            {
                SetProperty(ref _addressBookTab, value);
                AddressBookEntry entry =
                    AddressBookEntries.Cast<AddressBookEntry>().FirstOrDefault(a => String.Compare(a.Name, value, true) >= 0);
                if (entry == null) AddressBookEntries.MoveCurrentToLast();
                else AddressBookEntries.MoveCurrentTo(entry);
            }
        }

        /// <summary>
        /// A dictionary correlating dates with the names of those in the address book whose birthday is on that date.
        /// </summary>
        public Dictionary<DateTime, IEnumerable<string>> Birthdays
        {
            get
            {
                return datebook.AddressBookEntries.Select(e => e.Birthday).Distinct().ToDictionary(b => b,
                    b => datebook.AddressBookEntries.Where(e => e.Birthday == b).Select(e => e.Name));
            }
        }

        /// <summary>
        /// A list of the names of those in the address book whose birthday is the currently selected date.
        /// </summary>
        public List<string> CurrentBirthday
        {
            get
            {
                List<string> names = new List<string>();
                foreach (DateTime birthday in Birthdays.Keys)
                {
                    if (birthday.Month == CurrentDate.Month && birthday.Day == CurrentDate.Day)
                        names = names.Concat(Birthdays[birthday]).Distinct().ToList();
                }
                return names;
            }
        }

        /// <summary>
        /// The list of all dates which have notes or a birthday associated with them.
        /// </summary>
        public List<DateTime> HighlightedDates
        {
            get
            {
                List<DateTime> birthdays = new List<DateTime>();
                foreach (DateTime birthday in Birthdays.Keys)
                {
                    if (birthday.Month == CurrentDate.Month)
                        birthdays.Add(new DateTime(CurrentDate.Year, birthday.Month, birthday.Day));
                }
                return CalendarNotes.Select(c => c.Key).Concat(birthdays).Distinct().ToList();
            }
        }

        /// <summary>
        /// A collection of BudgetItems representing a monthly budget's incomes.
        /// </summary>
        public ListCollectionView BudgetIncomes { get; private set; }

        /// <summary>
        /// A collection of BudgetItems representing a monthly budget's expenses.
        /// </summary>
        public ListCollectionView BudgetExpenses { get; private set; }

        /// <summary>
        /// A collection of notes.
        /// </summary>
        public ListCollectionView Notes { get; private set; }
		
        /// <summary>
        /// The sum of all expenses and incomes.
        /// </summary>
		public double TotalBudget
        {
            get
            {
                double total =
                    BudgetIncomes.SourceCollection.Cast<BudgetItem>().Sum(b => Math.Abs(b.Amount)) -
                    BudgetExpenses.SourceCollection.Cast<BudgetItem>().Sum(b => Math.Abs(b.Amount));

                return Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }
		}

        #region View manipulation properties

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private sealed class FirstInitial : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.ToString().Substring(0, 1).ToUpper();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        private FirstInitial firstInitial = new FirstInitial();

        #endregion View manipulation properties

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ViewModel()
		{
            LoadData();

            AddressBookEntries = new ListCollectionView(datebook.AddressBookEntries);
            AddressBookEntries.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            AddressBookEntries.GroupDescriptions.Add(new PropertyGroupDescription("Name", firstInitial));
            AddressBookEntries.IsLiveSorting = true;
            AddressBookEntries.IsLiveGrouping = true;
            foreach (AddressBookEntry addrItem in AddressBookEntries)
                addrItem.PropertyChanged += DatePropertyChanged;
            AddAddrEntryCommand = new RelayCommand(AddAddrEntry);
            DeleteAddrEntryCommand = new RelayCommand(DeleteAddrEntry);

            BudgetIncomes = new ListCollectionView(datebook.BudgetIncomes);
            foreach (BudgetItem budgetItem in BudgetIncomes)
                budgetItem.PropertyChanged += BudgetPropertyChanged;
            AddIncomeCommand = new RelayCommand(AddIncome);

            BudgetExpenses = new ListCollectionView(datebook.BudgetExpenses);
            foreach (BudgetItem budgetItem in BudgetExpenses)
                budgetItem.PropertyChanged += BudgetPropertyChanged;
            AddExpenseCommand = new RelayCommand(AddExpense);

            DeleteBudgetCommand = new RelayCommand(DeleteBudgetItem);

            Notes = new ListCollectionView(datebook.Notes);
            foreach (Note note in Notes)
                note.PropertyChanged += CollectionPropertyChanged;
            AddNoteCommand = new RelayCommand(AddNote);
            DeleteNoteCommand = new RelayCommand(DeleteNote);
		}

        /// <summary>
        /// Displays any errors during save file loading to the user in a dialog box.
        /// </summary>
        /// <param name="errorMessage">The exception message to show.</param>
        private void LoadFailure(string errorMessage)
        {
            MessageBox.Show("Your stored data could not be loaded due to the following error:" +
                Environment.NewLine + Environment.NewLine +
                errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Loads saved data, or creates a new blank data object.
        /// </summary>
        private void LoadData()
        {
            try { datebook = Datebook.FromFile(); }
            catch (Exception ex)
            {
                // Report data load failures to the user.
                LoadFailure(ex.Message);

                // In case of failure, create a new data object so program operation can continue.
                datebook = new Datebook();
            }
        }

        private bool IsRichTextEmpty(string richTextDocument)
        {
            RichTextBox tester = new RichTextBox();
            try
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(richTextDocument));
                var document = (FlowDocument)XamlReader.Load(stream);
                tester.Document = document;
            }
            catch (Exception) { tester.Document = new FlowDocument(); }

            if (tester.Document.Blocks.Count == 0) return true;

            TextPointer start = tester.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer end = tester.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
            return start.CompareTo(end) == 0;
        }

        /// <summary>
        /// For use when the View needs to notify the data Model of an update
        /// to the current date's notes (since the data-binding mechanism does
        /// not allow for automatic notifications in the reverse direction due
        /// to its dictionary structure).
        /// </summary>
        public void UpdateCalendarNotes()
        {
            // Remove blank notes, to avoid highlighting dates which have had notes erased,
            // and dates selected but not edited.
            List<DateTime> dates = new List<DateTime>(CalendarNotes.Keys);
            foreach (DateTime date in dates)
            {
                if (date != CurrentDate &&
                    IsRichTextEmpty(CalendarNotes[date].NoteText))
                    CalendarNotes.Remove(date);
            }

            OnPropertyChanged("HighlightedDates");
            RequestCalendarRefresh();
            datebook.SaveData();
        }

        #region Commands

        private ICommand _closeCommand;
        /// <summary>
        /// Command for exiting the application.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(CloseApplication, param => true);
                }
                return _closeCommand;
            }
            set { _closeCommand = value; }
        }

        #region Budget

        /// <summary>
        /// Command to add a new income item to the <see cref="BudgetIncomes"/> collection.
        /// </summary>
        public ICommand AddIncomeCommand { get; set; }

        /// <summary>
        /// Adds a new income item to the <see cref="BudgetIncomes"/> collection.
        /// </summary>
        public void AddIncome(object obj)
        {
            BudgetItem newBudgetItem = new BudgetItem();
            newBudgetItem.PropertyChanged += BudgetPropertyChanged;

            BudgetIncomes.AddNewItem(newBudgetItem);
            BudgetIncomes.CommitNew();

            datebook.SaveData();
        }

        /// <summary>
        /// Command to add a new expense item to the <see cref="BudgetExpenses"/> collection.
        /// </summary>
        public ICommand AddExpenseCommand { get; set; }

        /// <summary>
        /// Adds a new expense item to the <see cref="BudgetExpenses"/> collection.
        /// </summary>
        public void AddExpense(object obj)
        {
            BudgetItem newBudgetItem = new BudgetItem();
            newBudgetItem.PropertyChanged += BudgetPropertyChanged;

            BudgetExpenses.AddNewItem(newBudgetItem);
            BudgetExpenses.CommitNew();

            datebook.SaveData();
        }

        /// <summary>
        /// Command to remove a budget item from the <see cref="BudgetIncomes"/> or <see cref="BudgetExpenses"/> collections.
        /// </summary>
        public ICommand DeleteBudgetCommand { get; set; }

        /// <summary>
        /// Removes a budget item to the <see cref="BudgetIncomes"/> or <see cref="BudgetExpenses"/> collections.
        /// </summary>
        public void DeleteBudgetItem(object obj)
        {
            BudgetItem budgetItem = obj as BudgetItem;
            budgetItem.PropertyChanged -= BudgetPropertyChanged;

            // Take advantage of silent failure feature of Remove to avoid
            // the need to determine which list this item belongs to.
            BudgetIncomes.Remove(budgetItem);
            BudgetExpenses.Remove(budgetItem);

            OnPropertyChanged("TotalBudget");

            datebook.SaveData();
        }

        #endregion // Budget

        #region Notes

        /// <summary>
        /// Command to add a text note to the <see cref="Notes"/> collection.
        /// </summary>
        public ICommand AddNoteCommand { get; set; }

        private void AddNewNote()
        {
            Note newNote = new Note();
            newNote.PropertyChanged += CollectionPropertyChanged;

            Notes.AddNewItem(newNote);
            Notes.CommitNew();

            Notes.MoveCurrentToLast();
        }

        /// <summary>
        /// Adds a text note to the <see cref="Notes"/> collection.
        /// </summary>
        public void AddNote(object obj)
        {
            AddNewNote();
            datebook.SaveData();
        }

        /// <summary>
        /// Command to remove the current text note from the <see cref="Notes"/> collection.
        /// </summary>
        public ICommand DeleteNoteCommand { get; set; }

        /// <summary>
        /// Removes a text note from the <see cref="Notes"/> collection.
        /// </summary>
        public void DeleteNote(object obj)
        {
            Note currentNote = Notes.CurrentItem as Note;
            currentNote.PropertyChanged -= CollectionPropertyChanged;

            Notes.Remove(currentNote);

            if (Notes.Count == 0) // Always at least 1.
                AddNewNote();

            datebook.SaveData();
        }

        #endregion // Notes

        #region Address Book

        /// <summary>
        /// Command to add an address book entry to the <see cref="AddressBookEntries"/> collection.
        /// </summary>
        public ICommand AddAddrEntryCommand { get; set; }

        private void AddNewAddrEntry()
        {
            AddressBookEntry newEntry = new AddressBookEntry();
            newEntry.PropertyChanged += DatePropertyChanged;

            AddressBookEntries.AddNewItem(newEntry);
            AddressBookEntries.CommitNew();

            AddressBookEntries.MoveCurrentTo(newEntry);
        }

        /// <summary>
        /// Adds a new address book entry to the <see cref="AddressBookEntries"/> collection.
        /// </summary>
        public void AddAddrEntry(object obj)
        {
            AddNewAddrEntry();

            OnPropertyChanged("HighlightedDates");
            RequestCalendarRefresh();

            datebook.SaveData();
        }

        /// <summary>
        /// Command to remove an address book entry from the <see cref="AddressBookEntries"/> collection.
        /// </summary>
        public ICommand DeleteAddrEntryCommand { get; set; }

        /// <summary>
        /// Removes an address book entry from the <see cref="AddressBookEntries"/> collection.
        /// </summary>
        public void DeleteAddrEntry(object obj)
        {
            AddressBookEntry item = obj as AddressBookEntry;
            item.PropertyChanged -= DatePropertyChanged;

            AddressBookEntries.Remove(item);

            if (AddressBookEntries.Count == 0) // Always at least 1.
                AddNewAddrEntry();

            OnPropertyChanged("HighlightedDates");
            RequestCalendarRefresh();

            datebook.SaveData();
        }

        #endregion // Address Book

        #endregion // Commands

        #region Event handling

        private void BudgetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("TotalBudget");
            datebook.SaveData();
        }

        /// <summary>
        /// Indicates that any calendars which display data from this ViewModel should be refreshed.
        /// </summary>
        public event EventHandler CalendarRefreshRequested;
        private void RequestCalendarRefresh()
        {
            if (CalendarRefreshRequested != null)
                CalendarRefreshRequested(this, new EventArgs());
        }

        private void DatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("HighlightedDates");
            OnPropertyChanged("CurrentBirthday");
            RequestCalendarRefresh();
            datebook.SaveData();
        }

        private void CollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            datebook.SaveData();
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

        /// <summary>
        /// Fires the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to notify changes.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Checks for unsaved data, then exits the application.
        /// </summary>
        /// <param name="parameter">An arbitrary parameter. Unused.</param>
        public void CloseApplication(object parameter)
        {
            if (!datebook.HasUnsavedData ||
                MessageBox.Show("Some data could not be saved. If you close now, your recent changes might be lost.\nDo you still wish to exit?",
                "Unsaved data", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        #endregion // Event handling
    }

    /// <summary>
    /// Converts a list of names into a readable string mentioning their birthdays.
    /// </summary>
    public sealed class BirthdayConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            IList<string> names = ((IEnumerable<string>)value).ToList();
            if (names.Count == 0) return String.Empty;

            StringBuilder sb = new StringBuilder("It's");

            for (int i = 0; i < names.Count; i++)
            {
                if (i != 0 && names.Count > 2) sb.Append(",");
                if (i == names.Count - 1 && names.Count > 1) sb.Append(" and");
                sb.Append(" ");
                sb.Append(names[i]);
            }

            sb.Append("'s birthday today!");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Creates a numbered title for a tab based on its index within the collection.
    /// </summary>
    public sealed class NoteTitleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TabItem tabItem = value as TabItem;
            return string.Format("Note {0}",
                ItemsControl.ItemsControlFromItemContainer(tabItem).ItemContainerGenerator.IndexFromContainer(tabItem) + 1);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Creates an alphabetic title for a tab based on its index within the collection.
    /// </summary>
    public sealed class IndexLetterConverter : IValueConverter
    {
        private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;

            string letter = value as string;
            return alphabet.IndexOf(letter);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "A";

            int index = (int)value;
            return alphabet[index];
        }
    }
}