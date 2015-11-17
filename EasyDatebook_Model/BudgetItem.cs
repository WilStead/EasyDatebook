using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyDatebook_Model
{
    /// <summary>
    /// A budget item object. A collection of these can represent a monthly budget.
    /// </summary>
    public class BudgetItem : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        /// <summary>
        /// The name/description of this budget item.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private double _amount;
        /// <summary>
        /// The amount of this budget item.
        /// </summary>
        /// <remarks>
        /// No particular currency is presumed. The program should automatically detect
        /// the local currrency setting of the user's system and use it to format
        /// displayed values.
        /// </remarks>
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        /// <summary>
        /// The default contructor.
        /// </summary>
        public BudgetItem() { Name = "<budget item>"; }

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
