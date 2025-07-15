using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS_Assignment.Settings;
using BTS_Assignment.Data;
using System.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using System.Diagnostics;
using DocumentFormat.OpenXml.Office.CustomUI;
using RichRun = System.Windows.Documents;


namespace BTS_Assignment.Printing
{
    public class AssignPrinting
    {
        public string c_TemplatePath = @"Templates\Поручение1.docx";
        public string c_EmptyTemplatePath = @"Templates\ПоручениеEmpty.docx";
        MainWindow _mainWindow;
        AssignmentSettingsData _assingmentSettingsData;
        AssignmentData _assignmentData;

        public AssignPrinting(MainWindow mainWindow, AssignmentSettingsData assignmentSettingsData,
            AssignmentData assignmentData)
        {
            _mainWindow = mainWindow;
            _assingmentSettingsData = assignmentSettingsData;
            _assignmentData = assignmentData;
        }

        public async Task PrintEmptyAsync(string path)
        {
            string copyPath = Path.Combine(Path.GetDirectoryName(path), "copy_" +
                Path.GetFileName(path));

            await Task.Run(() =>
            {
                File.Copy(path, copyPath, true);

                if (File.Exists(copyPath))
                {
                    Process.Start(new ProcessStartInfo(copyPath)
                    {
                        UseShellExecute = true
                    });
                }
            });
        }

