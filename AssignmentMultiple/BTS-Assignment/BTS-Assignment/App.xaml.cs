using BTS_Assignment.Assingment.EntryWindow;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BTS_Assignment
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            EntryUserWindow entryUserWindow = new EntryUserWindow();

            entryUserWindow.Show();
        }
    }
}
