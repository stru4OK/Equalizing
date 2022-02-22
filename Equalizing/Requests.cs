using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Equalizing
{
    public class Requests
    {
        public static int GetResultBMSAPI(ResponseBMS Response)
        {
            if (Response.BpsResponse == null | Response == null)
                return 1;

            if (Response.BpsResponse.state == "ERROR")
                return 1;

            return 0;
        }

        public static ResponseBMS BMSRequest(string typeRequest, string request, string data, string login, string password)
        {
            ResponseBMS Response = new ResponseBMS();

            try
            {
                //Trace.TraceWrite("Request: \n" + request + "\n\n");

                CredentialCache cache = new CredentialCache();
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(request);
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(login + ":" + password));
                req.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;

                //Игнорируем недостоверный сертификат SSL
                ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                        System.Security.Cryptography.X509Certificates.X509Chain chain,
                        System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

                req.KeepAlive = false;
                req.Method = typeRequest;
                req.Timeout = 20000;

                if (!Equals(typeRequest, "GET"))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(data);
                    req.ContentType = "application/json";
                    req.ContentLength = byteArray.Length;
                    Stream dataStreamReq = req.GetRequestStream();
                    dataStreamReq.Write(byteArray, 0, byteArray.Length);
                    dataStreamReq.Close();
                }

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Stream dataStreamResp = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStreamResp, Encoding.Default);
                String resp = reader.ReadToEnd();
                Response = JsonConvert.DeserializeObject<ResponseBMS>(resp);

                Trace.TraceWrite("Request:\n" + request);
                Trace.TraceWrite("Response:\n" + resp);
                //Trace.TraceWrite("Responce on confirm request:\nmessage = " + Response.BpsResponse.message + ", state = " 
                //    + Response.BpsResponse.state + ", stateCode = " + Response.BpsResponse.stateCode + "");

                reader.Close();
                dataStreamResp.Close();
                response.Close();

                return Response;
            }
            catch (WebException e)
            {
                Trace.TraceWrite(e.ToString());

                try
                {
                    Stream streamData = e.Response.GetResponseStream();
                    var reader = new StreamReader(streamData);
                    String resp = reader.ReadToEnd();

                    Response = JsonConvert.DeserializeObject<ResponseBMS>(resp);
                    reader.Close();
                    streamData.Close();

                    Trace.TraceWrite("Request:\n" + request);
                    Trace.TraceWrite("Response:\n" + resp);

                    /*Trace.TraceWrite("Responce on confirm request:\nmessage = " + Response.BpsResponse.message + ", state = "
                    + Response.BpsResponse.state + ", stateCode = " + Response.BpsResponse.stateCode + "");*/
                }
                catch (Exception ex)
                {
                    Trace.TraceWrite("Responce on confirm request:\n" + ex.ToString() + "");
                }

                return Response;
            }
            catch (Exception ex)
            {
                Trace.TraceWrite("Responce on confirm request:\n" + ex.ToString() + "");
                return Response;
            }
        }
    }
}
