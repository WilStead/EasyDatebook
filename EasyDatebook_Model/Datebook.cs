﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace EasyDatebook_Model
{
    /// <summary>
    /// The main data model for the EasyDatebook program, which collects all user data
    /// and provides saving and loading mechanisms for that data.
    /// </summary>
    [DataContract]
    public class Datebook
    {
        /// <summary>
        /// A collection of AddressBookEntries representing an address book.
        /// </summary>
        [DataMember]
        public ObservableCollection<AddressBookEntry> AddressBookEntries { get; set; }

        /// <summary>
        /// A collection of BudgetItems representing a monthly budget's incomes.
        /// </summary>
        [DataMember]
        public ObservableCollection<BudgetItem> BudgetIncomes { get; set; }

        /// <summary>
        /// A collection of BudgetItems representing a monthly budget's expenses.
        /// </summary>
        [DataMember]
        public ObservableCollection<BudgetItem> BudgetExpenses { get; set; }

        /// <summary>
        /// A collection of notes indexed by date.
        /// </summary>
        [DataMember]
        public Dictionary<DateTime, Note> CalendarNotes { get; set; }

        /// <summary>
        /// Indicates whether or not this DateBook object has changed since the last
        /// successful save operation.
        /// </summary>
        /// <remarks>
        /// Since saving is automatic upon data changes, when this is true it means
        /// a save operation failed. Failures may be transitory due to overlapping
        /// save attempts, in which case this value may be cleared when the next save
        /// operation completes.
        /// </remarks>
        public bool HasUnsavedData { get; private set; }

        /// <summary>
        /// A collection of notes.
        /// </summary>
        [DataMember]
        public ObservableCollection<Note> Notes { get; set; }

        /// <summary>
        /// The default contructor.
        /// </summary>
        public Datebook()
        {
            CalendarNotes = new Dictionary<DateTime, Note>();
            AddressBookEntries = new ObservableCollection<AddressBookEntry>() { new AddressBookEntry() };
            BudgetIncomes = new ObservableCollection<BudgetItem>();
            BudgetExpenses = new ObservableCollection<BudgetItem>();

            // Always initialize with one empty entry so there is a blank note to start.
            Notes = new ObservableCollection<Note>() { new Note() };
        }

        private static Datebook FromPath(string path)
        {
            using (StreamReader reader = File.OpenText(path))
            {
                using (XmlReader xmlReader = XmlReader.Create(reader))
                {
                    DataContractSerializer deserializer = new DataContractSerializer(typeof(Datebook));

                    return (Datebook)deserializer.ReadObject(xmlReader);
                }
            }
        }

        /// <summary>
        /// Attempts to create a Datebook object by loading the main save file,
        /// or by restoring the backup if the main save file can't be accessed.
        /// </summary>
        /// <returns>
        /// A Datebook object generated by deserializing the main save file, or
        /// the backup file if necessary.
        /// </returns>
        /// <exception cref="Exception"/>
        public static Datebook FromFile()
        {
            string path = GetSaveFilePath();

            // If no existing data is found, create a new data object.
            if (!File.Exists(path) && !RecoverBackup()) return new Datebook();

            try { return FromPath(path); }
            catch (Exception ex)
            {
                if (RecoverBackup())
                {
                    try { return FromPath(path); }
                    catch (Exception ex2) { throw ex2; }
                }
                else throw ex;
            }
        }

        /// <summary>
        /// Obtains the path of the backup file.
        /// </summary>
        /// <remarks>
        /// The default path is "My Documents\Datebook.xml.bak"
        /// Even though this method is static, since My Documents is unique
        /// for each Windows profile, a different backup file will be
        /// maintained for each Windows user on a system.
        /// </remarks>
        /// <returns>The path of the backup file.</returns>
        public static string GetBackupFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Datebook.xml.bak");
        }

        /// <summary>
        /// Obtains the path of the main save file.
        /// </summary>
        /// <remarks>
        /// The default path is "My Documents\Datebook.xml"
        /// Even though this method is static, since My Documents is unique
        /// for each Windows profile, a different save file will be
        /// maintained for each Windows user on a system.
        /// </remarks>
        /// <returns>The path of the backup file.</returns>
        public static string GetSaveFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Datebook.xml");
        }

        /// <summary>
        /// Attempts to replace the main save file with the backup.
        /// </summary>
        /// <returns>
        /// True if the backup was successfully recovered.
        /// False if there is no backup or if copying it fails for any reason.
        /// </returns>
        private static bool RecoverBackup()
        {
            string backupPath = GetBackupFilePath();

            if (File.Exists(backupPath))
            {
                try
                {
                    File.Copy(backupPath, GetSaveFilePath());
                    return true;
                }
                catch (Exception) { } // No need for handling here: a false return value will be the result below.
            }
            return false;
        }

        /// <summary>
        /// Attempts to save all data to the main save file and the backup.
        /// </summary>
        /// <remarks>
        /// Failures go unreported, but the object remembers whether it has any unsaved
        /// data until a save operation completes successfully, so that the user may
        /// be warned before closing the application when any data remains unsaved.
        /// </remarks>
        public void SaveData()
        {
            var serializer = new DataContractSerializer(typeof(Datebook));

            string savePath = GetSaveFilePath();

            try
            {
                using (StreamWriter stream = new StreamWriter(savePath))
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
                        {
                            xmlWriter.Formatting = Formatting.Indented;

                            serializer.WriteObject(xmlWriter, this);
                            xmlWriter.Flush();

                            stream.Write(stringWriter.ToString());

                            // Clear internal flag for unsaved data now that the save operation has finished.
                            HasUnsavedData = false;
                        }
                    }
                }

                // Save a backup copy if no exceptions aborted the main save.
                string backupPath = GetBackupFilePath();
                try { File.Copy(savePath, backupPath); }
                catch (Exception) { } // Failure to create a backup is ignored.
            }
            catch (Exception)
            {
                // Set the internal flag that some data remains unsaved.
                HasUnsavedData = true;
            }
        }
    }
}
