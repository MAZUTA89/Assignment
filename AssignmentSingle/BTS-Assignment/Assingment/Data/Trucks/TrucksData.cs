using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace BTS_Assignment.Data
{
    public class TrucksData
    {
        public const string c_TrucksPath = @"Resources\trucks.xml";
        const string c_kindNodeName = "kind";
        const string c_kindCodeChildNodeName = "kcode";
        const string c_kindNameChildNodeName = "kname";
        const string c_markNodeName = "marka";
        const string c_markCodeChildNodeName = "code";
        const string c_markNameChildNodeName = "name";

        public IEnumerable<TruckData> TruckTypes => _kindTrucks;
        public IEnumerable<TruckData> MarkTypes => _markTypes;

        private List<TruckData> _kindTrucks;
        private List<TruckData> _markTypes;

        XmlReaderSettings _xmlReaderSettings;

        public bool IsInitialized { get; private set; }

        public TrucksData()
        {
            _kindTrucks = new List<TruckData>();
            _markTypes = new List<TruckData>();

            _xmlReaderSettings = new XmlReaderSettings()
            {
                Async = true,
                IgnoreWhitespace = true
            };

        }
        public async Task<bool> TryInitializeAsync()
        {
            XmlElement root;

            string trucksPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetDirectoryName(c_TrucksPath),
                Path.GetFileName(c_TrucksPath));

            if (!File.Exists(trucksPath))
            {
                IsInitialized = false;
                throw new Exception($"Файл {Path.GetFileName(c_TrucksPath)} не найден в {Path.GetDirectoryName(trucksPath)}");
            }

            using (Stream fs = File.OpenRead(c_TrucksPath))
            using (XmlReader xmlReader = XmlReader.Create(fs, _xmlReaderSettings))
            {
                XmlDocument doc = new XmlDocument();

                await Task.Run(() =>
                {
                    doc.Load(xmlReader);
                });

                root = doc.DocumentElement;
            }

            if (root != null)
            {
                var rootNodes = root.ChildNodes;

                foreach (XmlNode node in rootNodes)
                {
                    switch (node.Name)
                    {
                        case c_kindNodeName:
                            {
                                TruckData kindData = new TruckData();

                                foreach (XmlNode childNode in node.ChildNodes)
                                {
                                    switch (childNode.Name)
                                    {
                                        case c_kindCodeChildNodeName:
                                            {
                                                if (!int.TryParse(childNode.InnerText, out kindData.Code))
                                                {
                                                    IsInitialized = false;

                                                    throw new Exception($"Не удалось конвертировать код транспорта.\n" +
                                                        $"Path:{c_TrucksPath}");
                                                }
                                                break;
                                            }
                                        case c_kindNameChildNodeName:
                                            {
                                                kindData.Name = childNode.InnerText;
                                                break;
                                            }
                                        default:
                                            continue;
                                    }
                                }

                                _kindTrucks.Add(kindData);

                                break;
                            }
                        case c_markNodeName:
                            {
                                TruckData markData = new TruckData();

                                foreach (XmlNode childNode in node.ChildNodes)
                                {
                                    switch (childNode.Name)
                                    {
                                        case c_markCodeChildNodeName:
                                            {
                                                if (!int.TryParse(childNode.InnerText, out markData.Code))
                                                {
                                                    IsInitialized = false;

                                                    throw new Exception($"Не удалось конвертировать код транспорта.\n" +
                                                        $"Path:{c_TrucksPath}");
                                                }
                                                break;
                                            }
                                        case c_markNameChildNodeName:
                                            {
                                                markData.Name = childNode.InnerText;
                                                break;
                                            }
                                        default:
                                            {
                                                continue;
                                            }
                                    }
                                }

                                _markTypes.Add(markData);

                                break;
                            }
                        default:
                            {
                                continue;
                            }
                    }
                }

                if (_kindTrucks.Count > 0 && _markTypes.Count > 0)
                    IsInitialized = true;
                else
                    IsInitialized = false;

                return IsInitialized;
            }
            else
            {
                IsInitialized = false;

                throw new Exception($"Не удалось считать файл транспорта.\n" +
                    $"Path:{c_TrucksPath}");
            }
        }

        public string this[int code, DataType dataType]
        {
            get
            {
                if (!IsInitialized)
                {
                    return code.ToString();
                }
                else
                {
                    TruckData truckData = null;

                    switch (dataType)
                    {
                        case DataType.Kind:
                            {
                                truckData = TruckTypes.FirstOrDefault(t => t.Code == code);
                                return truckData.Name;
                            }
                        case DataType.Marka:
                            {
                                truckData = MarkTypes.FirstOrDefault(t => t.Code == code);
                                return truckData.Name;
                            }
                        default:
                            {
                                throw new Exception("Unexpected data type while reading trucks data.");
                            }
                    }
                }
            }
        }
    }

    public enum DataType
    {
        Kind,
        Marka
    }
}
