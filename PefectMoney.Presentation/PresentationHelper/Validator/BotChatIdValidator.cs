using FluentValidation;

namespace PefectMoney.Presentation.PresentationHelper.Validator
{
    public record BotChatIdDto(string BotChatId);
    public class BotChatIdValidator : AbstractValidator<BotChatIdDto>
    {
        public BotChatIdValidator()
        {
            RuleFor(c => c.BotChatId)
                .NotEmpty().WithMessage("شماره بات آیدی نمی تواند خالی باشد")
                .Must(BeNumeric).WithMessage("شماره بات آیدی فقط میتواند شامل اعداد باشد");
        }

        private bool BeNumeric(string cardNumber)
        {
            foreach (char c in cardNumber)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
