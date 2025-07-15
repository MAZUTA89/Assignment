using BTS_Assignment.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MBox = System.Windows.MessageBox;

namespace BTS_Assignment.Assingment.EntryWindow
{
    /// <summary>
    /// Логика взаимодействия для EntryUserWindow.xaml
    /// </summary>
    public partial class EntryUserWindow : Window
    {
        public string CurrentUserName;
        AssignmentSettings _assignmentSettings { get; set; }
        AssignmentSettingsData _assignmentSettingsData => _assignmentSettings.LoadedData;
        BindingList<string> _usersConfigsTitles { get; set; }

        public EntryUserWindow()
        {
            InitializeComponent();
            _usersConfigsTitles = new BindingList<string>();
            _assignmentSettings = new AssignmentSettings();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            try
            {
                if (!_assignmentSettings.Load())
                {
                    return;
                }
                

                _usersConfigsTitles = new BindingList<string>(_assignmentSettingsData.EmployeeUsers);

                EmployeeSettingDataGrid.ItemsSource = _usersConfigsTitles;
            }
            catch (NullReferenceException ex)
            {
                MBox.Show("Настройки не загружены", "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            NewEmployeeTextBox.Focus();

            if (string.IsNullOrEmpty(NewEmployeeTextBox.Text))
            {
                MBox.Show("Поле пользователя не должно быть пустым.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            if (_assignmentSettingsData.EmployeeUsers.Contains(NewEmployeeTextBox.Text))
            {
                MBox.Show("Такая конфигурация уже существует", "Информация", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            _assignmentSettingsData.EmployeeUsers.Add(NewEmployeeTextBox.Text);
            _assignmentSettingsData.EmployeeSettings.Add(new EmployeeSettings() { LastFullName = NewEmployeeTextBox.Text });

            _assignmentSettings.Save(_assignmentSettingsData);

            LoadSettings();
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            NewEmployeeTextBox.Focus();

            if (EmployeeSettingDataGrid.SelectedIndex != -1)
            {
                if(EmployeeSettingDataGrid.SelectedItem is string selected)
                {
                    if(_assignmentSettingsData.EmployeeUsers.Contains(selected))
                    {
                        _assignmentSettingsData.EmployeeUsers.Remove(selected);

                        _assignmentSettings.Save(_assignmentSettingsData);
                    }
                }
                else
                {
                    MBox.Show("Поступил неожиданный формат данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MBox.Show("Выберите конфигурацию в таблице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _assignmentSettingsData.EmployeeUsers.Remove(NewEmployeeTextBox.Text);

            _assignmentSettings.Save(_assignmentSettingsData);

            LoadSettings();
        }

        private void SelectEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeSettingDataGrid.SelectedIndex != -1)
            {
                if(EmployeeSettingDataGrid.SelectedItem is string selected)
                {
                    CurrentUserName = selected;

                    EmployeeSettings employeeSettings = 
                        _assignmentSettingsData.EmployeeSettings.SingleOrDefault(emp => 
                    CurrentUserName ==
                    emp.LastFullName);

                    MainWindow mainWindow = new MainWindow();

                    if (employeeSettings == null)
                    {
                        employeeSettings = new EmployeeSettings();

                        employeeSettings.LastFullName = CurrentUserName;

                        _assignmentSettings.LoadedData.EmployeeSettings.Add(employeeSettings);

                        _assignmentSettings.Save(_assignmentSettings.LoadedData);
                    }

                    mainWindow.InitializeSettings(_assignmentSettings, employeeSettings);

                    mainWindow.Show();
                }
                else
                {
                    MBox.Show("Поступил неожиданный формат данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MBox.Show("Выберите конфигурацию в таблице.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
