namespace FreelancerStudent.API.DTOs.Request
{
    public class SePayWebhook_RequestDTO
    {
        public long id { get; set; }

        public string gateway { get; set; } = string.Empty;

        public string transactionDate { get; set; } = string.Empty;

        public string accountNumber { get; set; } = string.Empty;

        public string subAccount { get; set; } = string.Empty;

        public string? code { get; set; }

        public string content { get; set; } = string.Empty;

        public string transferType { get; set; } = string.Empty;

        public decimal transferAmount { get; set; }

        public decimal accumulated { get; set; }

        public string description { get; set; } = string.Empty;

        public string referenceCode { get; set; } = string.Empty;
    }
}