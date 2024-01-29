using FacturaGlobal.Properties;
using Newtonsoft.Json;
using Sap.Data.Hana;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FacturaGlobal.Components.List;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

namespace FacturaGlobal.Components.DL
{
    internal class Functions
    {
        #region Variables SAP
        /// <summary>
        /// Variables SAP
        /// </summary>
        private static HanaConnection cnn = new HanaConnection(Settings.Default.HanaConection);
        private static List.OV valores = new Components.List.OV();
        private static List<List.OV> listaOV = new List<Components.List.OV>();
        public static OV ovsap = new OV();
        private static List<Invoices> ListInvoices = new List<Invoices>();
        private static Invoices valoresInvoices = new Invoices();
        private static DocumentLines lines = new DocumentLines();

        private static Entregas valoresEntregas = new Entregas();
        private static List<Entregas> listaEntregas = new List<Entregas>();
        private static DetailsEntrega detalle = new DetailsEntrega();
        
        
        #endregion
        public static void ConexionSL()
        {
            try
            {
                
                var data = "{    \"CompanyDB\": \"" + Settings.Default.CompanyDB + "\",    \"UserName\": \"" + Settings.Default.UserSAP + "\",       \"Password\": \"" + Settings.Default.PassSAP + "\",\"Language\": \"25\"}";

                Log("DATA para services layer Conection: " + data + "" + DateTime.Now);

                var WebReq = (HttpWebRequest)WebRequest.Create(Properties.Settings.Default.SL + "Login");
                WebReq.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                WebReq.Method = "POST";
                WebReq.KeepAlive = true;
                WebReq.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                WebReq.Accept = "application/json;odata=minimalmetadata";
                WebReq.ServicePoint.Expect100Continue = false;

                WebReq.AllowAutoRedirect = true;
                WebReq.Timeout = 10000000;

                using (var streamWriter = new StreamWriter(WebReq.GetRequestStream()))
                { streamWriter.Write(data); }

                var httpResponse = (HttpWebResponse)WebReq.GetResponse();
                //listas.SLLogin obj = null;
                Globals.sLLogin = new List.SLLogin();


                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                    var obj = JsonConvert.DeserializeObject<dynamic>(result);
                    //var sesion = JsonConvert.DeserializeObject<dynamic>(result);
                    WebReq.Headers.Add("Cookie", $"B1SESSION=" + obj["SessionId"]);
                    httpResponse.Headers.Add("Cookie", $"B1SESSION=" + obj["SessionId"]);
                    Globals.sLLogin.SessionId = obj["SessionId"];
                    Globals.sLLogin.Version = obj["Version"];
                    Globals.sLLogin.SessionTimeout = obj["SessionTimeout"];
                    Log("Conexion establecida SL: " + DateTime.Now);
                }
                //return obj;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static List<List.Entregas> getListOVEfectivo(string un,string pg)
        {
            string mp = string.Empty;
            StringBuilder sb = new StringBuilder();

            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                //for (global::System.Int32 i = 0; i < Settings.Default.Efectivo.Count; i++)
                //{
                //    mp += Settings.Default.Efectivo[i] + ","; 
                //}

                //_ = mp.TrimEnd(',');
                if (pg == "PG001")
                {
                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append(" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from \"" + Settings.Default.CompanyDB + "\".ODLN T0 inner join \"" + Settings.Default.CompanyDB + "\".DLN1 T1 ");
                    sb.Append(" on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '01-EF-D'");

                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append("  where \"DocDate\" >= ADD_DAYS(TO_DATE(CURRENT_DATE), -" + Settings.Default.Frecuencia + ")");
                    //sb.Append(" and \"U_Estatus\" = 'A' \"PeyMethod\" = '" + un + "' LIMIT 1000");
                    valoresEntregas.CardCode = "PG001";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL DERMAEXPRESS";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "D";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.ProtocolCode = "";
                    //valoresEntregas.electronicProtocols = new ElectronicProtocols();
                    //valoresEntregas.electronicProtocols.GenerationType = "G";


                }
                else if (pg == "PG002")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'C' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '01-EF-C'");

                    valoresEntregas.CardCode = "PG002";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL CALL CENTER ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "C";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.GenerationType = "G";
                }
                else if (pg == "PG003")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'A' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '01-EF-A' ");

                    valoresEntregas.CardCode = "PG003";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL E-COMMERCE ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "A";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.e ectronicProtocols.GenerationType = "G";
                }

