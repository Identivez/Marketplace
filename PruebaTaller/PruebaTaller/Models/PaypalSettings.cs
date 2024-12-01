namespace GEJ_Lab.Models
{
    public class PayPalSettings
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Environment { get; set; }
        public string PaymentUrl { get; set; }
        public string TokenUrl { get; set; }
    }
}
