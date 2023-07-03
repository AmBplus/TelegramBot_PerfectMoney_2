using FluentValidation;

namespace PefectMoney.Presentation.PresentationHelper.Validator
{
 
    public record CardNumberPresentaionDto {
        public string CardNumber { get; set; }
    }
    public class CardNumberValidator : AbstractValidator<CardNumberPresentaionDto>
    {
        public CardNumberValidator()
        {
            RuleFor(c => c.CardNumber)
                .NotEmpty().WithMessage("شماره کارت نمی تواند خالی باشد")
                .Length(16).WithMessage("شماره کارت باید 16 رقم باشد")
                .Must(BeNumeric).WithMessage("شماره کارت فقط میتواند شامل اعداد باشد");
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
