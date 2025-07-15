using BTS_Assignment.Data;
using BTS_Assignment.Printing;
using BTS_Assignment.Settings;
using BTS_Assignment.XMLReading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MBox = System.Windows.MessageBox;
using System.Diagnostics;
using System.IO;
using BTS_Assignment.Assingment.EntryWindow;



namespace BTS_Assignment
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string CurrentUserName;
        public const string c_ProgramInfoPass = @"Resources\РуководствоПоручение.docx";
        public TrucksData TrucksData { get; private set; }
        public CountryService CountryService { get; private set; }
        public IXMLParser TransitXmlParser { get; private set; }

        private Task _contriesLoadTask { get; set; }

        private AssignmentData _assignmentData { get; set; }

        private AssignDeclarationInitializer _assignDeclarationInitializer;

        private SettingsWindow _settingsWindow;

        private AssignmentSettings _assignmentSettings;

        AssignPrinting _assignPrinting;

        public EmployeeSettings EmployeeSettings;

        public MainWindow()
        {
            InitializeComponent();
            TrucksData = new TrucksData();
            CountryService = new CountryService();

            _assignDeclarationInitializer = new AssignDeclarationInitializer(this,
                CountryService);

            _assignmentSettings = new AssignmentSettings();
            _assignmentData = new AssignmentData();

           
        }

        public void InitializeSettings(AssignmentSettings assignmentSettings,
            EmployeeSettings employeeSettings)
        {
            _assignmentSettings = assignmentSettings;

            EmployeeSettings = employeeSettings;

            CurrentUserName = EmployeeSettings.LastFullName;
            
            Title += CurrentUserName;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();

            try
            {
                if (!await TrucksData.TryInitializeAsync())
                {
                    throw new Exception($"Resource {TrucksData.c_TrucksPath} read error");
                }

                TransitXmlParser = new TransitDeclarationXmlParser(TrucksData,
                    CountryService);

                _contriesLoadTask = CountryService.InitializeAsync();

            }
            catch (Exception ex)
            {
                MBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadSettings()
        {
            try
            {
                _assignmentSettings?.Load();

                EmployeeSettings = _assignmentSettings.LoadedData
                    .EmployeeSettings.SingleOrDefault(
                    emp => emp.LastFullName == 
                CurrentUserName);

                AssignPlaceTextBox.Text = EmployeeSettings.AssignmentPlace;

                CurrentDatePicker.SelectedDate = DateTime.Now;

                if (EmployeeSettings.AssignmentDate != null)
                {
                    ContractDatePicker.SelectedDate =
                    DateTime.Parse(EmployeeSettings.AssignmentDate);
                }

                AssinmentNumberTextBox.Text = EmployeeSettings.AssignmentNumber;
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

        private async Task ParseFile(string filePath)
        {
            try
            {
                _assignmentData = new AssignmentData();
                _assignmentData = await TransitXmlParser.ParseXmlAsync(filePath);

                _assignDeclarationInitializer.Initialize(_assignmentData, _assignmentSettings.LoadedData);
            }
            catch (Exception ex)
            {
                MBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LowePrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintMenuItem_Click(sender, e);
        }

        private async void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_assignmentSettings.LoadedData.IsRuCountriesCodePriority)
            {
                if (!_contriesLoadTask.IsCompleted)
                {
                    if (CountryService.IsInitialized == false)
                    {
                        if (MBox.Show("Не удалось выгрузить перевод кодов из ресурса.\n" +
                            "Попробовать загрузить повторно?",
                        "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            _contriesLoadTask = CountryService.InitializeAsync();

                            await _contriesLoadTask;

                            LoadMenuItem_Click(sender, e);
                        }

                    }
                    return;
                }
            }


            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = false;
                dialog.RestoreDirectory = true;

                dialog.Filter = "XML files (*.xml)|*.xml";
                dialog.Title = "Выберите .xml файл.";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filePath = dialog.FileName;

                    await ParseFile(filePath);
                }
            }
        }

        private async void PrintMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _assignPrinting = new AssignPrinting(this, _assignmentSettings.LoadedData,
                    _assignmentData, EmployeeSettings);
                await _assignPrinting.PrintAsync(_assignPrinting.c_TemplatePath);
            }
            catch (Exception ex)
            {
                MBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow = new SettingsWindow();
            _settingsWindow.Initialize(_assignmentSettings, this);
            _settingsWindow.Show();
            _settingsWindow.Owner = this;
            _settingsWindow.Activate();
        }

        private void LowerLoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMenuItem_Click(sender, e);
        }

        private async void PrintEmptyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _assignPrinting = new AssignPrinting(this, _assignmentSettings.LoadedData, _assignmentData, EmployeeSettings
                    );
                await _assignPrinting.PrintEmptyAsync(_assignPrinting.c_EmptyTemplatePath);
            }
            catch (Exception ex)
            {
                MBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoodsDetailsDataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var dataGrid = sender as System.Windows.Controls.DataGrid;

                // Проверяем, что строка выбрана
                if (dataGrid?.SelectedItem != null)
                {
                    // Получаем привязанную коллекцию
                    var collection = dataGrid.ItemsSource as ObservableCollection<GoodsDetailData>;

                    // Удаляем выбранный элемент
                    collection?.Remove(dataGrid.SelectedItem as GoodsDetailData);

                    // Блокируем дальнейшую обработку события
                    e.Handled = false;
                }
            }
        }

        private void ReferenceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string copyPath = Path.Combine(
                Path.Combine(Path.GetDirectoryName(c_ProgramInfoPass),
                "_copy" + Path.GetFileName(c_ProgramInfoPass)));

            try
            {
                if (File.Exists(c_ProgramInfoPass))
                {
                    File.Copy(c_ProgramInfoPass, copyPath, true);
                }
            }
            catch(Exception ex)
            {
                MBox.Show("Файл уже открыт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (File.Exists(copyPath))
            {
                Process.Start(new ProcessStartInfo(copyPath)).Start();
            }
        }
    }
}
