using FacturaGlobal.Components.List;
using FacturaGlobal.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;
using System.Xml.Linq;

namespace FacturaGlobal.Components.BL
{
    internal class Functions
    {
        #region Variables SAP
        /// <summary>
        /// Variables SAP
        /// </summary>
        private static List.OV valores = new Components.List.OV();
        private static List<List.OV> listaOV = new List<Components.List.OV>();
        public static OV ovsap = new OV();
        private static List<Invoices> ListInvoices = new List<Invoices>();
        #endregion

        public static void UpdateOVList(List<Entregas> lista)
        {
            try
            {
                string data = @"{\""U_Estatus\"":'C'}";
                foreach (var item in lista)
                {
                    for (int i = 0; i < item.DocumentLines.Count; i++)
                    {

                        string url = Settings.Default.SL + "DeliveryNotes(" + item.DocumentLines[i].BaseEntry + ")";
                        //var json = JsonConvert.SerializeObject(item);
                        var content = new StringContent(data, Encoding.UTF8, "application/json");

                        var WebReq = (HttpWebRequest)WebRequest.Create(url);
                        WebReq.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                        WebReq.Method = "PATCH";
                        WebReq.KeepAlive = true;
                        WebReq.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                        WebReq.Accept = "application/json;odata=minimalmetadata";
                        WebReq.ServicePoint.Expect100Continue = false;

                        WebReq.AllowAutoRedirect = true;
                        WebReq.Timeout = 10000000;
                        WebReq.Headers.Add("Cookie", $"B1SESSION=" + Globals.sLLogin.SessionId);

                        using (var streamWriter = new StreamWriter(WebReq.GetRequestStream()))
                        { streamWriter.Write(data); }

                        var httpResponse = (HttpWebResponse)WebReq.GetResponse();
                        //listas.SLLogin obj = null;
                        //Globals.sLLogin = new listas.SLLogin();

                        if (httpResponse.StatusCode == HttpStatusCode.Created)
                        {
                            var responseString = new
                                StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                            ovsap = JsonConvert.DeserializeObject<OV>(responseString);
                            ////var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                            Components.DL.Functions.Log(DateTime.Now + "Entregas actualizada en SAP ");
                            //lista.DocNum = ovsap.DocNum;
                            //lista.DocEntry = ovsap.DocEntry;
                            //BL.Functions.CreateInvoices(ovsap.DocEntry, lista, estado);

                        }
                        else
                        {
                            var responseString = new
                               StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                            List.errores err = JsonConvert.DeserializeObject<Components.List.errores>(responseString);
                            //var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                            Components.DL.Functions.Log(DateTime.Now + "Error al actualizar Entregas : " + err.error.message.value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Components.DL.Functions.Log("Error al actualizar orden de venta: " + ex.Message);
            }
        }
        public static void CloseOV(string DocEntry)
        {
            try
            {
                string url = Settings.Default.SL + "/Orders(" + DocEntry + ")/Close";
                //var json = JsonConvert.SerializeObject(lista);
                //var content = new StringContent(data, Encoding.UTF8, "application/json");

                var WebReq = (HttpWebRequest)WebRequest.Create(url);
                WebReq.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                WebReq.Method = "POST";
                WebReq.KeepAlive = true;
                WebReq.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                WebReq.Accept = "application/json;odata=minimalmetadata";
                WebReq.ServicePoint.Expect100Continue = false;

                WebReq.AllowAutoRedirect = true;
                WebReq.Timeout = 10000000;
                WebReq.Headers.Add("Cookie", $"B1SESSION=" + Globals.sLLogin.SessionId);

                //using (var streamWriter = new StreamWriter(WebReq.GetRequestStream()))
                //{ streamWriter.Write(data); }

                var httpResponse = (HttpWebResponse)WebReq.GetResponse();
                //listas.SLLogin obj = null;
                //Globals.sLLogin = new listas.SLLogin();

                if (httpResponse.StatusCode == HttpStatusCode.Created)
                {
                    var responseString = new
                        StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                    //ovsap = JsonConvert.DeserializeObject<DL.listas.OVSAP>(responseString);
                    ////var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                    Components.DL.Functions.Log(DateTime.Now + "OV actualizada en SAP ");
                    //lista.DocNum = ovsap.DocNum;
                    //lista.DocEntry = ovsap.DocEntry;
                    //BL.Functions.CreateInvoices(ovsap.DocEntry, lista, estado);

                }
                else
                {
                    var responseString = new
                       StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                    List.errores err = JsonConvert.DeserializeObject<Components.List.errores>(responseString);
                    //var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                    Components.DL.Functions.Log(DateTime.Now + "Error al cerrar OV: " + err.error.message.value);
                }
            }
            catch (Exception ex)
            {
                Components.DL.Functions.Log("Error al cerrar la orden de venta: " + ex.Message);
            }
        }
        public static void UpdateDeliveryNotes(List<List.OV> lista)
        {
            try
            {
                string data = @"{\""U_Estatus\"":'C'}";
                foreach (var item in lista)
                {

                    string url = Settings.Default.SL + "/DeliveryNotes(" + item.DocEntry + ")";
                    //var json = JsonConvert.SerializeObject(lista);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");

                    var WebReq = (HttpWebRequest)WebRequest.Create(url);
                    WebReq.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                    WebReq.Method = "PATCH";
                    WebReq.KeepAlive = true;
                    WebReq.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    WebReq.Accept = "application/json;odata=minimalmetadata";
                    WebReq.ServicePoint.Expect100Continue = false;

                    WebReq.AllowAutoRedirect = true;
                    WebReq.Timeout = 10000000;
                    WebReq.Headers.Add("Cookie", $"B1SESSION=" + Globals.sLLogin.SessionId);
                    

                    using (var streamWriter = new StreamWriter(WebReq.GetRequestStream()))
                    { streamWriter.Write(data); }

                    var httpResponse = (HttpWebResponse)WebReq.GetResponse();
                    //listas.SLLogin obj = null;
                    //Globals.sLLogin = new listas.SLLogin();

                    if (httpResponse.StatusCode == HttpStatusCode.Created)
                    {
                        var responseString = new
                            StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                        ovsap = JsonConvert.DeserializeObject<OV>(responseString);
                        ////var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                        Components.DL.Functions.Log(DateTime.Now + "OV actualizada en SAP ");
                        //lista.DocNum = ovsap.DocNum;
                        //lista.DocEntry = ovsap.DocEntry;
                        //BL.Functions.CreateInvoices(ovsap.DocEntry, lista, estado);

                    }
                    else
                    {
                        var responseString = new
                           StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                        List.errores err = JsonConvert.DeserializeObject<Components.List.errores>(responseString);
                        //var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                        Components.DL.Functions.Log(DateTime.Now + "Error al actualizar OV: " + err.error.message.value);
                    }
                }
            }
            catch (Exception ex)
            {
                Components.DL.Functions.Log("Error Update DeliveryNote: " + ex.Message);
            }
        }
        public static void CloseDeliveryNotes(string DocEntry)
        {
            try
            {
                string url = Settings.Default.SL + "/Orders(" + DocEntry + ")/Close";
                //var json = JsonConvert.SerializeObject(lista);
                //var content = new StringContent(data, Encoding.UTF8, "application/json");

                var WebReq = (HttpWebRequest)WebRequest.Create(url);
                WebReq.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                WebReq.Method = "POST";
                WebReq.KeepAlive = true;
                WebReq.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                WebReq.Accept = "application/json;odata=minimalmetadata";
                WebReq.ServicePoint.Expect100Continue = false;

                WebReq.AllowAutoRedirect = true;
                WebReq.Timeout = 10000000;
                WebReq.Headers.Add("Cookie", $"B1SESSION=" + Globals.sLLogin.SessionId);

                //using (var streamWriter = new StreamWriter(WebReq.GetRequestStream()))
                //{ streamWriter.Write(data); }

                var httpResponse = (HttpWebResponse)WebReq.GetResponse();
                //listas.SLLogin obj = null;
                //Globals.sLLogin = new listas.SLLogin();

                if (httpResponse.StatusCode == HttpStatusCode.Created)
                {
                    var responseString = new
                        StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                    //ovsap = JsonConvert.DeserializeObject<DL.listas.OVSAP>(responseString);
                    ////var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                    Components.DL.Functions.Log(DateTime.Now + "OV actualizada en SAP ");
                    //lista.DocNum = ovsap.DocNum;
                    //lista.DocEntry = ovsap.DocEntry;
                    //BL.Functions.CreateInvoices(ovsap.DocEntry, lista, estado);

                }
                else
                {
                    var responseString = new
                       StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                    List.errores err = JsonConvert.DeserializeObject<Components.List.errores>(responseString);
                    //var model = JsonConvert.DeserializeObject<DL.listas.errores>(text);
                        DL.Functions.Log(DateTime.Now + "Error al cerrar OV: " + err.error.message.value);
                }
            }
            catch (Exception ex)
            {
                Components.DL.Functions.Log("Error al cerrar entrega: " + ex.Message);
            }
        }
        public static void CreateOverallInvoices(List<Entregas> ListaOV, string type, string serie)
        {
            try
            {
                //string data = @"{CardCode: \"""+ Properties.Settings.Default.CardCode +"\","  +
                //                 "NumAtCard: \"" + Settings.Default.Rfc + "\"," +
                //                 "Comments: \"" + Settings.Default.Rfc + "\"," +
                //                 "Series: \"" + Settings.Default.Serie + "\"," +
                //                 "U_B1SYS_MainUsage: \"" +  Settings.Default.UsoCFDI + "\"," +
                //                 "U_MetodoPago: \"" + Settings.Default.MetodoPago + "\"," +

                //    "}";

                //ListInvoices = Components.DL.Functions.getListInvoices(listaOV,serie);


                //foreach (var invoice in listaOV)
                //{
                Components.DL.Functions.ConexionSL();

                    string url = Settings.Default.SL + "Invoices";
                    var json = JsonConvert.SerializeObject(ListaOV);

                var formatjson = json.Substring(1, json.Length - 2);
                    var content = new StringContent(formatjson, Encoding.UTF8, "application/json");

                    var WebReq = (HttpWebRequest)WebRequest.Create(url);
                    WebReq.ContentType = "application/json;odata=minimalmetadata;charset=utf8";
                    WebReq.Method = "POST";
                    WebReq.KeepAlive = true;
                    WebReq.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    WebReq.Accept = "application/json;odata=minimalmetadata";
                    WebReq.ServicePoint.Expect100Continue = false;

                    WebReq.AllowAutoRedirect = true;
                    WebReq.Timeout = 10000000;
                    WebReq.Headers.Add("Cookie", $"B1SESSION=" + Globals.sLLogin.SessionId);

                    using (var streamWriter = new StreamWriter(WebReq.GetRequestStream()))
                    { streamWriter.Write(formatjson); }

                    var httpResponse = (HttpWebResponse)WebReq.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var obj = JsonConvert.DeserializeObject<dynamic>(result);


                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            //band = true;
                        }
                        else
                        {
                            //band = false;
                        }
                    }
                //}
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    // TODO: Handle response being null
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        var text = reader.ReadToEnd();
                        var model = JsonConvert.DeserializeObject<Components.List.errores>(text);
                        DL.Functions.Log(model.error.message.value);
                    }
                }
                //DL.Functions.Log(ex.Message);
            }
        }
        public static void SendInvoicesSAP(char UN)
        {
            try
            {

            }
            catch (Exception ex)
            {
                DL.Functions.Log(ex.Message);
            }
        }
    }
}
