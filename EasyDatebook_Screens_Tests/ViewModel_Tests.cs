using EasyDatebook_Model;
using EasyDatebook_Screens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDatebook_Screens_Tests
{
    [TestClass]
    public class ViewModel_Tests
    {
        #region Notes

        /// <summary>
        /// Verifies that after adding and deleting text notes, the Notes collection contains the expected number of notes.
        /// </summary>
        [TestMethod]
        public void AddAndDeleteNotes_Test()
        {
            ViewModel vm = new ViewModel();
            vm.AddNote(null);
            Assert.IsTrue(vm.Notes.Count == 2);
            vm.DeleteNote(null);
            Assert.IsTrue(vm.Notes.Count == 1);
        }

        /// <summary>
        /// Verifies that after deleting the last text note, a new blank note automatically replaces it.
        /// </summary>
        [TestMethod]
        public void DeleteLastNote_Test()
        {
            ViewModel vm = new ViewModel();
            vm.DeleteNote(null);
            Assert.IsTrue(vm.Notes.Count == 1);
        }

        #endregion // Notes

        #region Budget

        /// <summary>
        /// Verifies that after adding or deleting an income item, the collection contains the expected number of items.
        /// </summary>
        [TestMethod]
        public void AddAndDeleteBudgetIncomes_Test()
        {
            ViewModel vm = new ViewModel();
            vm.AddIncome(null);
            Assert.IsTrue(vm.BudgetIncomes.Count == 1);
            vm.DeleteBudgetItem(vm.BudgetIncomes.GetItemAt(0));
            Assert.IsTrue(vm.BudgetIncomes.Count == 0);
        }

        /// <summary>
        /// Verifies that after adding or deleting an expense item, the collection contains the expected number of items.
        /// </summary>
        [TestMethod]
        public void AddAndDeleteBudgetExpenses_Test()
        {
            ViewModel vm = new ViewModel();
            vm.AddExpense(null);
            Assert.IsTrue(vm.BudgetExpenses.Count == 1);
            vm.DeleteBudgetItem(vm.BudgetExpenses.GetItemAt(0));
            Assert.IsTrue(vm.BudgetExpenses.Count == 0);
        }

        /// <summary>
        /// Verifies that after adding several income and expense items, the balance is correct.
        /// </summary>
        [TestMethod]
        public void BalancedBudget_Test()
        {
            ViewModel vm = new ViewModel();

            BudgetItem income1 = new BudgetItem();
            income1.Amount = 100;
            vm.BudgetIncomes.AddNewItem(income1);
            vm.BudgetExpenses.CommitNew();
            Assert.IsTrue(vm.TotalBudget == 100);

            BudgetItem income2 = new BudgetItem();
            income2.Amount = 200;
            vm.BudgetIncomes.AddNewItem(income2);
            vm.BudgetExpenses.CommitNew();
            Assert.IsTrue(vm.TotalBudget == 300);

            BudgetItem income3 = new BudgetItem();
            income3.Amount = 50;
            vm.BudgetIncomes.AddNewItem(income3);
            vm.BudgetExpenses.CommitNew();
            Assert.IsTrue(vm.TotalBudget == 350);

            BudgetItem expense1 = new BudgetItem();
            expense1.Amount = 75;
            vm.BudgetExpenses.AddNewItem(expense1);
            vm.BudgetExpenses.CommitNew();
            Assert.IsTrue(vm.TotalBudget == 275);

            BudgetItem expense2 = new BudgetItem();
            expense2.Amount = 150;
            vm.BudgetExpenses.AddNewItem(expense2);
            vm.BudgetExpenses.CommitNew();
            Assert.IsTrue(vm.TotalBudget == 125);

            BudgetItem expense3 = new BudgetItem();
            expense3.Amount = 25;
            vm.BudgetExpenses.AddNewItem(expense3);
            vm.BudgetExpenses.CommitNew();
            Assert.IsTrue(vm.TotalBudget == 100);
        }

        #endregion // Budget

        #region Address Book

        /// <summary>
        /// Verifies that after adding and deleting address book entries, the AddressBookEntries collection contains the expected number of entries.
        /// </summary>
        [TestMethod]
        public void AddAndDeleteAddrEntries_Test()
        {
            ViewModel vm = new ViewModel();
            vm.AddAddrEntry(null);
            Assert.IsTrue(vm.AddressBookEntries.Count == 2);
            vm.AddressBookEntries.MoveCurrentToLast();
            vm.DeleteAddrEntry(vm.AddressBookEntries.CurrentItem);
            Assert.IsTrue(vm.AddressBookEntries.Count == 1);
        }

        /// <summary>
        /// Verifies that after deleting the last address book entry, a new blank entry automatically replaces it.
        /// </summary>
        [TestMethod]
        public void DeleteLastAddrEntry_Test()
        {
            ViewModel vm = new ViewModel();
            vm.AddressBookEntries.MoveCurrentToLast();
            vm.DeleteAddrEntry(vm.AddressBookEntries.CurrentItem);
            Assert.IsTrue(vm.AddressBookEntries.Count == 1);
        }

        #endregion // Address Book
    }
}
