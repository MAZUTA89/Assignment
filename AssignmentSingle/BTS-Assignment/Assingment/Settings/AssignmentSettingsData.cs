using RegistryHelper;
using System.Collections.Generic;

namespace BTS_Assignment.Settings
{
    public class AssignmentSettingsData : IRegistryData
    {
        public string AssinmentNumber;
        public string AssinmentDate;
        public string ContractNumber;
        public string AssignmentPlace;
        public string LastFullName;
        public string LastPost;
        public string Comission;
        public string TrustNumber;
        public bool IsAddCommonRow;
        public bool IsRuCountriesCodePriority;

        public AssignmentSettingsData()
        {
           
        }
    }
}
