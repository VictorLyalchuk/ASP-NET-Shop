using DataAccess.Entities;
using FluentValidation;


namespace DataAccess.Validation
{
    public class ProductValidator:AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(product => product.Name)
                .NotNull()
                .NotEmpty()
                .MinimumLength(2)
                .WithMessage("Value {PropertyValue} is incorrect. {PropertyName} is required and must be len greater 2... ");

            RuleFor(product => product.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Value {PropertyValue} of property {PropertyName} is incorrect.");

            RuleFor(product => product.ImagePath)
                .Must(LinkMustBeAUri)
                .WithMessage("{PropertyName} has incorrect URL format");
        }

        private static bool LinkMustBeAUri(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return false;
            }

            Uri result;
            return Uri.TryCreate(link, UriKind.Absolute, out result);
        }
    }
}
