using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDatebook_Model;
using System.IO;

namespace EasyDatebook_Model_Tests
{
    [TestClass]
    public class Datebook_Tests
    {
        #region SaveData
        /// <summary>
        /// Verifies that SaveData saves both the main save file and the backup file,
        /// and that it clears the HasUnsavedData flag.
        /// </summary>
        [TestMethod]
        public void SaveData_Test()
        {
            Datebook tester = new Datebook();
            tester.SaveData();
            Assert.IsTrue(!tester.HasUnsavedData);
            Assert.IsTrue(File.Exists(Datebook.GetSaveFilePath()));
            Assert.IsTrue(File.Exists(Datebook.GetBackupFilePath()));
        }

        /// <summary>
        /// Verifies that SaveData sets the HasUnsavedData flag when both the main save file and
        /// the backup file are inaccessible.
        /// </summary>
        [TestMethod]
        public void SaveData_Test_Fail()
        {
            Datebook tester = new Datebook();
            FileStream save = File.OpenWrite(Datebook.GetSaveFilePath());
            FileStream backup = File.OpenWrite(Datebook.GetBackupFilePath());
            tester.SaveData();
            save.Close();
            backup.Close();
            Assert.IsTrue(tester.HasUnsavedData);
        }
        #endregion

        #region FromFile
        /// <summary>
        /// Verifies that FromFile correctly generates a Datebook object from the saved file.
        /// </summary>
        [TestMethod]
        public void FromFile_Test()
        {
            if (!File.Exists(Datebook.GetSaveFilePath()))
            {
                Datebook tester = new Datebook();
                tester.SaveData();
            }
            Datebook.FromFile();
        }

        /// <summary>
        /// Verifies that FromFile correctly generates a new Datebook object when no saved
        /// file or backup exists.
        /// </summary>
        [TestMethod]
        public void FromFile_Test_NoFile()
        {
            if (File.Exists(Datebook.GetSaveFilePath()))
                File.Delete(Datebook.GetSaveFilePath());
            if (File.Exists(Datebook.GetBackupFilePath()))
                File.Delete(Datebook.GetBackupFilePath());
            Datebook.FromFile();
        }

        /// <summary>
        /// Verifies that FromFile correctly generates a Datebook object from the backup
        /// file when the main save file is missing.
        /// </summary>
        [TestMethod]
        public void FromFile_Test_BackupFile()
        {
            if (!File.Exists(Datebook.GetBackupFilePath()))
            {
                Datebook tester = new Datebook();
                tester.SaveData();
            }
            if (File.Exists(Datebook.GetSaveFilePath()))
                File.Delete(Datebook.GetSaveFilePath());
            Datebook.FromFile();
        }
        #endregion
    }
}
