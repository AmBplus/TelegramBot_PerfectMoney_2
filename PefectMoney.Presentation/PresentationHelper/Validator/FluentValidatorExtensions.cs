using FluentValidation.Results;
using System.Runtime.CompilerServices;
using System.Text;

namespace PefectMoney.Presentation.PresentationHelper.Validator
{
    public static  class FluentValidatorExtensions
    {

        public static string ToStringErrors(this List<ValidationFailure> validationFailures)
        {
            StringBuilder sb = new StringBuilder(); 
            foreach (var item in validationFailures)
            {
                sb.AppendLine(item.ErrorMessage);
            }
            return sb.ToString();
        }
    }
}
