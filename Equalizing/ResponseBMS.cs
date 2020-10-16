namespace Equalizing
{
    public class ResponseBMS
    {
        public BpsResponse BpsResponse { get; set; }
    }

    public class BpsResponse
    {
        public string state { get; set; }
        public string stateCode { get; set; }
        public string message { get; set; }
    }
}
