namespace PefectMoney.Core.UseCase.ZibalPayment
{
    public record OrderDto
    {
        public DateTime CreationTime { get; set; }
        public long Id { get; set; }
        public string trackId { get; set; }
        public double TotalDollarPrice { get; set; }
        public double TotalRialsPrice { get; set; }

        public int OrderStatus { get; set; }
        public int Count { get; set; }
      
        public long ProductId { get; set; }
        public long BotChatId { get; set; }

        public int Rial { get; set; }
        public double Dollar { get; set; }
    }
}
