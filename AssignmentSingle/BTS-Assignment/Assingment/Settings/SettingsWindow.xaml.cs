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

namespace BTS_Assignment.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        AssignmentSettingsData _assignmentSettingsData => _assingmentSettings.LoadedData;
        AssingmentSettings _assingmentSettings;
        MainWindow _mainWindow;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void Initialize(AssingmentSettings assignmentSettings, MainWindow mainWindow)
        {
            _assingmentSettings = assignmentSettings;
            _mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _assingmentSettings.Load();
                InitializeSettingUI(_assignmentSettingsData);
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

        void InitializeSettingUI(AssignmentSettingsData assingmentSettings)
        {
            if(assingmentSettings.AssinmentNumber != null)
            {
                AssignNumberTextBox.Text = assingmentSettings.AssinmentNumber;
            }
            
            if(assingmentSettings.AssignmentPlace != null)
            {
                PlaceTextBox.Text = assingmentSettings.AssignmentPlace;
            }
            if(assingmentSettings.LastFullName != null)
            {
                FullNameTextBox.Text = assingmentSettings.LastFullName;
            }
            if(assingmentSettings.TrustNumber != null)
            {
                TrustNumberTextBox.Text = _assignmentSettingsData.TrustNumber;
            }

            foreach (ComboBoxItem item in PostComboBox.Items)
            {
                if(item.Content is string post)
                {
                    if(post == _assignmentSettingsData.LastPost)
                    {
                        int index = PostComboBox.Items.IndexOf(item);
                        PostComboBox.SelectedIndex = index;
                        break;
                    }
                }
            }

            AddCommonRowCheckBox.IsChecked = assingmentSettings.IsAddCommonRow;
            RuCountriesPriorityCheckBox.IsChecked = assingmentSettings.IsRuCountriesCodePriority;
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
                _assignmentSettingsData.AssignmentPlace = PlaceTextBox.Text;

                _assignmentSettingsData.AssinmentNumber = AssignNumberTextBox.Text;

                if(FullNameTextBox.Text != null)
                {
                    _assignmentSettingsData.LastFullName = FullNameTextBox.Text.ToString();
                }

                if (PostComboBox.SelectedItem != null)
                {
                    _assignmentSettingsData.LastPost = (PostComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                }

                if(TrustNumberTextBox.Text != null)
                {
                    _assignmentSettingsData.TrustNumber = TrustNumberTextBox.Text;
                }

                _assignmentSettingsData.IsAddCommonRow = (bool)AddCommonRowCheckBox.IsChecked;

                _assignmentSettingsData.IsRuCountriesCodePriority = (bool)RuCountriesPriorityCheckBox.IsChecked;

                _assingmentSettings.Save(_assignmentSettingsData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