                valoresEntregas.DocumentLines = new List<DetailsEntrega>();

                using (HanaCommand command = new HanaCommand(sb.ToString(),cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {

                            detalle.ItemCode = _reader.GetString(0);
                            detalle.ItemName = _reader.GetString(1);
                            detalle.Quantity = _reader.GetString(2).Split(',')[0];
                            detalle.TaxCode = _reader.GetString(3);
                            detalle.UnitPrice = _reader.GetString(4).Replace(",",".");
                            detalle.WarehouseCode = _reader.GetString(5);
                            detalle.BaseEntry = _reader.GetString(6);
                            detalle.BaseLine = _reader.GetString(7);
                            detalle.BaseType = "15";
                            detalle.DiscountPercent = _reader.GetString(8).Split(',')[0];

                            valoresEntregas.DocumentLines.Add(detalle);
                            detalle = new DetailsEntrega();
                        }
                    }
                    listaEntregas.Add(valoresEntregas);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            return listaEntregas;
        }
        public static List<List.Entregas> getListOVDebito(string un, string pg)
        {
            string mp = string.Empty;
            StringBuilder sb = new StringBuilder();

            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                if (pg == "PG001")
                {
                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append(" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from \"" + Settings.Default.CompanyDB + "\".ODLN T0 inner join \"" + Settings.Default.CompanyDB + "\".DLN1 T1 ");
                    sb.Append(" on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '"+ un +"'");

                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append("  where \"DocDate\" >= ADD_DAYS(TO_DATE(CURRENT_DATE), -" + Settings.Default.Frecuencia + ")");
                    //sb.Append(" and \"U_Estatus\" = 'A' \"PeyMethod\" = '" + un + "' LIMIT 1000");
                    valoresEntregas.CardCode = "PG001";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL DERMAEXPRESS";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "D";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.ProtocolCode = "";
                    //valoresEntregas.electronicProtocols = new ElectronicProtocols();
                    //valoresEntregas.electronicProtocols.GenerationType = "G";


                }
                else if (pg == "PG002")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'C' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '"+ un +"'");

                    valoresEntregas.CardCode = "PG002";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL CALL CENTER ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "C";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses =  DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.GenerationType = "G";
                }
                else if (pg == "PG003")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'A' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '"+ un +"'");

                    valoresEntregas.CardCode = "PG003";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL E-COMMERCE ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "A";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.e ectronicProtocols.GenerationType = "G";
                }

                valoresEntregas.DocumentLines = new List<DetailsEntrega>();

