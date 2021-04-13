namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class ApiResponse<T>
    {
        public ApiResponse()
        {
            return_code = ApiResponseCode.Failed;
        }
        public ApiResponseCode return_code { get; set; }
        public string return_msg { get; set; }
        public T data { get; set; }
    }
}
