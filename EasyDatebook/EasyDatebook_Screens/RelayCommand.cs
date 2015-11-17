using System;
using System.Windows.Input;

namespace EasyDatebook_Screens
{
    /// <summary>
    /// An implementation of the ICommand interface.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> execute;

        private Predicate<object> canExecute;

        private event EventHandler canExecuteChanged;

        /// <summary>
        /// Constructs a Command with a given Action for Execute, and a specified Predicate for CanExecute.
        /// </summary>
        /// <param name="execute">An Action for the Execute method of this Command.</param>
        /// <param name="canExecute">A Predicate for the CanExecute method of this Command.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            if (canExecute == null) throw new ArgumentNullException("canExecute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Constructs a Command with a given Action for Execute.
        /// </summary>
        /// <param name="execute">An Action for the Execute method of this Command.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, DefaultCanExecute) { }

        /// <inheritdoc/>
        public void Execute(object parameter) { this.execute(parameter); }

        /// <inheritdoc/>
        public bool CanExecute(object parameter)
        {
            return this.canExecute != null && this.canExecute(parameter);
        }

        private static bool DefaultCanExecute(object parameter) { return true; }

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.canExecuteChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                this.canExecuteChanged -= value;
            }
        }

        /// <summary>
        /// Should be called when CanExecute has been changed to notify listeners.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            EventHandler handler = this.canExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Cleans instance before garbage collection.
        /// </summary>
        public void Destroy()
        {
            canExecute = _ => false;
            execute = _ => { return; };
        }
    }
}
