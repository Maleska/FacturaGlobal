using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGlobal.Components.List
{
    internal class OV
    {
        public string DocEntry { get; set; }    
        public string DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string DocTotal { get; set; }    
        public List<Details> Details { get; set; }
        public string U_UN { get; set; }
        
    }
    internal class Details
    {
        public string ItemCode { get; set; }
        public string ItemName { get;set; }
        public string Quantity { get; set; }
        public string Discount { get; set; }
        public string Price { get; set; }
        public string LineTotal { get; set; }

    }
}
