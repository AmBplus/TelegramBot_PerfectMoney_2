using FluentValidation;

namespace PefectMoney.Presentation.PresentationHelper.Validator
{
    public record ValidateCountBuyVoicherTxt(string txt);
    public class ValidateCountBuyVoicher : AbstractValidator<ValidateCountBuyVoicherTxt>
    {
        public ValidateCountBuyVoicher()
        {
            RuleFor(c => c.txt)
                .NotEmpty().WithMessage("تعداد نمیتواند خالی باشد")
                .Must(BeNumeric).WithMessage("باید عدد باشد");
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
