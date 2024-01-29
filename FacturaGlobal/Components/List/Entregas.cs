using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGlobal.Components.List
{
    internal class Entregas
    {
        //public string DocEntry { get; set; }
        //public string DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        //public string DocTotal { get; set; }
        public string GroupNum { get; set; }
        public string PaymentMethod { get; set; }
        public string U_UN { get; set; }
        public string U_B1SYS_MainUsage { get; set; }
        public string U_IL_Periodicidad { get; set; }
        public string U_IL_Meses { get; set; }
        public string U_IL_Anio { get; set; }
        public string Comments { get; set; }
        public string LicTradNum { get; set; }
        public string U_B1SYS_CFDiYear { get; set; }
        //public ElectronicProtocols electronicProtocols { get; set; }
        public string EDocGenerationType { get; set; } = "G";

        /// <summary>
        /// Campos Interlatin - DIXUP
        /// </summary>
        public string U_IL_Timbrar { get; set; }

        public List<DetailsEntrega> DocumentLines { get; set; }
    }
    internal class ElectronicProtocols
    {
        //public string ProtocolCode { get; set; }
        public string GenerationType { get; set; }
    }
    internal class DetailsEntrega
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public string DiscountPercent { get; set; }
        public string UnitPrice { get; set; }
        //public string LineTotal { get; set; }
        public string BaseEntry { get; set; }
        public string BaseType { get; set; } = "15";
        public string WarehouseCode { get; set; }
        public string TaxCode { get; set; }
        public string BaseLine { get; set; }

    }
}
