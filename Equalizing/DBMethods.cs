using Oracle.ManagedDataAccess.Client;
using System;
using System.Threading;

namespace Equalizing
{
    public class DBMethods
    {
        public static string TestConnectionDataBase(string oracleDBConnection)
        {
            string result = String.Empty;

            OracleConnection conn = new OracleConnection(oracleDBConnection);

            try
            {
                conn.Open();
                OracleCommand dbcmd = conn.CreateCommand();

                return result;
            }
            catch (ArgumentException ex)
            {
                Trace.TraceWrite("Test DB failed: " + ex.ToString() + "\n\n");
                result = "Ошибка соединения с БД";

                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceWrite("Test DB failed: " + ex.ToString() + "\n\n");
                result = "Указан неверный адрес БД";

                return result;
            }
            finally
            {
                OracleConnection.ClearPool(conn);
                conn.Dispose();
                conn.Close();
                conn = null;
            }
        }

        public static string TestEqualizingData(string oracleDBConnection, string code, string cardNum, string spendBonus)
        {
            string result = string.Empty;

            result = DataBaseSQL(oracleDBConnection, "select data from (select 'Терминал заблокирован' as data from dual where exists(select * from terminals where code = '" + code + "' and is_locked = 1) "
                + "union select 'Такого терминала нет в данной БД' as data from dual where not exists(select * from terminals where code = '" + code + "') "
                + "union select 'Такой карты нет в данной БД' as data from dual where not exists(select * from cards where card_num = replace('" + cardNum + "', chr(32), '')) "
                + "union select 'Карта заблокирована' as data from dual where not exists(select * from cards where card_num = replace('" + cardNum + "', chr(32), '') and is_delete = 0 and is_locked = 0) "
                + "union select 'Счет клиента в результате корректировки окажется в минусе' as data from dual where 0 > (select balance - " + spendBonus + " from accounts where account_type = 'GLOBAL' "
                + "and cli_id in (select cli_id from cards where card_num = replace('" + cardNum + "', chr(32), '') and is_delete = 0 and is_locked = 0)) "
                + "union select 'Счет клиента в минусе' as data from dual where 0 > (select balance from accounts where account_type = 'GLOBAL' "
                + "and cli_id in (select cli_id from cards where card_num = replace('" + cardNum + "', chr(32), '') and is_delete = 0 and is_locked = 0)) "
                + "union select 'У введенной карты нет счетов в данной БД' as data from dual where not exists(select * from accounts where cli_id in (select cli_id from cards where card_num = replace('"
                + cardNum + "', chr(32), ''))))", true);

            return result;
        }

