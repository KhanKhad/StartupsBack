using StartupsBack.Models.DbModels;

namespace StartupsBack.Models.JsonModels
{
    public class MessageJsonModel
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;

    }
}
