using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equalizing
{
    public class EqualizingProcess
    {
        public static string CreateEqualizingAndConfirm(string oracleDBConnection, string serverAddr, string redmineTicket, 
            string date, string cardNum, string terminalCode, string billSum, string spendBonus, string earnBonus, string organizerFee)
        {
            string request_id = Guid.NewGuid().ToString();

            string result = DBMethods.CreateEqualizing(oracleDBConnection, serverAddr, request_id, redmineTicket, date, cardNum, terminalCode, billSum,
                    spendBonus, earnBonus, organizerFee);

            if (!String.IsNullOrEmpty(result)) return result;

            result = ConfirmRequest.BMSConfirmOperation(oracleDBConnection, serverAddr, request_id);

            if (!String.IsNullOrEmpty(result))
                return result;
            else
                return result = "Корректировка проведена и подтверждена!";
        }
    }
}
