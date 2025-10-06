
namespace _Game.Core.Services.IAP.Data
{
    public class ReceiptData
    {

        public string Store;
        public string TransactionID;
        public string Payload;

        public ReceiptData()
        {
            Store = TransactionID = Payload = "";
        }

        public ReceiptData(string store, string transactionID, string payload)
        {
            Store = store;
            TransactionID = transactionID;
            Payload = payload;
        }
    }
}
