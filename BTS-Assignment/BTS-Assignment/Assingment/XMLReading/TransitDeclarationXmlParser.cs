using BTS_Assignment.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Globalization;

namespace BTS_Assignment.XMLReading
{
    public class TransitDeclarationXmlParser : IXMLParser
    {
        public AssignmentData AssignmentData { get; private set; }
        protected TrucksData TrucksData;
        protected XmlReaderSettings XmlSettings;
        protected XmlNamespaceManager NamespaceManager;
        protected CountryService CountryService;
        public TransitDeclarationXmlParser(TrucksData trucksData,
             CountryService countryService)
        {
            TrucksData = trucksData;

            CountryService = countryService;

            XmlSettings = new XmlReaderSettings()
            {
                Async = true,
                IgnoreWhitespace = true
            };

        }
        public async Task<AssignmentData> ParseXmlAsync(string path)
        {
            XmlElement root = null;
            using (var fr = File.OpenRead(path))
            using (XmlReader xmlReader = XmlReader.Create(fr, XmlSettings))
            {
                XmlDocument xDoc = new XmlDocument();

                await Task.Run(() => { xDoc.Load(xmlReader); });

                root = xDoc.DocumentElement;

                NamespaceManager = new XmlNamespaceManager(xDoc.NameTable);
            }

            NamespaceManager.AddNamespace("csdo", "urn:EEC:M:SimpleDataObjects:v0.4.14");
            NamespaceManager.AddNamespace("casdo", "urn:EEC:M:CA:SimpleDataObjects:v1.8.1");
            NamespaceManager.AddNamespace("cacdo", "urn:EEC:M:CA:ComplexDataObjects:v1.8.1");
            NamespaceManager.AddNamespace("ccdo", "urn:EEC:M:ComplexDataObjects:v0.4.14");


            AssignmentData = new AssignmentData();

            if (root != null)
            {
                var nodes = root.ChildNodes;

                foreach (XmlNode node in nodes)
                {
                    switch (node.Name)
                    {
                        case "cacdo:TDGoodsShipmentDetails":
                            {
                                HandleTDGoodsShipmentDetailsNode(node);
                                break;
                            }
                        case "cacdo:TransportMeansItemDetails":
                            {
                                HandleTransportMeansDetailsNode(node);
                                break;
                            }
                        case "cacdo:CarrierDetails":
                            {
                                HandleCarrierDetailsNode(node);
                                break;
                            }
                        case "cacdo:SignatoryRepresentativeDetails":
                            {
                                HandleSignatoryRepresentativeDetailsNode(node);
                                break;
                            }
                        case "csdo:EDocDateTime":
                            {
                                AssignmentData.DocumentData.TransitCreationDate = node.InnerText;
                                break;
                            }
                    }
                }
            }

            return AssignmentData;
        }

