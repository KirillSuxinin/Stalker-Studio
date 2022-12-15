using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Stalker_Studio
{
    class Interface
    {
        public static void InitializeWindow(Window window)
        {
            // обработчики команд окна

            window.CommandBindings.Add(new CommandBinding(
                SystemCommands.CloseWindowCommand,
                (object sender, ExecutedRoutedEventArgs e) =>
                {
                    window.Close();
                },
                (object sender, CanExecuteRoutedEventArgs e) =>
                {
                    e.CanExecute = true;
                }
                ));

            window.CommandBindings.Add(new CommandBinding(
                SystemCommands.MaximizeWindowCommand,
                (object sender, ExecutedRoutedEventArgs e) =>
                {
                    if (window.WindowState == WindowState.Maximized)
                        window.WindowState = WindowState.Normal;
                    else
                        window.WindowState = WindowState.Maximized;
                },
                (object sender, CanExecuteRoutedEventArgs e) =>
                {
                    e.CanExecute = true;
                }
                ));

            window.CommandBindings.Add(new CommandBinding(
                SystemCommands.MinimizeWindowCommand,
                (object sender, ExecutedRoutedEventArgs e) =>
                {
                    window.WindowState = WindowState.Minimized;
                },
                (object sender, CanExecuteRoutedEventArgs e) =>
                {
                    e.CanExecute = true;
                }
                ));
        }

    }
}
