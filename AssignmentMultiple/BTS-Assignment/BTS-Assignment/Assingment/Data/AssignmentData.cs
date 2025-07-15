using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Assignment.Data
{
    public class AssignmentData
    {
        public SubjectAddressDetails ConsignorSubjectAddressDetails { get; set; }
        public SubjectAddressDetails ConsigneeSubjectAddressDetails { get; set; }
        public List<TransportMeansItemDetails> TransportMeansItemDetails { get; set; }
        public Route Route { get; set; }
        public DriverDetails DriverDetails { get; set; }
        public TransitGoodsItemDetails TransitGoodsItemDetails { get; set; }
        public string ConsigneeBriefName { get; set; }
        public CustomsOfficeDetails CustomsOfficeDetails { get; set; }
        public CarrierSubjectAddressDetails CarrierSubjectAddressDetails { get; set; }
        public Document DocumentData { get; set; }

        public AssignmentData()
        {
            ConsignorSubjectAddressDetails = new SubjectAddressDetails();
            ConsigneeSubjectAddressDetails = new SubjectAddressDetails();
            TransportMeansItemDetails = new List<TransportMeansItemDetails>();
            Route = new Route();
            DriverDetails = new DriverDetails();
            TransitGoodsItemDetails = new TransitGoodsItemDetails();
            CustomsOfficeDetails = new CustomsOfficeDetails();
            CarrierSubjectAddressDetails = new CarrierSubjectAddressDetails();
            DocumentData = new Document();
        }
    }
}
