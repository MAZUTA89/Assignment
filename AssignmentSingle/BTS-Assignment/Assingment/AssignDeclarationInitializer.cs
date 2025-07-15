using BTS_Assignment.Data;
using BTS_Assignment.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace BTS_Assignment
{
    public class AssignDeclarationInitializer
    {

        public const string c_CodeColumnName = "Код ТНВЭД";
        public const string c_CargoHeaderColumnName = "Нименование груза";
        public const string c_CargoCostColumnName = "Стоимость груза";
        public const string c_PlacesAmountColumnName = "Количество мест";
        public const string c_BruttoWeightColumnName = "Вес брутто";

        public BindingList<GoodsDetailData> GoodsDetails { get; private set; }

        TransitDateParser _dateParser;

        CountryService _countryService;
        AssignmentData _assignmentData;
        AssignmentSettingsData _assignmentSettingsData;
        MainWindow _mainWindow;

        public bool _isInitialized;
        public AssignDeclarationInitializer(
            MainWindow mainWindow, CountryService countryService)
        {
            _mainWindow = mainWindow;
            _countryService = countryService;
            _dateParser = new TransitDateParser();
            GoodsDetails = new BindingList<GoodsDetailData>();
            _mainWindow.GoodsDetailsDataGrid.ItemsSource = GoodsDetails;
        }

        public void Initialize(AssignmentData assignmentData,
            AssignmentSettingsData assignmentSettingsData)
        {

           _assignmentSettingsData = assignmentSettingsData;

            if (assignmentData != null)
            {
                _assignmentData = assignmentData;

                _mainWindow.LoadingPlaceTextBox.Text =
                    _countryService[_assignmentData.Route.Sent];

                InitTruckTypes();
                InitRoute();
                InitDriverInfo();
                InitTransitGoodsDetails();
                InitDocumentData();

                _mainWindow.ConsignorDataTextBox.Text = _countryService[_assignmentData.Route.Sent];
                _mainWindow.ConsigneeDataTextBox.Text = _assignmentData.CustomsOfficeDetails.OfficeCode.Substring(3) + ", " +
                _assignmentData.CustomsOfficeDetails.OfficeName;

                _mainWindow.LoadHolderTextBox.Text = _assignmentData.ConsigneeBriefName;

                _mainWindow.UnloadingPlaceTextBox.Text = _assignmentData.ConsigneeSubjectAddressDetails.CityName + ", " +
                    _assignmentData.ConsigneeSubjectAddressDetails.StreetName;
                

                _isInitialized = true;
            }
        }

        void InitDocumentData()
        {
            if (_dateParser.TryParse(_assignmentData.DocumentData.TransitDate, out DateTime transitDate))
            {
                _mainWindow.ContractDatePicker.SelectedDate = transitDate;
            }
            else
            {
                MessageBox.Show("Не удалось считать дату контракта.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (_dateParser.TryParse(_assignmentData.DocumentData.TransitCreationDate, out DateTime creationDate))
            {
                _mainWindow.CurrentDatePicker.SelectedDate = creationDate;
            }
            else
            {
                _mainWindow.CurrentDatePicker.SelectedDate = DateTime.Now;

                MessageBox.Show("Не удалось считать дату документа.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _mainWindow.ContractNumberTextBox.Text = _assignmentData.DocumentData.TransitNumber;
        }

        void InitTruckTypes()
        {
            StringBuilder truckBuilder = new StringBuilder();

            for (int i = 0; i < _assignmentData.TransportMeansItemDetails.Count; i++)
            {
                var truckData = _assignmentData.TransportMeansItemDetails[i];

                if (i == _assignmentData.TransportMeansItemDetails.Count - 1)
                {
                    truckBuilder.Append(truckData.TransportMeansRegId);
                }
                else
                {
                    truckBuilder.Append(truckData.TransportMeansRegId + "/");
                }
            }

            _mainWindow.TruckTypesTextBox.Text = truckBuilder.ToString();
        }
        void InitRoute()
        {
            StringBuilder routeBuilder = new StringBuilder();

            routeBuilder.Append(_countryService[_assignmentData.Route.Sent]);
            routeBuilder.Append("-");
            routeBuilder.Append(_countryService[_assignmentData.Route.Assigned]);

            _mainWindow.RouteTextBox.Text = routeBuilder.ToString();
        }

        void InitDriverInfo()
        {
            _mainWindow.DriverTextBox.Text = _assignmentData.DriverDetails.FirstName + " " +
                _assignmentData.DriverDetails.LastName;
        }

        void InitTransitGoodsDetails()
        {
            GoodsDetails.Clear();

            InitializeTransitTable();

            _mainWindow.GoodsDetailsDataGrid.ItemsSource = GoodsDetails;
        }
        void InitializeTransitTable()
        {
            //Закинуть данные о товарах в одну строку таблицы
            //InitInOneRow();
            //Закинуть данные о товарах отдльно по каждому
            InitRows();
        }

        void InitRows()
        {
            var codeDescripts = _assignmentData.TransitGoodsItemDetails.TransitCodeDescriptions;

            foreach (var detail in codeDescripts)
            {
                GoodsDetailData data = new GoodsDetailData();

                data.CargoCost = detail.ValueAmount;
                data.CargoCost += " " + _assignmentData.TransitGoodsItemDetails.CurrencyCode;
                data.CargoHeader = detail.GoodsDescription;
                data.BruttoWeight = detail.UnifiedGrossMassMeasure.ToString();
                data.Code = detail.Code;
                data.PlacesAmount = detail.CargoQuantity.ToString();
                
                GoodsDetails.Add(data);
            }

            if (GoodsDetails.Count == 1 && !_assignmentSettingsData.IsAddCommonRow)
            {
                return;
            }


            GoodsDetailData lastRowDetails = new GoodsDetailData();

            lastRowDetails.BruttoWeight = _assignmentData.TransitGoodsItemDetails.CommonMassMeasure.ToString();
            lastRowDetails.CargoCost = _assignmentData.TransitGoodsItemDetails.TotalAmount.ToString() + " " +
                _assignmentData.TransitGoodsItemDetails.CurrencyCode;
            lastRowDetails.PlacesAmount = _assignmentData.TransitGoodsItemDetails.CommonCargoQuantity.ToString();
            lastRowDetails.Code = "Всего:";

            GoodsDetails.Add(lastRowDetails);
            
        }

        void InitInOneRow()
        {
            GoodsDetailData goodsDetailData = new GoodsDetailData();

            StringBuilder codesRowValue = new StringBuilder();
            StringBuilder cargoNameBuilder = new StringBuilder();

            string costValue = _assignmentData.TransitGoodsItemDetails.TotalAmount;
            costValue += " " + _assignmentData.TransitGoodsItemDetails.CurrencyCode;

            string cargoPlaces = _assignmentData.TransitGoodsItemDetails.CommonCargoQuantity;
            string bruttoWeight = _assignmentData.TransitGoodsItemDetails.CommonMassMeasure;

            var codeDescriptions = _assignmentData.TransitGoodsItemDetails.TransitCodeDescriptions;

            codesRowValue.Append(codeDescriptions[0].Code);

            for (int i = 1; i < codeDescriptions.Count; i++)
            {
                codesRowValue.Append(", ").Append(codeDescriptions[i].Code);
            }

            HashSet<string> uniqueCargoHeaderSet = new HashSet<string>();

            foreach (var descr in codeDescriptions)
            {
                uniqueCargoHeaderSet.Add(descr.GoodsDescription);
            }

            foreach (var cargoName in uniqueCargoHeaderSet)
            {
                cargoNameBuilder.Append(cargoName).Append("\n");
            }

            goodsDetailData.Code = codesRowValue.ToString();
            goodsDetailData.CargoHeader = cargoNameBuilder.ToString();
            goodsDetailData.CargoCost = costValue;
            goodsDetailData.PlacesAmount = cargoPlaces;
            goodsDetailData.BruttoWeight = bruttoWeight;

            GoodsDetails.Add(goodsDetailData);
        }
    }
}
