using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS_Assignment.Data;

namespace BTS_Assignment.XMLReading
{
    public interface IXMLParser
    {
        Task<AssignmentData> ParseXmlAsync(string path);
    }
}
