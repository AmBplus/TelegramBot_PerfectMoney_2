using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.PerfectMoney
{

    public class PerfectmoneyModel
    {
        private string apiUrl = "https://perfectmoney.is/acct/";
        private string AccountID = "";
        private string PassPhrase = "";
        private string PayerAccount = "";
        public string errorMsg = "z";

        public PerfectmoneyModel(string acc_id, string pw, string pay_acc)
        {
            AccountID = acc_id;
            PassPhrase = pw;
            PayerAccount = pay_acc;
        }

        public string GetBalance()
        {
            try
            {
                string Balance = "";
                string url = apiUrl + "balance.asp?AccountID=" + AccountID + "&PassPhrase=" + PassPhrase;
                string result = sendCurl("GET", url);
                if (result == "")
                {
                    throw new Exception("Empty Response from PerfectMoney Server");
                }
                foreach (Match match in Regex.Matches(result, "<input name='(.*?)' type='hidden' value='(.*?)'>", RegexOptions.IgnoreCase))
                {
                    if (match.Groups[1].Value == "ERROR")
                    {
                        throw new Exception(match.Groups[2].Value);
                    }
                    if (match.Groups[1].Value == PayerAccount)
                    {
                        Balance = match.Groups[2].Value;
                    }
                }
                if (Balance != "")
                {
                    return "Balance : " + Balance;
                }
                throw new Exception("Response Not valid (it should contain error or voucher details) : " + result);
            }
            catch (Exception ex)
            {
                errorMsg = "Error : " + ex.Message;
            }
            return "";
        }


        public ResultOperation CreateVoucher(double amount)
        {
            try
            {
                string VOUCHER_NUM = "";
                string VOUCHER_CODE = "";
                string PAYMENT_BATCH_NUM = "";
                string url = apiUrl + "ev_create.asp?AccountID=" + AccountID + "&PassPhrase=" + PassPhrase + "&Payer_Account=" + PayerAccount + "&Amount=" + amount;
                string result = sendCurl("GET", url);
                if (result == "")
                {
                    return ResultOperation.ToFailedResult("Empty Response from PerfectMoney Server");
                }
                foreach (Match match in Regex.Matches(result, "<input name='(.*?)' type='hidden' value='(.*?)'>", RegexOptions.IgnoreCase))
                {
                    if (match.Groups[1].Value == "ERROR")
                    {
                        return ResultOperation.ToFailedResult(match.Groups[2].Value);
                    }
                    if (match.Groups[1].Value == "VOUCHER_NUM")
                    {
                        VOUCHER_NUM = match.Groups[2].Value;
                    }
                    if (match.Groups[1].Value == "VOUCHER_CODE")
                    {
                        VOUCHER_CODE = match.Groups[2].Value;
                    }
                    if (match.Groups[1].Value == "PAYMENT_BATCH_NUM")
                    {
                        PAYMENT_BATCH_NUM = match.Groups[2].Value;
                    }
                }
                if (VOUCHER_NUM != "" && VOUCHER_CODE != "" && PAYMENT_BATCH_NUM != "")
                {
                    return ResultOperation.ToSuccessResult("Serial : " + VOUCHER_NUM + " - ActivationCode : " + VOUCHER_CODE + " - ReferenceNumber : " + PAYMENT_BATCH_NUM);
                }
                return ResultOperation.ToFailedResult("Response Not valid (it should contain error or voucher details) : " + result);
                
            }
            catch (Exception ex)
            {
                errorMsg = "Error : " + ex.Message;
                return ResultOperation.ToFailedResult(errorMsg);
            }
           
        }

        public string RedeemVoucher(string ev_number, string ev_code)
        {
            try
            {
                string VOUCHER_NUM = "";
                string VOUCHER_AMOUNT = "";
                string Payee_Account = "";
                string PAYMENT_BATCH_NUM = "";
                string url = apiUrl + "ev_activate.asp?AccountID=" + AccountID + "&PassPhrase=" + PassPhrase + "&Payee_Account=" + PayerAccount + "&ev_number=" + ev_number + "&ev_code=" + ev_code;
                string result = sendCurl("GET", url);
                if (result == "")
                {
                    throw new Exception("Empty Response from PerfectMoney Server");
                }
                foreach (Match match in Regex.Matches(result, "<input name='(.*?)' type='hidden' value='(.*?)'>", RegexOptions.IgnoreCase))
                {
                    if (match.Groups[1].Value == "ERROR")
                    {
                        throw new Exception(match.Groups[2].Value);
                    }
                    if (match.Groups[1].Value == "VOUCHER_NUM")
                    {
                        VOUCHER_NUM = match.Groups[2].Value;
                    }
                    if (match.Groups[1].Value == "VOUCHER_AMOUNT")
                    {
                        VOUCHER_AMOUNT = match.Groups[2].Value;
                    }
                    if (match.Groups[1].Value == "Payee_Account")
                    {
                        Payee_Account = match.Groups[2].Value;
                    }
                    if (match.Groups[1].Value == "PAYMENT_BATCH_NUM")
                    {
                        PAYMENT_BATCH_NUM = match.Groups[2].Value;
                    }
                }
                if (VOUCHER_NUM != "" && VOUCHER_AMOUNT != "" && Payee_Account != "" && PAYMENT_BATCH_NUM != "")
                {
                    return "VOUCHER_AMOUNT : " + VOUCHER_AMOUNT + " - ReferenceNumber : " + PAYMENT_BATCH_NUM + " - VOUCHER_NUM : " + VOUCHER_NUM + " - Payee_Account : " + Payee_Account;
                }
                throw new Exception("Response Not valid (it should contain error or voucher details) : " + result);
            }
            catch (Exception ex)
            {
                errorMsg = "Error : " + ex.Message;
            }
            return "";
        }

        public string sendCurl(string Method, string Address)
        {
            return sendCurl(Method, Address, "");
        }
        public string sendCurl(string Method, string Address, string PostData)
        {
            try
            {
                System.IO.Stream dataStream;
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Address);
                request.Method = Method;
                if (PostData != String.Empty)
                {
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(PostData);
                    request.ContentLength = byteArray.Length;
                    dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                System.Net.WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                string Result = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                return Result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

}
