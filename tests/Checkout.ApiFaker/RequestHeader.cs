namespace Checkout.ApiFaker
{
    public class RequestHeader
    {
        public RequestHeader(string key, string[] value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public string[] Value { get; set; }
    }
}