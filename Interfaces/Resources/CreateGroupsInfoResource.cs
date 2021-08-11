using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateGroupsInfoResource
    {
        public int _Slot { get; set; }
        public int _Row { get; set; }
        public int _Value { get; set; }
        public int CustomerID { get; set; }
        public int GroupID { get; set; }
    }
}