        void InitializeSubjectAddressByNode(XmlNode consignDetailNode,
            SubjectAddressDetails subjectAddressDetails)
        {
            foreach (XmlNode detailNode in consignDetailNode.ChildNodes)
            {
                if(detailNode.Name == "ccdo:SubjectAddressDetails")
                {
                    foreach (XmlNode subjectsAddress in detailNode.ChildNodes)
                    {
                        switch (subjectsAddress.Name)
                        {
                            case "csdo:UnifiedCountryCode":
                                {
                                    subjectAddressDetails.UnifiedCountryCode = subjectsAddress.InnerText;
                                    break;
                                }
                            case "csdo:CityName":
                                {
                                    subjectAddressDetails.CityName = subjectsAddress.InnerText;
                                    break;
                                }
                            case "csdo:StreetName":
                                {
                                    subjectAddressDetails.StreetName = subjectsAddress.InnerText;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
        }
        void HandleSignatoryRepresentativeDetailsNode(XmlNode signatoryDetailsNode)
        {
            foreach (XmlNode node in signatoryDetailsNode)
            {
                if(node.Name == "cacdo:RepresentativeContractDetails")
                {
                    XmlNode docIdNode = node.SelectSingleNode("csdo:DocId", NamespaceManager);
                    XmlNode creationDateNode = node.SelectSingleNode("csdo:DocCreationDate", NamespaceManager);

                    AssignmentData.DocumentData.TransitDate = creationDateNode.InnerText;
                    AssignmentData.DocumentData.TransitNumber = docIdNode.InnerText;
                }
            }
        }
        void HandleTDGoodsShipmentDetailsNode(XmlNode node)
        {
            foreach (XmlNode goodsNode in node.ChildNodes)
            {
                if (goodsNode.Name == "cacdo:ConsignorDetails")
                {
                    var consignorDetails = AssignmentData.ConsignorSubjectAddressDetails;

                    InitializeSubjectAddressByNode(goodsNode, consignorDetails);
                }
                if(goodsNode.Name == "cacdo:ConsigneeDetails")
                {
                    var consigneeDetails = AssignmentData.ConsigneeSubjectAddressDetails;

                    InitializeSubjectAddressByNode(goodsNode, consigneeDetails);

                    XmlNode briefNode = goodsNode.SelectSingleNode("csdo:SubjectBriefName", NamespaceManager);
                    if(briefNode != null)
                    {
                        AssignmentData.ConsigneeBriefName = briefNode.InnerText;
                    }
                }

                if (goodsNode.Name == "casdo:DepartureCountryCode")
                {
                    AssignmentData.Route.Sent = goodsNode.InnerText;
                }
                if (goodsNode.Name == "casdo:DestinationCountryCode")
                {
                    AssignmentData.Route.Assigned = goodsNode.InnerText;
                }

                if (goodsNode.Name == "casdo:CargoQuantity")
                {
                    AssignmentData.TransitGoodsItemDetails.CommonCargoQuantity = goodsNode.InnerText;
                }

                if(goodsNode.Name == "casdo:TotalAmount")
                {
                    AssignmentData.TransitGoodsItemDetails.TotalAmount = goodsNode.InnerText;

                    foreach (XmlAttribute amountAttr in goodsNode.Attributes)
                    {
                        if(amountAttr.Name == "currencyCode")
                        {
                            AssignmentData.TransitGoodsItemDetails.CurrencyCode = amountAttr.InnerText;
                            break;
                        }
                    }
                }

                if (goodsNode.Name == "cacdo:TransitGoodsItemDetails")
                {
                    TransitCodeDescription itemDescr = new TransitCodeDescription();

                    foreach (XmlNode itemNode in goodsNode.ChildNodes)
                    {
                        if (itemNode.Name == "csdo:CommodityCode")
                        {
                            string text = itemNode.InnerText;

                            itemDescr.Code = text; /*text.Substring(0, text.Length - 4);*/
                        }
                        if (itemNode.Name == "casdo:GoodsDescriptionText")
                        {
                            itemDescr.GoodsDescription = itemNode.InnerText;
                        }
                        if (itemNode.Name == "csdo:UnifiedGrossMassMeasure")
                        {
                            if (float.TryParse(itemNode.InnerText, NumberStyles.Float,
                                CultureInfo.InvariantCulture,  out float mass))
                            {
                                itemDescr.UnifiedGrossMassMeasure = mass;
                            }
                            else
                            {
                                itemDescr.UnifiedGrossMassMeasure = 0;
                                throw new Exception("Can't parse to float [UnifiedGrossMassMeasure] node.");
                            }
                        }

                        if(itemNode.Name == "casdo:CAValueAmount")
                        {
                            if(float.TryParse(itemNode.InnerText, NumberStyles.Float,
                                CultureInfo.InvariantCulture, out float value))
                            {
                                itemDescr.ValueAmount = itemNode.InnerText;
                            }
                            else
                            {
                                throw new Exception("Can't parse to float [CAValueAmount] node.");
                            }
                        }    
                        if(itemNode.Name == "cacdo:CargoPackagePalletDetails")
                        {
                            XmlNode packageNode = itemNode.SelectSingleNode("casdo:CargoQuantity", NamespaceManager);

                            if(int.TryParse(packageNode.InnerText, out int quantity))
                            {
                                itemDescr.CargoQuantity = quantity;
                            }
                            else
                            {
                                throw new Exception($"Can't parse to int [CargoQuantity] node.");
                            }
                                
                        }
                    }

                    AssignmentData.TransitGoodsItemDetails.TransitCodeDescriptions.Add(itemDescr);
                }

                if(goodsNode.Name == "cacdo:TransitDestinationDetails")
                {
                    XmlNode officeDetailsNode = goodsNode.SelectSingleNode("ccdo:CustomsOfficeDetails", NamespaceManager);
                    
                    if(officeDetailsNode != null)
                    {
                        AssignmentData.CustomsOfficeDetails.OfficeCode =
                            officeDetailsNode.SelectSingleNode("csdo:CustomsOfficeCode", NamespaceManager).InnerText;

                        AssignmentData.CustomsOfficeDetails.OfficeName =
                            officeDetailsNode.SelectSingleNode("csdo:CustomsOfficeName", NamespaceManager).InnerText;
                    }
                }

                if(goodsNode.Name == "cacdo:TDPresentedDocDetails")
                {
                    XmlNode docsCreationNode = goodsNode.SelectSingleNode("csdo:DocCreationDate");
                    AssignmentData.DocumentData.TransitCreationDate = docsCreationNode.InnerText;
                }

            }
        }
        void HandleTransportMeansDetailsNode(XmlNode node)
        {
            TransportMeansItemDetails tramsportDetails =
                new TransportMeansItemDetails();

            foreach (XmlNode detailNode in node.ChildNodes)
            {
                switch (detailNode.Name)
                {
                    case "casdo:TransportTypeCode":
                        {
                            if (int.TryParse(detailNode.InnerText,
                                out int code))
                            {
                                string typeName = TrucksData[code, DataType.Kind];

                                tramsportDetails.TransportTypeCode = typeName;
                            }
                            else
                            {
                                throw new Exception("Can't parse to int [TransportTypeCode] node.");
                            }
                            break;
                        }
                    case "csdo:VehicleMakeCode":
                        {
                            if (int.TryParse(detailNode.InnerText,
                                out int code))
                            {
                                string markName = TrucksData[code, DataType.Marka];

                                tramsportDetails.VehicleMakeCode = markName;
                            }
                            else
                            {
                                throw new Exception("Can't parse to int [VehicleMakeCode] node.");
                            }
                            break;
                        }
                    case "csdo:TransportMeansRegId":
                        {
                            tramsportDetails.TransportMeansRegId = detailNode.InnerText;
                            break;
                        }
                    case "csdo:VehicleId":
                        {
                            tramsportDetails.VehicleId = detailNode.InnerText;
                            break;
                        }
                }

            }
                AssignmentData.TransportMeansItemDetails.Add(tramsportDetails);
        }
       
        void HandleCarrierDetailsNode(XmlNode node)
        {
            foreach (XmlNode carrierDetail in node.ChildNodes)
            {
                if (carrierDetail.Name == "cacdo:CarrierRepresentativeDetails")
                {
                    foreach (XmlNode representDetail in carrierDetail.ChildNodes)
                    {
                        if (representDetail.Name == "ccdo:FullNameDetails")
                        {
                            foreach (XmlNode fullNameNode in representDetail.ChildNodes)
                            {
                                switch (fullNameNode.Name)
                                {
                                    case "csdo:FirstName":
                                        {
                                            AssignmentData.DriverDetails.FirstName = fullNameNode.InnerText;
                                            break;
                                        }
                                    case "csdo:LastName":
                                        {
                                            AssignmentData.DriverDetails.LastName = fullNameNode.InnerText;
                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
            }

            XmlNode carrierAddressDetailsNode = node.SelectSingleNode("ccdo:SubjectAddressDetails", NamespaceManager);

            if(carrierAddressDetailsNode != null)
            {
                InitializeSubjectAddressByNode(node,
                    AssignmentData.CarrierSubjectAddressDetails);
            }

            XmlNode carrierBriefNameNode = node.SelectSingleNode("csdo:SubjectBriefName", NamespaceManager);

            if(carrierBriefNameNode != null)
                AssignmentData.CarrierSubjectAddressDetails.SubjectBriefName =
                    carrierBriefNameNode.InnerText;
        }
    }
}
