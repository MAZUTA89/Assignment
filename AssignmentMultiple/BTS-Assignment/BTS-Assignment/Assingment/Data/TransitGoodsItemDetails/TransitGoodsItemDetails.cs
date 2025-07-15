using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Assignment.Data
{
    public class TransitGoodsItemDetails
    {
        public string CurrencyCode { get; set; }
        public string TotalAmount { get; set; }
        public string CommonCargoQuantity { get; set; }
        public string CommonMassMeasure
        {
            get
            {
                float mass = 0;

                foreach (var transitDescription in TransitCodeDescriptions)
                {
                    mass += transitDescription.UnifiedGrossMassMeasure;
                }

                return mass.ToString();
            }
        }

        public List<TransitCodeDescription> TransitCodeDescriptions { get; set; }

        public TransitGoodsItemDetails()
        {
            TransitCodeDescriptions = new List<TransitCodeDescription>();
        }
    }
}
