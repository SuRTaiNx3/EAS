﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EAS.Common
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    public class RelayCommand : ICommand
    {
        #region Property backing fields

        private readonly Action<object> m_Execute;
        private readonly Func<object, bool> m_CanExecute;
        private readonly Action<object> ExecuteAction;
        private volatile bool IsExecuting;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <exception cref="System.ArgumentNullException">execute</exception>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            // m_Execute = execute ?? throw new ArgumentNullException(nameof(execute)); // commenting out as this syntax '??' is only supported in VS 2017

            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            m_Execute = execute;
            m_CanExecute = canExecute;

            ExecuteAction = parameter =>
            {
                var canExecuteAction = m_CanExecute?.Invoke(parameter) ?? true;

                if (canExecuteAction)
                    m_Execute(parameter);
            };
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter = null)
        {
            if (IsExecuting) return false;
            return m_CanExecute == null || m_CanExecute(parameter);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public async void Execute(object parameter = null)
        {
            if (IsExecuting) return;

            try
            {
                IsExecuting = true;
                await Application.Current.Dispatcher.BeginInvoke(ExecuteAction, DispatcherPriority.Normal, parameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not execute command. {ex.Message}");
                throw ex;
            }
            finally
            {
                IsExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion
    }
}
