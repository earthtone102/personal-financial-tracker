namespace PersonalFinancialTracker.Helper
{
    public class BaseResponse
    {
        public OperationEnum.RequestResult? Acknowledge { get; set; }
        public string? Message {  get; set; }
        public string? ErrorMessage {  get; set; }
    }
}
