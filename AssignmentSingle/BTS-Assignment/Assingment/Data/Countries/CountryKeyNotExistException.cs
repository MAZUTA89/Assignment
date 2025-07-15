using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Assignment.Data
{
    public class CountryKeyNotExistException : Exception
    {
        public CountryKeyNotExistException(): base("Полное имя страны по коду не найдено")
        {
           
        }
    }
}
