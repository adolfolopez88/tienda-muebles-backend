using FluentValidation;
using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Validators;

public class CreateCategoriaValidator : AbstractValidator<CreateCategoriaRequest>
{
    public CreateCategoriaValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