                using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {

                            detalle.ItemCode = _reader.GetString(0);
                            detalle.ItemName = _reader.GetString(1);
                            detalle.Quantity = _reader.GetString(2).Split(',')[0];
                            detalle.TaxCode = _reader.GetString(3);
                            detalle.UnitPrice = _reader.GetString(4).Replace(",", ".");
                            detalle.WarehouseCode = _reader.GetString(5);
                            detalle.BaseEntry = _reader.GetString(6);
                            detalle.BaseLine = _reader.GetString(7);
                            detalle.BaseType = "15";
                            detalle.DiscountPercent = _reader.GetString(8).Split(',')[0];

                            valoresEntregas.DocumentLines.Add(detalle);
                            detalle = new DetailsEntrega();
                        }
                    }
                    listaEntregas.Add(valoresEntregas);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            return listaEntregas;
        }
        public static List<List.Entregas> getListOVCredito(string un, string pg)
        {
            string mp = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                if (pg == "PG001")
                {
                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append(" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from \"" + Settings.Default.CompanyDB + "\".ODLN T0 inner join \"" + Settings.Default.CompanyDB + "\".DLN1 T1 ");
                    sb.Append(" on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '"+ un +"'");

                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append("  where \"DocDate\" >= ADD_DAYS(TO_DATE(CURRENT_DATE), -" + Settings.Default.Frecuencia + ")");
                    //sb.Append(" and \"U_Estatus\" = 'A' \"PeyMethod\" = '" + un + "' LIMIT 1000");
                    valoresEntregas.CardCode = "PG001";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL DERMAEXPRESS";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "D";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.ProtocolCode = "";
                    //valoresEntregas.electronicProtocols = new ElectronicProtocols();
                    //valoresEntregas.electronicProtocols.GenerationType = "G";


                }
                else if (pg == "PG002")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'C' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '"+ un +"'");

                    valoresEntregas.CardCode = "PG002";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL CALL CENTER ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "C";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.GenerationType = "G";
                }
                else if (pg == "PG003")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'A' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '"+ un +"' ");

                    valoresEntregas.CardCode = "PG003";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL E-COMMERCE ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "A";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.e ectronicProtocols.GenerationType = "G";
                }

                valoresEntregas.DocumentLines = new List<DetailsEntrega>();

                using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {

                            detalle.ItemCode = _reader.GetString(0);
                            detalle.ItemName = _reader.GetString(1);
                            detalle.Quantity = _reader.GetString(2).Split(',')[0];
                            detalle.TaxCode = _reader.GetString(3);
                            detalle.UnitPrice = _reader.GetString(4).Replace(",", ".");
                            detalle.WarehouseCode = _reader.GetString(5);
                            detalle.BaseEntry = _reader.GetString(6);
                            detalle.BaseLine = _reader.GetString(7);
                            detalle.BaseType = "15";
                            detalle.DiscountPercent = _reader.GetString(8).Split(',')[0];

                            valoresEntregas.DocumentLines.Add(detalle);
                            detalle = new DetailsEntrega();
                        }
                    }
                    listaEntregas.Add(valoresEntregas);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            return listaEntregas;
        }
        public static List<List.Entregas> getListOVTransferencia(string un, string pg)
        {
            string mp = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                if (pg == "PG001")
                {
                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append(" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from \"" + Settings.Default.CompanyDB + "\".ODLN T0 inner join \"" + Settings.Default.CompanyDB + "\".DLN1 T1 ");
                    sb.Append(" on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'D' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '" + un + "'");

                    //sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\",\"U_UN\" from " + Settings.Default.CompanyDB + ".ODLN");
                    //sb.Append("  where \"DocDate\" >= ADD_DAYS(TO_DATE(CURRENT_DATE), -" + Settings.Default.Frecuencia + ")");
                    //sb.Append(" and \"U_Estatus\" = 'A' \"PeyMethod\" = '" + un + "' LIMIT 1000");
                    valoresEntregas.CardCode = "PG001";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL DERMAEXPRESS";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "D";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.ProtocolCode = "";
                    //valoresEntregas.electronicProtocols = new ElectronicProtocols();
                    //valoresEntregas.electronicProtocols.GenerationType = "G";


                }
                else if (pg == "PG002")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'C' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '" + un + "'");

                    valoresEntregas.CardCode = "PG002";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL CALL CENTER ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "C";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.electronicProtocols.GenerationType = "G";
                }
                else if (pg == "PG003")
                {
                    sb.Append("select T1.\"ItemCode\",T1.\"Dscription\",T1.\"Quantity\",T1.\"TaxCode\",T1.\"Price\",T1.\"WhsCode\",T1.\"DocEntry\"");
                    sb.Append(" ,T1.\"LineNum\",T1.\"DiscPrcnt\" from ODLN T0 inner join DLN1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" where \"U_UN\" = 'A' and \"U_Estatus\" = 'A' ");
                    sb.Append(" and \"DocStatus\" = 'O' and T0.\"PeyMethod\" = '" + un + "' ");

                    valoresEntregas.CardCode = "PG003";
                    valoresEntregas.CardName = "PUBLICO EN GENERAL E-COMMERCE ALICIA";
                    valoresEntregas.DocDueDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.DocDate = DateTime.Today.ToShortDateString();
                    valoresEntregas.GroupNum = "2";
                    valoresEntregas.PaymentMethod = un;
                    valoresEntregas.U_UN = "A";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_MainUsage = "S01";
                    valoresEntregas.U_IL_Periodicidad = "02";
                    valoresEntregas.U_IL_Meses = DateTime.Today.Month.ToString("00");
                    valoresEntregas.U_IL_Anio = DateTime.Today.Year.ToString();
                    valoresEntregas.Comments = "Factura global fecha: " + DateTime.Today.ToShortDateString();
                    valoresEntregas.LicTradNum = "XAXX010101000";
                    valoresEntregas.U_IL_Timbrar = "Y";
                    valoresEntregas.U_B1SYS_CFDiYear = DateTime.Today.Year.ToString();

                    //valoresEntregas.e ectronicProtocols.GenerationType = "G";
                }

                valoresEntregas.DocumentLines = new List<DetailsEntrega>();

                using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {

                            detalle.ItemCode = _reader.GetString(0);
                            detalle.ItemName = _reader.GetString(1);
                            detalle.Quantity = _reader.GetString(2).Split(',')[0];
                            detalle.TaxCode = _reader.GetString(3);
                            detalle.UnitPrice = _reader.GetString(4).Replace(",", ".");
                            detalle.WarehouseCode = _reader.GetString(5);
                            detalle.BaseEntry = _reader.GetString(6);
                            detalle.BaseLine = _reader.GetString(7);
                            detalle.BaseType = "15";
                            detalle.DiscountPercent = _reader.GetString(8).Split(',')[0];

                            valoresEntregas.DocumentLines.Add(detalle);
                            detalle = new DetailsEntrega();
                        }
                    }
                    listaEntregas.Add(valoresEntregas);
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            return listaEntregas;
        }
        public static List<List.OV> getListOVTransferencia()
        {
            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\" from " + Settings.Default.CompanyDB + ".ORDR");
                sb.Append("  where \"DocDate\" >= ADD_DAYS(TO_DATE(CURRENT_DATE), -" + Settings.Default.Frecuencia + ")");
                sb.Append(" and \"U_Estado\" = 'A' and \"U_UN\" = 'D' and \"PeyMethod\" ='" + Settings.Default.Transferencia + "' LIMIT 1000");

                using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {
                            valores.DocNum = _reader.GetString(0);
                            valores.CardCode = _reader.GetString(1);
                            valores.CardName = _reader.GetString(2);
                            valores.DocDate = _reader.GetString(3);
                            valores.DocDueDate = _reader.GetString(4);
                            valores.DocTotal = _reader.GetString(5);

                            listaOV.Add(valores);
                            valores = new Components.List.OV();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            return listaOV;
        }
        public static List<OV> getListDeliveryNotes()
        {
            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("select \"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\" from " + Settings.Default.CompanyDB + ".ODLN");
                sb.Append("  where \"DocDate\" >= ADD_DAYS(TO_DATE(CURRENT_DATE), -" + Settings.Default.Frecuencia + ")");
                sb.Append(" and \"U_Estado\" = 'A' and \"U_UN\" = 'D' LIMIT 1000");

                using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {
                            valores.DocNum = _reader.GetString(0);
                            valores.CardCode = _reader.GetString(1);
                            valores.CardName = _reader.GetString(2);
                            valores.DocDate = _reader.GetString(3);
                            valores.DocDueDate = _reader.GetString(4);
                            valores.DocTotal = _reader.GetString(5);

                            listaOV.Add(valores);
                            valores = new Components.List.OV();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error al buscar entregas: " + ex.Message);
            }
            return listaOV;
        }
        public static List<Invoices> getListInvoices(List<Entregas> ListDelivery, string serie)
        {
            try
            {
                string DocNumOV = string.Empty;
                decimal docTotal= decimal.Zero;

                ListInvoices = new List<Invoices>();
                //foreach (var item in ListDelivery)
                //{
                //    DocNumOV += item.DocNum + ",";
                //    docTotal += decimal.Parse(item.DocTotal);
                //}
                //foreach (OV o in ListDelivery)
                //{

                switch (serie.Substring(serie.Length -1,1))
                {
                    case "A":
                        valoresInvoices.Series = Settings.Default.Serie;
                        valoresInvoices.PeyMethod = Settings.Default.PeyMethodA;
                        valoresInvoices.GroupNum = Settings.Default.GroupNum;
                        break; 
                    case "D":
                        valoresInvoices.Series = Settings.Default.Serie;
                        valoresInvoices.PeyMethod = Settings.Default.PeyMethodD;
                        valoresInvoices.GroupNum = Settings.Default.GroupNum;
                        break;
                    case "C":
                        valoresInvoices.Series = Settings.Default.Serie;
                        valoresInvoices.PeyMethod = Settings.Default.PeyMethodC;
                        valoresInvoices.GroupNum = Settings.Default.GroupNum;
                        break;
                }

                valoresInvoices.CardCode = Settings.Default.CardCode;
                valoresInvoices.CardName = GetCardName(Settings.Default.CardCode);

                valoresInvoices.DocDate = DateTime.Now.ToShortDateString();
                valoresInvoices.DocDueDate= DateTime.Now.ToShortDateString();

                //valoresInvoices.U_MetodoPago = Settings.Default.MetodoPago;
                valoresInvoices.U_B1SYS_CFDiPeriod = Settings.Default.Periodicidad;
                valoresInvoices.U_B1SYS_CFDiMonths = DateTime.Now.Month.ToString();
                valoresInvoices.U_B1SYS_CFDiYear = DateTime.Now.Year.ToString();
                valoresInvoices.Comments = DocNumOV.Substring(0,DocNumOV.Length - 1);

                for (int i = 0; i < ListDelivery.Count; i++)
                {
                    for (int j = 0; j < ListDelivery[i].DocumentLines.Count; j++)
                    {
                        lines.ItemCode = Settings.Default.ItemCode;
                        lines.ItemDescription = GetItemDscripciont(Settings.Default.ItemCode);
                        lines.Quantity = "1";
                        lines.UnitPrice = docTotal.ToString();
                        valoresInvoices.DocumentLines.Add(lines);
                    }
                }
                //lines.ItemCode = Settings.Default.ItemCode;
                //lines.ItemDescription = GetItemDscripciont(Settings.Default.ItemCode);
                //lines.Quantity = "1";
                //lines.UnitPrice = docTotal.ToString();

                //valoresInvoices.DocumentLines.Add(lines);
                //}
                valoresInvoices.DocTotal = docTotal.ToString();

                ListInvoices.Add(valoresInvoices);
                valoresInvoices = new Invoices();

                ////
                ///Esta consulta es para cargar la lista de las facturas

                //if (cnn.State == ConnectionState.Closed)
                //{
                //    cnn.Open();
                //}

                //StringBuilder sb = new StringBuilder();
                //sb.Append("select \"DocEntry\",\"DocNum\",\"CardCode\",\"CardName\",\"DocDate\",\"DocDueDate\",\"DocTotal\" ");
                //sb.Append(" from ");
                //sb.Append(Settings.Default.CompanyDB + ".OINV");
                //sb.Append("  where \"DocDate\" >= ADD_DAYS(TO_DATE(CURRENT_DATE), -" + Settings.Default.Frecuencia + ")");
                //sb.Append(" and \"U_Estado\" = 'A' and \"U_UN\" = 'D' LIMIT 1000");

                //using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                //{
                //    HanaDataReader _reader = command.ExecuteReader();
                //    if (_reader.HasRows)
                //    {
                //        while (_reader.Read())
                //        {
                //            valores.DocNum = _reader.GetString(0);
                //            valores.CardCode = _reader.GetString(1);
                //            valores.CardName = _reader.GetString(2);
                //            valores.DocDate = _reader.GetString(3);
                //            valores.DocDueDate = _reader.GetString(4);
                //            valores.DocTotal = _reader.GetString(5);

                //            listaOV.Add(valores);
                //            valores = new Components.List.OV();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log("Error: Error al obtener las facturas: " + ex.Message);
            }
            return ListInvoices;
        }
        public static string GetCardName(string CardCode)
        {
            string CardName = string.Empty;
            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("select \"CardName\" ");
                sb.Append(" from " + Settings.Default.CompanyDB + ".OCRD");
                sb.Append(" where \"CardCode\" ='"+ Settings.Default.CardCode +"'; ");

                using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {
                           CardName = _reader.GetString(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error al consultar el CardName: " + ex.Message);
            }
            return CardName;
        }
        public static string GetItemDscripciont(string ItemCode)
        {
            string ItemDscription = string.Empty;
            try
            {
                if (cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("select \"ItemName\" ");
                sb.Append(" from " + Settings.Default.CompanyDB + ".OITM");
                sb.Append(" where \"ItemCode\" ='" + Settings.Default.ItemCode + "'; ");

                using (HanaCommand command = new HanaCommand(sb.ToString(), cnn))
                {
                    HanaDataReader _reader = command.ExecuteReader();
                    if (_reader.HasRows)
                    {
                        while (_reader.Read())
                        {
                            ItemDscription = _reader.GetString(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error al traer la descripción del artículo: " + ex.Message);
            }
            return ItemDscription;
        }

        public static void Log(string error)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string hoy = DateTime.Today.ToShortDateString().Replace("/", "-");


                if (!Directory.Exists(path + "//Logs"))
                {
                    Directory.CreateDirectory(path + "//Logs");
                }
                if (!File.Exists(path + "//Logs//" + hoy + ".txt"))
                {
                    File.Exists(path + "//Logs//" + hoy + ".txt");
                }

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + "//Logs//" + hoy + ".txt", true))
                {
                    file.WriteLine(DateTime.Now + " - " + error);
                }

            }
            catch (Exception ex)
            {
                //Log(ex.Message);
            }
        }

    }
}
