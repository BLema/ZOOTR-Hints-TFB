using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOOTR_Hints
{
    class Check
    {
        public string Location { get; set; }
        
        public string Has { get; set; }

        // constructor
        public Check()
        {
            Location = string.Empty;
            Has = string.Empty;
        }

        public Check(string location, string has)
        {
            Location = location;
            Has = has;
        }
    }
}
