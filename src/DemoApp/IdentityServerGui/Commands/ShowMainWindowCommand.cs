using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IdentityServerGui.Commands
{
    public class ShowMainWindowCommand : ICommand
    {
        private Window _window;
        private TaskbarIcon _taskbarIcon;
        public ShowMainWindowCommand(Window window, TaskbarIcon taskbarIcon)
        {
            _window = window;
            _taskbarIcon = taskbarIcon;
        }

        public void Execute(object parameter)
        {
            _window.Show();
            _taskbarIcon.Visibility = Visibility.Hidden;
            //MessageBox.Show(parameter.ToString());
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
