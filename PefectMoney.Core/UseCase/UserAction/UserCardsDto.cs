namespace PefectMoney.Core.UseCase.UserAction
{
    public record UserCardsDto(string CardNumber)
    {
       public long Id { get; set; }
    }
}
