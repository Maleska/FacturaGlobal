using FacturaGlobal.Components.List;
using FacturaGlobal.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacturaGlobal
{
    internal class MyService
    {
        #region Variables
        /// <summary>
        /// Variables de SAP
        /// </summary>
        /// <param name="args"></param>
        public static List<Components.List.OV> listaOV = new List<Components.List.OV>();
        private static List<Entregas> listaEntregas = new List<Entregas>();
        public static bool band = false, band1 = false;
        #endregion
        public void Start()
        {
            // write code here that runs when the Windows Service starts up.  
            //Método para traer pedido en efectivo
            ConsoleSpinner spinner = new ConsoleSpinner();
            spinner.Delay = 300;
            //while (true)
            //{
            //    spinner.Turn(displayMsg: "Working ", sequenceCode: 5);
            //    spinner.Delay = 1000;
            //}
            Console.Clear();
            #region Efectivo
            for (int j = 0; j < Settings.Default.PublicoGeneral.Count; j++)
            {
                for (int i = 0; i < Settings.Default.Efectivo.Count; i++)
                {

                    //if (Settings.Default.PublicoGeneral[j] == "PG001")//dermaExpress
                    //{

                    Console.WriteLine("Hola soy el generador de Facturas Globales.");
                    Console.WriteLine("Vamos a buscar las ordenes de venta en efectivo.");
                    Console.WriteLine("Iniciamos a consultar las ordenes de venta/entregas en efectivo.");

                    if (Settings.Default.Efectivo[i].Contains("D") && Settings.Default.PublicoGeneral[j] == "PG001")
                    {
                        listaEntregas = Components.DL.Functions.getListOVEfectivo(Settings.Default.Efectivo[i], "PG001");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("A") && Settings.Default.PublicoGeneral[j] == "PG003")
                    {
                        listaEntregas = Components.DL.Functions.getListOVEfectivo(Settings.Default.Efectivo[i], "PG003");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("C") && Settings.Default.PublicoGeneral[j] == "PG002")
                    {
                        listaEntregas = Components.DL.Functions.getListOVEfectivo(Settings.Default.Efectivo[i], "PG002");
                    }


                    Console.WriteLine("Se termino de consultar los pedios/Entregas en efectio.");
                    if (listaEntregas.Count > 0)
                    {
                        //Console.WriteLine("Iniciamos con los pedidos de ventas de Alicia");

                        Console.WriteLine("Creamos la factura Global de efectivo.");
                        spinner.Turn(displayMsg: "Trabajando ", sequenceCode: 5);
                        Components.BL.Functions.CreateOverallInvoices(listaEntregas, "E", Settings.Default.Efectivo[i]);
                        Console.WriteLine("Se termino de generar la factura globa en efectivo.");

                        Console.WriteLine("-----------------------------------------------------------------------------------");
                        Console.WriteLine("Cerraremos las Ordenes de venta despues de que se crea la factura global en efectivo.");
                        Components.BL.Functions.UpdateOVList(listaEntregas);
                        Console.WriteLine("Se actualian todos los pedidos a cerrado por el CDU.");
                        //spinner.Turn(displayMsg: "Working ", sequenceCode: 5);
                    }
                    //}
                }
            }
            #endregion

            #region TC
            for (int j = 0; j < Settings.Default.PublicoGeneral.Count; j++)
            {
                for (int i = 0; i < Settings.Default.TC.Count; i++)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                    Console.WriteLine("Consultamos las ordenes de venta/entregas de crédito.");

                    if (Settings.Default.Efectivo[i].Contains("D") && Settings.Default.PublicoGeneral[j] == "PG001")
                    {
                        listaEntregas = Components.DL.Functions.getListOVCredito(Settings.Default.TC[i], "PG001");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("A") && Settings.Default.PublicoGeneral[j] == "PG003")
                    {
                        listaEntregas = Components.DL.Functions.getListOVCredito(Settings.Default.TC[i], "PG003");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("C") && Settings.Default.PublicoGeneral[j] == "PG002")
                    {
                        listaEntregas = Components.DL.Functions.getListOVCredito(Settings.Default.TC[i], "PG002");
                    }

                    if (listaOV.Count > 0)
                    {

                        Console.WriteLine("Se termino de consultar los pedidos/entregas.");
                        Console.WriteLine("Creamos las facturas globales de crédito.");
                        Components.BL.Functions.CreateOverallInvoices(listaEntregas, "C", "");
                        Console.WriteLine("Se termino de generar la factura global de crédito.");
                        Console.WriteLine("-----------------------------------------------------------------------------------");
                        Console.WriteLine("Cerraremos las Ordenes de venta despues de que se crea la factura global en efectivo.");
                        Components.BL.Functions.UpdateOVList(listaEntregas);
                        Console.WriteLine("Se actualian todos los pedidos a cerrado por el CDU.");
                    }
                }
            }
            #endregion

            #region TD
            for (int j = 0; j < Settings.Default.PublicoGeneral.Count; j++)
            {
                for (int i = 0; i < Settings.Default.TD.Count; i++)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                    Console.WriteLine("Consultamos las ordenes de venta/entregas de débito.");
                    //listaEntregas = Components.DL.Functions.getListOVDebito();

                    if (Settings.Default.Efectivo[i].Contains("D") && Settings.Default.PublicoGeneral[j] == "PG001")
                    {
                        listaEntregas = Components.DL.Functions.getListOVDebito(Settings.Default.TD[i], "PG001");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("A") && Settings.Default.PublicoGeneral[j] == "PG003")
                    {
                        listaEntregas = Components.DL.Functions.getListOVDebito(Settings.Default.TD[i], "PG003");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("C") && Settings.Default.PublicoGeneral[j] == "PG002")
                    {
                        listaEntregas = Components.DL.Functions.getListOVDebito(Settings.Default.TD[i], "PG002");
                    }
                    if (listaOV.Count > 0)
                    {
                        Console.WriteLine("Se termino de consultar los pedidos/entregas.");
                        Console.WriteLine("Creamos las facturas globales de débito.");
                        Components.BL.Functions.CreateOverallInvoices(listaEntregas, "D", "");
                        Console.WriteLine("Se termino de generar la factura global de débito.");
                        Console.WriteLine("Cerraremos las Ordenes de venta despues de que se crea la factura global en efectivo.");
                        Components.BL.Functions.UpdateOVList(listaEntregas);
                        Console.WriteLine("Se actualian todos los pedidos a cerrado por el CDU.");
                    }
                    Console.WriteLine("Se termino de generar la factura global.");
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                }
            }
            #endregion

            #region Transferencia
            for (int j = 0; j < Settings.Default.PublicoGeneral.Count; j++)
            {
                for (int i = 0; i < Settings.Default.Transferencia.Count; i++)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                    Console.WriteLine("Consultamos las ordenes de venta/entregas de Transferencia.");
                    //listaEntregas = Components.DL.Functions.getListOVDebito();

                    if (Settings.Default.Efectivo[i].Contains("D") && Settings.Default.PublicoGeneral[j] == "PG001")
                    {
                        listaEntregas = Components.DL.Functions.getListOVTransferencia(Settings.Default.Transferencia[i], "PG001");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("A") && Settings.Default.PublicoGeneral[j] == "PG003")
                    {
                        listaEntregas = Components.DL.Functions.getListOVTransferencia(Settings.Default.Transferencia[i], "PG003");
                    }
                    else if (Settings.Default.Efectivo[i].Contains("C") && Settings.Default.PublicoGeneral[j] == "PG002")
                    {
                        listaEntregas = Components.DL.Functions.getListOVTransferencia(Settings.Default.Transferencia[i], "PG002");
                    }
                    if (listaOV.Count > 0)
                    {
                        Console.WriteLine("Se termino de consultar los pedidos/entregas.");
                        Console.WriteLine("Creamos las facturas globales de débito.");
                        Components.BL.Functions.CreateOverallInvoices(listaEntregas, "D", "");
                        Console.WriteLine("Se termino de generar la factura global de débito.");
                        Console.WriteLine("Cerraremos las Ordenes de venta despues de que se crea la factura global en efectivo.");
                        Components.BL.Functions.UpdateOVList(listaEntregas);
                        Console.WriteLine("Se actualian todos los pedidos a cerrado por el CDU.");
                    }
                    Console.WriteLine("Se termino de generar la factura global.");
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                }
            }

            #endregion
        }
        public void Stop()
        {
            // write code here that runs when the Windows Service stops.  
        }
    }
}
