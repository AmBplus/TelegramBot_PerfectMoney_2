using System.Net;

namespace PefectMoney.Core.UseCase.ZibalPayment;

class Zibal
{
    public static HttpWebResponse HttpRequestToZibal(string url, string data)
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url); // make request
        httpWebRequest.ContentType = "application/json; charset=utf-8"; // content of request -> must be JSON
        httpWebRequest.Method = "POST"; // method of request -> must be POST
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            streamWriter.Write(data); // send request
            streamWriter.Flush(); // flush stream
        }
        return (HttpWebResponse)httpWebRequest.GetResponse(); // get Response
    }

    public class makeRequest
    {
        public string merchant { get; set; }
        public string orderId { get; set; }
        public int amount { get; set; }
        public string callbackUrl { get; set; }
        public string description { get; set; }
        public string mobile { get; set; }
        public string[] allowedCards { get; set; }
    }


    public class makeRequest_response
    {
        public string trackId { get; set; }
        public string result { get; set; }
        public string message { get; set; }
    }

    public class verifyRequest
    {
        public string merchant { get; set; }
        public string trackId { get; set; }
    }

    public class verifyResponse
    {
        public string paidAt { get; set; }
        public string createdAt { get; set; }
        public string cardNumber { get; set; }
        public string refNumber { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string wage { get; set; }
        public string amount { get; set; }
        public string result { get; set; }
        public string message { get; set; }
    }


}


//class Request
//{
//    static void Main(string[] args)
//    {
//        try
//        {
//            string url = "https://gateway.zibal.ir/v1/request"; // url
//            Zibal.makeRequest Request = new Zibal.makeRequest(); // define Request
//            Request.merchant = "zibal"; // String
//            Request.orderId = "1000"; // String
//            Request.amount = 1000; //Integer
//            Request.callbackUrl = "http://callback.com/api"; //String
//            Request.description = "Hello Zibal !"; // String
//            var httpResponse = Zibal.HttpRequestToZibal(url, JsonConvert.SerializeObject(Request));  // get Response
//            using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) // make stream reader
//            {
//                var responseText = streamReader.ReadToEnd(); // read Response
//                Zibal.makeRequest_response item = JsonConvert.DeserializeObject<Zibal.makeRequest_response>(responseText); // Deserilize as response class object
//                                                                                                                           // you can access track id with item.trackId , result with item.result and message with item.message
//                                                                                                                           // in asp.net you can use Response.Redirect("https://gateway.zibal.ir/start/item.trackId"); for start gateway and redirect to third-party gateway page                                                                                                          // also you can use Response.Redirect("https://gateway.zibal.ir/start/item.trackId/direct"); for start gateway page directly
//            }
//        }
//        catch (WebException ex)
//        {
//            Console.WriteLine(ex.Message); // print exception error
//        }
//    }
//}


