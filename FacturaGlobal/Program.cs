using FacturaGlobal.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGlobal
{
    internal class Program
    {
        #region Variables
            /// <summary>
            /// Variables de SAP
            /// </summary>
            /// <param name="args"></param>
               public static List<Components.List.OV> listaOV = new List<Components.List.OV>();
        #endregion
        static void Main(string[] args)
        {

            //Se hace una busqueda de las OV 
            //listaOV = Components.DL.Functions.getListOV();
           string month = DateTime.Now.Month.ToString("00");

            ConfigureService.Configure();

        }
        //public void Start()
        //{
        //    listaOV = Components.DL.Functions.getListOV();
        //}
        //public void Stop()
        //{

        //}
    }
}