        public static string CreateEqualizing(string oracleDBConnection, string serverAddr, string request_id, string redmineTicket, string date, string cardNum, string terminalCode, string billSum,
            string spendBonus, string earnBonus, string organizerFee)
        {
            string result, bill_id, good_id;
            
            result = DataBaseSQL(oracleDBConnection, "insert into summary_data (request_id, request_date, ins_date, ext_request_id, request_type, request_state, request_state_code, "
                + "is_offline, club_id, retail_network_id, legacy_client_id, retail_point_id, terminal_id, cli_id, fio, phone_mobile, card_num, card_mask, card_type, rolled_card_type, "
                + "is_card_locked, bill_date, bill_code, bill_sum, bill_sum_with_bonus, cashbacks_sum, bonus_sum, bonus_sum_ep, organizer_fee, bonus_sum_minus_org_fee, bonus_credit_sum, "
                + "extra_amount_withdraw, extra_amount_reverse, extra_amount_withdraw_fee) "
                + "select '" + request_id + "', to_date('" + date + "', 'dd.mm.yyyy hh24:mi:ss'), sysdate, '" 
                + request_id + "', 'PAYMENT_AND_CONFIRM', 'PROCESSED', 'OK', 0, (select club_id from retail_networks where retail_network_id = (select network_id from retail_points "
                + "where retail_point_id = (select retail_point_id from terminals where code = '" + terminalCode + "'))), (select network_id from retail_points "
                + "where retail_point_id = (select retail_point_id from terminals where code = '" + terminalCode + "')), (select legacy_cli_id from retail_points "
                + "where retail_point_id = (select retail_point_id from terminals where code = '" + terminalCode + "')), (select retail_point_id from terminals where code = '" 
                + terminalCode + "'), (select terminal_id from terminals where code = '" + terminalCode + "'), (select cli_id from cards where card_num = '" + cardNum 
                + "' and is_delete = 0 and is_locked = 0), (select last_name || ' ' || first_name || ' ' || middle_name from physical_clients where cli_id = "
                + "(select cli_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0)), (select phone_mobile from physical_clients where cli_id = "
                + "(select cli_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0)), '" + cardNum + "', '" + cardNum + "', "
                + "(select card_type from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0), 'ROLLED_CARD', 0, to_date('" + date 
                + "', 'dd.mm.yyyy hh24:mi:ss'), '" + redmineTicket + "', " + billSum + ", " + billSum + " - " + spendBonus + ", 0, " + earnBonus + ", 0, " 
                + organizerFee + ", " + earnBonus + " - " + organizerFee + ", " + spendBonus + ", 0, 0, 0 from dual where exists (select cli_id from cards where card_num = '" 
                + cardNum + "' and is_delete = 0 and is_locked = 0)", false);

            if (!String.IsNullOrEmpty(result)) return result;

            bill_id = DataBaseSQL(oracleDBConnection, "select bills$seq.nextval as data from dual", true);
            good_id = DataBaseSQL(oracleDBConnection, "select goods$seq.nextval as data from dual", true);

            result = DataBaseSQL(oracleDBConnection, "insert into requests (request_id, ext_request_id, request_date, request_state, request_type, card_id, terminal_id, request_state_code, employee_code, ins_date, upd_date) "
                + "select '" + request_id + "', '" + request_id + "', to_date('" + date + "', 'dd.mm.yyyy hh24:mi:ss'), 'READY', 'PAYMENT_AND_CONFIRM', (select card_id from cards where card_num = '" + cardNum + "' "
                + "and is_delete = 0 and is_locked = 0), (select terminal_id from terminals where code = '" + terminalCode + "'), 'OK', 'EQUALIZING', sysdate, sysdate from dual", false);

            if (!String.IsNullOrEmpty(result)) return result;

            result = result + DataBaseSQL(oracleDBConnection, "insert into bills (bill_id, bill_code, bill_date, bill_length, bill_sum, is_spend_bonus, request_id, ins_date, upd_date, is_processed, state) "
                + "select " + bill_id + ", '" + redmineTicket + "', to_date('" + date + "', 'dd.mm.yyyy hh24:mi:ss'), 1, " + billSum + ", "
                + "(select(case when " + spendBonus + " > 0 then 1 else 0 end) from dual), '" + request_id + "', sysdate, sysdate, 0, 'PROCESSED' from dual", false);

            if (!String.IsNullOrEmpty(result)) return result;

            result = result + DataBaseSQL(oracleDBConnection, "insert into goods (good_id, bill_id, good_code, amount, good_title, is_discount_available, order_num, position_price, ins_date, upd_date, good_type, state) "
                + "select " + good_id + ", " + bill_id + ", '" + redmineTicket + "', 1, '" + redmineTicket + "', 0, 1, " + billSum + ", sysdate, sysdate, 'GOOD', 'CONFIRMED'  from dual", false);

            if (!String.IsNullOrEmpty(result)) return result;

            result = result + DataBaseSQL(oracleDBConnection, "insert into transactions (transaction_id, operation_type, transaction_state, amount, account_id, good_id, card_id, bill_id, ins_date, upd_date, transaction_kind, "
                + "request_id, retail_point_id, bonus_type) select transactions$seq.nextval, 'DEBIT', 'READY', " + earnBonus + ", (select account_id from accounts where account_type = 'GLOBAL' "
                + "and cli_id = (select cli_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0)), " + good_id + ", "
                + "(select card_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0), " + bill_id + ", sysdate, sysdate, 'PAYMENT_DEBIT', '" + request_id + "', "
                + "(select retail_point_id from terminals where code = '" + terminalCode + "'), 'CASH' from dual where " + earnBonus + " > 0", false);

            if (!String.IsNullOrEmpty(result)) return result;

            result = result + DataBaseSQL(oracleDBConnection, "insert into transactions(transaction_id, operation_type, transaction_state, amount, account_id, good_id, card_id, bill_id, ins_date, upd_date, transaction_kind, "
                + "request_id, retail_point_id, bonus_type) select transactions$seq.nextval, 'CREDIT', 'READY', " + organizerFee + ", (select account_id from accounts where account_type = 'GLOBAL' "
                + "and cli_id = (select cli_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0)), " + good_id + ", "
                + "(select card_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0), " + bill_id + ", sysdate, sysdate, 'ORGANIZER_FEE', '" + request_id + "', "
                + "(select retail_point_id from terminals where code = '" + terminalCode + "'), 'CASH' from dual where " + organizerFee + " > 0", false);

            if (!String.IsNullOrEmpty(result)) return result;

            result = result + DataBaseSQL(oracleDBConnection, "insert into transactions(transaction_id, operation_type, transaction_state, amount, account_id, good_id, card_id, bill_id, ins_date, upd_date, transaction_kind, "
                + "request_id, retail_point_id, bonus_type) select transactions$seq.nextval, 'CREDIT', 'READY', " + spendBonus + ", (select account_id from accounts where account_type = 'GLOBAL' "
                + "and cli_id = (select cli_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0)), " + good_id + ", "
                + "(select card_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0), " + bill_id + ", sysdate, sysdate, 'PAYMENT_CREDIT', '" + request_id + "', "
                + "(select retail_point_id from terminals where code = '" + terminalCode + "'), 'CASH' from dual where " + spendBonus + " > 0", false);

            if (!String.IsNullOrEmpty(result)) return result;

            result = result + DataBaseSQL(oracleDBConnection, "update accounts set amount = amount + " + earnBonus + " - " + organizerFee + ", balance = balance - " 
                + spendBonus + ", locked_amount = locked_amount + " + earnBonus + " - " + organizerFee + " + " + spendBonus + " where account_type = 'GLOBAL' "
                + "and cli_id = (select cli_id from cards where card_num = '" + cardNum + "' and is_delete = 0 and is_locked = 0)", false);
                        
            return result;
        }

