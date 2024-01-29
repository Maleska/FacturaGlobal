using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGlobal.Components.List
{
    internal class Invoices
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string NumAtCard { get; set; }
        public string Comments { get; set; }
        public string Series { get; set; }
        public string DocTotal { get; set; }
        public string GroupNum { get; set; }
        public string PeyMethod { get; set; }
        public string U_B1SYS_MainUsage { get; set; }
        //public string U_MetodoPago { get; set; }
        public string U_B1SYS_CFDiPeriod { get; set; }
        public string U_B1SYS_CFDiMonths { get; set; }
        public string U_B1SYS_CFDiYear { get; set; }
        public string U_UN { get; set; }
        public List<DocumentLines> DocumentLines { get; set; }


    }
    internal class DocumentLines
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }

    }
}
