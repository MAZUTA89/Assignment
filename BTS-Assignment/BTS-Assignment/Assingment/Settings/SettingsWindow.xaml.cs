using System;
using System.Collections.Generic;
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
using RegistryHelper;
using RegistryHelper.JSONDataSerializing;
using RegistryHelper.UserRegistries;
using RegistryHelper.RegistryEncryption;
using DocumentFormat.OpenXml.Wordprocessing;
using BTS_Assignment.Assingment.EntryWindow;

namespace BTS_Assignment.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        AssignmentSettingsData _assignmentSettingsData => _assingmentSettings.LoadedData;
        AssignmentSettings _assingmentSettings;
        MainWindow _mainWindow;
        EmployeeSettings _employeeSettings;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void Initialize(AssignmentSettings assignmentSettings, 
            MainWindow mainWindow)
        {
            _assingmentSettings = assignmentSettings;
            _mainWindow = mainWindow;
            Title += mainWindow.CurrentUserName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _assingmentSettings.Load();

                _employeeSettings = _assignmentSettingsData.EmployeeSettings.FirstOrDefault(emp => emp.LastFullName == 
                _mainWindow.EmployeeSettings.LastFullName);

                InitializeSettingUI(_assignmentSettingsData, _employeeSettings);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Настройки не загружены", "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.GetType(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void InitializeSettingUI(AssignmentSettingsData assingmentSettingsData, EmployeeSettings employeeSettings)
        {
            if(employeeSettings.AssignmentNumber != null)
            {
                AssignNumberTextBox.Text = employeeSettings.AssignmentNumber;
            }
            
            if(employeeSettings.AssignmentPlace != null)
            {
                PlaceTextBox.Text = employeeSettings.AssignmentPlace;
            }
            if(employeeSettings.LastFullName != null)
            {
                FullNameTextBox.Text = employeeSettings.LastFullName;
            }
            if(employeeSettings.TrustNumber != null)
            {
                TrustNumberTextBox.Text = employeeSettings.TrustNumber;
            }

            foreach (ComboBoxItem item in PostComboBox.Items)
            {
                if(item.Content is string post)
                {
                    if(post == employeeSettings.LastPost)
                    {
                        int index = PostComboBox.Items.IndexOf(item);
                        PostComboBox.SelectedIndex = index;
                        break;
                    }
                }
            }

            AddCommonRowCheckBox.IsChecked = employeeSettings.IsAddCommonRow;
            RuCountriesPriorityCheckBox.IsChecked = employeeSettings.IsRuCountriesCodePriority;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Activate();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();

            if(ApplySettingCheckBox.IsChecked == true)
            {
                _mainWindow.LoadSettings();
            }
        }

        void SaveSettings()
        {
            try
            {
                _employeeSettings.AssignmentPlace = PlaceTextBox.Text;

                _employeeSettings.AssignmentNumber = AssignNumberTextBox.Text;

                if(FullNameTextBox.Text != null)
                {
                    _employeeSettings.LastFullName = FullNameTextBox.Text.ToString();
                }

                if (PostComboBox.SelectedItem != null)
                {
                    _employeeSettings.LastPost = (PostComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                }

                if(TrustNumberTextBox.Text != null)
                {
                    _employeeSettings.TrustNumber = TrustNumberTextBox.Text;
                }

                _employeeSettings.IsAddCommonRow = (bool)AddCommonRowCheckBox.IsChecked;

                _employeeSettings.IsRuCountriesCodePriority = (bool)RuCountriesPriorityCheckBox.IsChecked;

                _assingmentSettings.Save(_assignmentSettingsData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