        public async Task PrintAsync(string path)
        {
            InitializeTags();

            string copyPath;

            copyPath = Path.Combine(Path.GetDirectoryName(path), "copy_" +
                Path.GetFileName(path));

            await Task.Run(() =>
            {

            if (!File.Exists(path))
                throw new Exception($"Не удается найти файл {path}");

            File.Copy(path, copyPath, true);

            using (WordprocessingDocument doc = WordprocessingDocument
                .Open(copyPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                var bodyText = body.Descendants<Text>();

                foreach (Text text in bodyText)
                {
                    foreach (var key in PrintConfig.Tags.Keys)
                    {
                        var keyValue = PrintConfig.Tags[key];

                        Regex wordRegex = new Regex($"^\\s*{key}\\s*$", RegexOptions.IgnoreCase);

                        if (wordRegex.IsMatch(text.Text))
                        {
                            if (keyValue is string strValue)
                            {
                                var targetRegex = new Regex($"{key}", RegexOptions.IgnoreCase);

                                text.Text = targetRegex.Replace(text.Text, strValue);
                            }
                            else if (keyValue is DataTable table)
                            {
                                CreateDocumentTable(table, text);
                            }
                        }
                    }
                }
            }
            });

            if (File.Exists(copyPath))
            {
                Process.Start(new ProcessStartInfo(copyPath)
                {
                    UseShellExecute = true
                });
            }

        }
        void InitializeTags()
        {
            var tags = PrintConfig.Tags;

            if (tags.ContainsKey(PrintConfig.АдресФирмы))
            {
                tags[PrintConfig.АдресФирмы] = _assignmentData.CarrierSubjectAddressDetails.StreetName;
            }

            if (tags.ContainsKey(PrintConfig.Водитель))
            {
                tags[PrintConfig.Водитель] = _mainWindow.DriverTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.ДатаДоговора))
            {
                tags[PrintConfig.ДатаДоговора] =
                    _mainWindow.ContractDatePicker.SelectedDate?.Date.ToString("dd.MM.yyyy");
            }

            if (tags.ContainsKey(PrintConfig.Доверенность))
            {
                tags[PrintConfig.Доверенность] = _assingmentSettingsData.Comission;
            }

            if (tags.ContainsKey(PrintConfig.Должность))
            {
                tags[PrintConfig.Должность] = _assingmentSettingsData.LastPost;
            }

            if (tags.ContainsKey(PrintConfig.ДолжностьПеревозчика))
            {
                tags[PrintConfig.ДолжностьПеревозчика] = "ВОДИТЕЛЬ";
            }

            if (tags.ContainsKey(PrintConfig.Загрузка))
            {
                tags[PrintConfig.Загрузка] = _mainWindow.LoadingPlaceTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.Маршрут))
            {
                tags[PrintConfig.Маршрут] = _mainWindow.RouteTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.Место))
            {
                tags[PrintConfig.Место] = _mainWindow.AssignPlaceTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.Назначение))
            {
                tags[PrintConfig.Назначение] = _mainWindow.ConsigneeDataTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.НомерДоговора))
            {
                tags[PrintConfig.НомерДоговора] = _mainWindow.ContractNumberTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.НомерПоручения))
            {
                tags[PrintConfig.НомерПоручения] = _mainWindow.AssinmentNumberTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.Отправление))
            {
                tags[PrintConfig.Отправление] = _mainWindow.ConsignorDataTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.Получатель))
            {
                tags[PrintConfig.Получатель] = _mainWindow.LoadingPlaceTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.ПунктВвоза))
            {
                tags[PrintConfig.ПунктВвоза] = _mainWindow.ImportationTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.Разгрузка))
            {
                tags[PrintConfig.Разгрузка] = _mainWindow.UnloadingPlaceTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.ТекущаяДата))
            {
                tags[PrintConfig.ТекущаяДата] = _mainWindow.CurrentDatePicker.SelectedDate?.Date.ToString("dd.MM.yyyy");
            }

            if (tags.ContainsKey(PrintConfig.Товар))
            {
                var table = GetDataTable(_mainWindow.GoodsDetailsDataGrid);
                tags[PrintConfig.Товар] = table;
            }

            if (tags.ContainsKey(PrintConfig.Транспорт))
            {
                tags[PrintConfig.Транспорт] = _mainWindow.TruckTypesTextBox.Text;
            }

            if (tags.ContainsKey(PrintConfig.УНП))
            {
                tags[PrintConfig.УНП] = _assignmentData.CarrierSubjectAddressDetails.CityName;
            }

            if (tags.ContainsKey(PrintConfig.Фирма))
            {
                tags[PrintConfig.Фирма] = _assignmentData.CarrierSubjectAddressDetails.SubjectBriefName;
            }

            if (tags.ContainsKey(PrintConfig.ФИО))
            {
                tags[PrintConfig.ФИО] = _assingmentSettingsData.LastFullName;
            }

            if (tags.ContainsKey(PrintConfig.ФИОПеревозчика))
            {
                tags[PrintConfig.ФИОПеревозчика] = _assignmentData.DriverDetails.FirstName + " " + _assignmentData.DriverDetails.LastName;
            }

            if(tags.ContainsKey(PrintConfig.Доверенность))
            {
                tags[PrintConfig.Доверенность] = _assingmentSettingsData.TrustNumber;
            }   
        }

        public DataTable GetDataTable(DataGrid dataGrid)
        {
            DataTable dataTable = new DataTable();

            // 1. Создаем колонки в DataTable (на основе заголовков DataGrid)
            foreach (DataGridColumn column in dataGrid.Columns)
            {
                // Проверяем, является ли колонка DataGridTextColumn (чтобы избежать ошибок)
                if (column is DataGridTextColumn textColumn)
                {
                    // Получаем имя свойства из Binding (например, "Name", "Price")
                    string bindingPath = (textColumn.Binding as Binding)?.Path.Path;
                    if (!string.IsNullOrEmpty(bindingPath))
                    {
                        // Добавляем колонку с именем свойства и типом string (или другим)
                        dataTable.Columns.Add(bindingPath, typeof(string));
                    }
                }
            }

            // 2. Получаем данные из ItemsSource (BindingList<GoodsDetailData>)
            var items = dataGrid.ItemsSource as BindingList<GoodsDetailData>;
            if (items == null)
            {
                return dataTable; // Если данных нет, возвращаем пустую таблицу
            }

            // 3. Заполняем DataTable данными
            foreach (var goods in items)
            {
                DataRow row = dataTable.NewRow();

                // Проходим по всем колонкам DataTable и заполняем значения
                foreach (DataColumn column in dataTable.Columns)
                {
                    // Получаем значение свойства из goods (например, goods.Name, goods.Price)
                    var propertyValue = goods.GetType().GetProperty(column.ColumnName)?.GetValue(goods);
                    row[column.ColumnName] = propertyValue?.ToString(); // Записываем значение или DBNull
                }

                dataTable.Rows.Add(row);
            }

            //Переименовываем столбцы, как в программме
            foreach (DataColumn column in dataTable.Columns)
            {
                int columnIndex = dataTable.Columns.IndexOf(column);

                if (dataGrid.Columns[columnIndex] is DataGridTextColumn textColumn)
                {
                    column.ColumnName = textColumn.Header.ToString();
                }
            }


            return dataTable;
        }


        void CreateDocumentTable(DataTable dataTable, Text text)
        {
            Run parentRun = text.Parent as Run;

            var paragrph = parentRun?.Parent as Paragraph;

            if (paragrph == null)
            {
                return;
            }
            Table table = new Table();

            TableProperties tableProps = new TableProperties(
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new TableLayout() { Type = TableLayoutValues.Autofit },
                new TableWidth() { Width = "8000", Type = TableWidthUnitValues.Dxa }
            );

            tableProps.AppendChild(new TableLook() { Val = "04A0" });


            table.AppendChild(tableProps);


            TableRow headerRow = new TableRow();

            foreach (DataColumn column in dataTable.Columns)
            {
                headerRow.Append(CreateTableCell(column.ColumnName, true, "18"));
            }

            table.Append(headerRow);

            foreach (DataRow row in dataTable.Rows)
            {
                TableRow dataRow = new TableRow();

                foreach (var item in row.ItemArray)
                {
                    dataRow.Append(CreateTableCell(item.ToString(), false, "18"));
                }
                table.Append(dataRow);
            }


            paragrph.Parent.InsertAfter(table, paragrph);

            paragrph.Remove();

        }

        TableCell CreateTableCell(string text, bool isHeader, string fontSizeValue)
        {
            TableCell cell = new TableCell();

            // Настройка ячейки
            TableCellProperties cellProps = new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2500" },
                new Shading() { Fill = "auto" },
                new WordWrap()
            );

            Paragraph paragraph = new Paragraph();
            Run run = new Run();


            run.Append(new RunProperties(
                  new Bold(),
                  new FontSize() { Val = fontSizeValue }
              ));

            run.Append(new Text(text));
            paragraph.Append(run);
            cell.Append(paragraph);

            return cell;
        }
    }
}
