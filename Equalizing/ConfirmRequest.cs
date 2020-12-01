using System;

namespace Equalizing
{
    public class ConfirmRequest
    {
        public static string BMSConfirmOperation(string oracleDBConnection, string serverAddr, string request_id)
        {
            string result = String.Empty;

            if (Requests.GetResultBMSAPI(Requests.BMSRequest("GET", serverAddr.Remove(serverAddr.Length - 5, 5) + "/do.PROCESS_REQUEST/param={\"REQUEST_ID\":\"" + request_id
                + "\"}", String.Empty, String.Empty, String.Empty)) == 1)
                result = "Ошибка при подтверждении корректировки";

            return result;
        }
    }
}
