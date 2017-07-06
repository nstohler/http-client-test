using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ApsNetCoreConsole
{
    public class AdUserInfo
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SamAccountName { get; set; }
        public string Mail { get; set; }
    }
}
