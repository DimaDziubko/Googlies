namespace _Game.Core.Services.IAP.Data
{
    public class PayloadAndroidData
    {
        public string json;
        public string signature;

        public PayloadAndroidData()
        {
            json = signature = "";
        }

        public PayloadAndroidData(string _json, string _signature)
        {
            json = _json;
            signature = _signature;
        }
    }
}
