namespace ScavengeRUs.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? Text { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}