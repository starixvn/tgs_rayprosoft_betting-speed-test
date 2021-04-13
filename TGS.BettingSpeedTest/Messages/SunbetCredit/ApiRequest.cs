namespace TGS.BettingSpeedTest.Messages.SunbetCredit
{
    public class ApiRequest<T>
    {
        public string app_id { get; set; }
        public string app_secret { get; set; }
        public T data { get; set; }
    }
}