        /*public static string DataBaseSQL(string oracleDBConnection, string sql, bool needResult)
        {
            DBResult DBResult = DBSQL(oracleDBConnection, sql, needResult);

            while (string.Equals(DBResult.state, Variables.ERROR))
            {
                Thread.Sleep(1000);
                DBResult = DBSQL(oracleDBConnection, sql, needResult);
            }

            return DBResult.result;
        }*/

        public static string DataBaseSQL(string oracleDBConnection, string sql, bool needResult)
        {
            DBResult DBResult = DBSQL(oracleDBConnection, sql, needResult);

            /*while (string.Equals(DBResult.state, Variables.ERROR))
            {
                Thread.Sleep(1000);
                DBResult = DBSQL(oracleDBConnection, sql, needResult);
            }*/

            return DBResult.result;
        }

        public static DBResult DBSQL(string oracleDBConnection, string sql, bool needResult)
        {
            //Trace.TraceWrite("Execute script in DB: \n" + sql + "\n");

            DBResult DBResult = new DBResult();

            DBResult.result = String.Empty;
            DBResult.state = Variables.SUCCESS;

            OracleConnection conn = new OracleConnection(oracleDBConnection);

            try
            {
                conn.Open();
                OracleCommand dbcmd = conn.CreateCommand();
                dbcmd.CommandText = sql;
                dbcmd.CommandTimeout = 60;
                OracleDataReader reader = dbcmd.ExecuteReader();

                if (needResult)
                {
                    while (reader.Read())
                    {
                        DBResult.result = (string)reader["data"].ToString();
                    }
                }

                reader.Close();
                reader = null;

                //Trace.TraceWrite("Execution was OK\n\n");

                return DBResult;
            }

            catch (OracleException ex)
            {
                //12519 - TNS refused connection
                if (ex.Number == 12519)
                    DBResult = DBSQL(oracleDBConnection, sql, needResult);
                else
                {
                    Trace.TraceWrite("Execution was FAIL: " + ex.ToString() + "\n\n");
                    DBResult.result = "Ошибка исполнения скрипта в БД\n";
                    DBResult.state = Variables.ERROR;
                }

                return DBResult;
            }
            catch (Exception e)
            {
                Trace.TraceWrite("Execution was FAIL: " + e.ToString() + "\n\n");
                DBResult.result = "Ошибка исполнения скрипта с БД\n";
                DBResult.state = Variables.ERROR;
                return DBResult;
            }
            finally
            {
                OracleConnection.ClearPool(conn);
                conn.Dispose();
                conn.Close();
                conn = null;
            }
        }
    }
}
