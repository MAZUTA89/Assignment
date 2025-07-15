using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS_Assignment.Settings
{
    public interface IAssignmentSettings
    {
        void Save();
        
        void Load();
    }
}
