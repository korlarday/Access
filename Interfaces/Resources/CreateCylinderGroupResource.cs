using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateCylinderGroupResource
    {
        public int _CustomerID { get; set; }
        public int _GroupID { get; set; }
        public int[] _CylinderIDs { get; set; }
    }

    public class ModifyCylinderGroup
    {
        public int _CustomerID { get; set; }
        public int _GroupId { get; set; }
        public int[] _CylinderPosNumbers { get; set; }
    }
    public class CreateCylinderGroupVerification
    {
        public int _CustomerID { get; set; }
        public int _GroupID { get; set; }
        public int[] _CylinderIDs { get; set; }
    }



    public class ReadCylinderGroupResource
    {
        public int _CylinderID { get; set; }
        public int _GroupID { get; set; }
        public int CustomerID { get; set; }

        public string _Customer { get; set; }
        public string _DoorName { get; set; }
        public string _KeyName { get; set; }
    }
}
