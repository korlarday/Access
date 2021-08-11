using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class PageVisited
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public int PageIndex { get; set; }
        public string PageName { get; set; }
        public DateTime DateVisited { get; set; }
    }
}
