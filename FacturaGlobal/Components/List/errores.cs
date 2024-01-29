using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGlobal.Components.List
{
    internal class errores
    {

        public valores error { get; set; }
    }
    internal class valores
    {
        public string code { get; set; }
        public message message { get; set; }
    }


    internal class message
    {
        public string lang { get; set; }
        public string value { get; set; }
    }
}
