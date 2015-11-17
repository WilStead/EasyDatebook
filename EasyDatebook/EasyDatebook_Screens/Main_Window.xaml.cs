using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace EasyDatebook_Screens
{
	/// <summary>
	/// Interaction logic for Main_Screen.xaml
	/// </summary>
    public partial class Main_Window : Window, INotifyPropertyChanged
	{
        private EmbeddedMediaPlayer mediaPlayer;

        private HelpPlaylist calendarHelpPlaylist;
        /// <summary>
        /// Indicates whether the calendarHelpPlaylist has IsAutoplay enabled.
        /// </summary>
        public bool CalendarHelpAutoplay { get { return calendarHelpPlaylist.IsAutoplay; } }
        /// <summary>
        /// Indicates the CurrentTrack of calendarHelpPlaylist.
        /// </summary>
        public int CalendarHelpStep { get { return calendarHelpPlaylist.CurrentTrack; } }

        private HelpPlaylist addressBookHelpPlaylist;
        /// <summary>
        /// Indicates whether the addressBookHelpPlaylist has IsAutoplay enabled.
        /// </summary>
        public bool AddressBookHelpAutoplay { get { return addressBookHelpPlaylist.IsAutoplay; } }
        /// <summary>
        /// Indicates the CurrentTrack of addressBookHelpPlaylist.
        /// </summary>
        public int AddressBookHelpStep { get { return addressBookHelpPlaylist.CurrentTrack; } }

        private HelpPlaylist budgetHelpPlaylist;
        /// <summary>
        /// Indicates whether the budgetHelpPlaylist has IsAutoplay enabled.
        /// </summary>
        public bool BudgetHelpAutoplay { get { return budgetHelpPlaylist.IsAutoplay; } }
        /// <summary>
        /// Indicates the CurrentTrack of budgetHelpPlaylist.
        /// </summary>
        public int BudgetHelpStep { get { return budgetHelpPlaylist.CurrentTrack; } }

        private HelpPlaylist notesHelpPlaylist;
        /// <summary>
        /// Indicates whether the notesHelpPlaylist has IsAutoplay enabled.
        /// </summary>
        public bool NotesHelpAutoplay { get { return notesHelpPlaylist.IsAutoplay; } }
        /// <summary>
        /// Indicates the CurrentTrack of notesHelpPlaylist.
        /// </summary>
        public int NotesHelpStep { get { return notesHelpPlaylist.CurrentTrack; } }

        private int currentTab = -1;

        /// <summary>
        /// Default constructor.
        /// </summary>
		public Main_Window()
		{
            mediaPlayer = new EmbeddedMediaPlayer();

            calendarHelpPlaylist = new HelpPlaylist(mediaPlayer, new List<Uri>()
            {
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Calendar0.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Calendar1.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Calendar2.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Calendar3.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Calendar4.mp3")
            });
            calendarHelpPlaylist.PropertyChanged += calendarHelpPlaylist_PropertyChanged;

            addressBookHelpPlaylist = new HelpPlaylist(mediaPlayer, new List<Uri>()
            {
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/AddrBook0.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/AddrBook1.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/AddrBook2.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/AddrBook3.mp3")
            });
            addressBookHelpPlaylist.PropertyChanged += addressBookHelpPlaylist_PropertyChanged;

            budgetHelpPlaylist = new HelpPlaylist(mediaPlayer, new List<Uri>()
            {
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Budget0.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Budget1.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Budget2.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Budget3.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Budget4.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Budget5.mp3")
            });
            budgetHelpPlaylist.PropertyChanged += budgetHelpPlaylist_PropertyChanged;

            notesHelpPlaylist = new HelpPlaylist(mediaPlayer, new List<Uri>()
            {
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Notes0.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Notes1.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Notes2.mp3"),
                new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Notes3.mp3")
            });
            notesHelpPlaylist.PropertyChanged += notesHelpPlaylist_PropertyChanged;

			this.InitializeComponent();
            DataContext = new ViewModel();
            ((ViewModel)DataContext).CalendarRefreshRequested += Main_Window_CalendarRefreshRequested;
		}

        private static T FindAncestor<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null) return parent;
            else return FindAncestor<T>(parentObject);
        }

        private void Main_Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MonthCalendar.DisplayMode == CalendarMode.Month)
                MonthCalendar.DisplayMode = CalendarMode.Year;
            DayCalendar.SelectedDate = DayCalendar.DisplayDate;
            mediaPlayer.PlayResource(new Uri("pack://application:,,,/EasyDatebook.Screens;component/AudioFiles/Intro.mp3"));
        }

        private void Window_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void mainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainTabControl.SelectedIndex != -1 &&
                currentTab != mainTabControl.SelectedIndex)
            {
                StopAudio();
                currentTab = mainTabControl.SelectedIndex;
            }
        }

        #region Calendar

        private void Main_Window_CalendarRefreshRequested(object sender, System.EventArgs e)
        {
            MonthCalendar.Refresh();
            DayCalendar.Refresh();
        }

        private void DayCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // Bug in .NET WPF Calendar control causes it to display incorrectly
            // if this is enabled:
            //if (MonthCalendar.DisplayMode == CalendarMode.Month)
            //    MonthCalendar.DisplayMode = CalendarMode.Year;

            ((ViewModel)DataContext).UpdateCalendarNotes();

            // Autoplay will cause the tutorial to end automatically after explaining
            // birthdays, since that step doesn't require user interaction.
            if (calendarHelpPlaylist.CurrentTrack == 3)
                calendarHelpPlaylist.IsAutoplay = true;

            if (calendarHelpPlaylist.CurrentTrack == 1 ||
                calendarHelpPlaylist.CurrentTrack == 3)
                calendarHelpPlaylist.QueueNextTrack();
        }

        private void DayCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (MonthCalendar.DisplayMode == CalendarMode.Month)
                MonthCalendar.DisplayMode = CalendarMode.Year;
        }

        private void CalendarNotesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
            {
                RichTextBoxBinder.SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox.Document));
                ((ViewModel)DataContext).UpdateCalendarNotes();
            }
        }

        #endregion Calendar

        #region Address Book

        private void AddrEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddrEntryList.SelectedItem != null)
                AddrEntryList.ScrollIntoView(AddrEntryList.SelectedItem);
        }

        private void AddrEntryField_LostFocus(object sender, RoutedEventArgs e)
        {
            ListViewItem item = FindAncestor<ListViewItem>(sender as DependencyObject);
            if (item != null)
            {
                AddrEntryList.Items.MoveCurrentTo(item);
                AddrEntryList.SelectedItem = item;
                AddrEntryList.ScrollIntoView(item);
            }
        }

        #endregion Address Book

        #region Notes

        private void Notes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Control c = sender as Control;
            c.Focus();
        }

        private void NoteTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            if (richTextBox != null)
            {
                RichTextBoxBinder.SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox.Document));
            }
        }

        #endregion Notes

        private void StopAudio()
        {
            calendarHelpPlaylist.Stop();
            addressBookHelpPlaylist.Stop();
            budgetHelpPlaylist.Stop();
            notesHelpPlaylist.Stop();
            mediaPlayer.Stop();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop anything already in progress.
            StopAudio();

            switch (mainTabControl.SelectedIndex)
            {
                case 0: // Calendar
                    calendarHelpPlaylist.PlayEntireList();
                    break;
                case 1: // Address book
                    addressBookHelpPlaylist.PlayEntireList();
                    break;
                case 2: // Budget
                    budgetHelpPlaylist.PlayEntireList();
                    break;
                case 3: // Notes
                    notesHelpPlaylist.PlayEntireList();
                    break;
                default: return;
            }
        }

        #region Tutorials

        private void TutorialButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop anything already in progress.
            StopAudio();

            switch (mainTabControl.SelectedIndex)
            {
                case 0: // Calendar
                    CalendarTutorial();
                    break;
                case 1: // Address book
                    AddressBookTutorial();
                    break;
                case 2: // Budget
                    BudgetTutorial();
                    break;
                case 3: // Notes
                    NotesTutorial();
                    break;
                default: return;
            }
        }

        #region Calendar

        private void CalendarTutorial()
        {
            calendarHelpPlaylist.PlayFirstTrack();
            calendarHelpPlaylist.QueueNextTrack();
        }

        private void CalendarNotesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (calendarHelpPlaylist.CurrentTrack == 2)
                calendarHelpPlaylist.QueueNextTrack();
            e.Handled = false;
        }

        #endregion // Calendar

        #region Address Book

        private void AddressBookTutorial()
        {
            addressBookHelpPlaylist.PlayFirstTrack();
            addressBookHelpPlaylist.QueueNextTrack();
        }

        private void AddAddrEntryButton_Click(object sender, RoutedEventArgs e)
        {
            if (addressBookHelpPlaylist.CurrentTrack == 1)
                addressBookHelpPlaylist.QueueNextTrack();
            e.Handled = false;
        }

        private void AddrEntryField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (addressBookHelpPlaylist.CurrentTrack == 2)
                addressBookHelpPlaylist.QueueNextTrack();
            e.Handled = false;
        }

        private void AddrAlphaTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addressBookHelpPlaylist.CurrentTrack == 3)
                addressBookHelpPlaylist.Advance();
            e.Handled = false;
        }

        #endregion // Address Book

        #region Budget

        private void BudgetTutorial()
        {
            budgetHelpPlaylist.PlayFirstTrack();
            budgetHelpPlaylist.QueueNextTrack();
        }

        private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (budgetHelpPlaylist.CurrentTrack == 1)
                budgetHelpPlaylist.QueueNextTrack();
            e.Handled = false;
        }

        private void BudgetItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Autoplay will cause the tutorial to end automatically after explaining
            // the balance, since that step doesn't require user interaction.
            if (budgetHelpPlaylist.CurrentTrack == 4)
                budgetHelpPlaylist.IsAutoplay = true;

            if (budgetHelpPlaylist.CurrentTrack == 2 ||
                budgetHelpPlaylist.CurrentTrack == 4)
                budgetHelpPlaylist.QueueNextTrack();

            e.Handled = false;
        }

        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (budgetHelpPlaylist.CurrentTrack == 3)
                budgetHelpPlaylist.QueueNextTrack();
            e.Handled = false;
        }

        #endregion // Budget

        #region Notes

        private void NotesTutorial()
        {
            notesHelpPlaylist.PlayFirstTrack();
            notesHelpPlaylist.QueueNextTrack();
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (notesHelpPlaylist.CurrentTrack == 1)
                notesHelpPlaylist.QueueNextTrack();
            e.Handled = false;
        }

        private void NoteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (notesHelpPlaylist.CurrentTrack == 2)
                notesHelpPlaylist.QueueNextTrack();
            e.Handled = false;
        }

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (notesHelpPlaylist.CurrentTrack == 3)
                notesHelpPlaylist.Advance(); // Advancing from the final track will reset the playlist.
            e.Handled = false;
        }

        #endregion // Notes

        #endregion // Tutorials

        #region Event Handling

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

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

        private void calendarHelpPlaylist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("CalendarHelpAutoplay");
            OnPropertyChanged("CalendarHelpStep");
        }

        private void addressBookHelpPlaylist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("AddressBookHelpAutoplay");
            OnPropertyChanged("AddressBookHelpStep");
        }

        private void budgetHelpPlaylist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("BudgetHelpAutoplay");
            OnPropertyChanged("BudgetHelpStep");
        }

        private void notesHelpPlaylist_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("NotesHelpAutoplay");
            OnPropertyChanged("NotesHelpStep");
        }

        #endregion // Event Handling
    }
}