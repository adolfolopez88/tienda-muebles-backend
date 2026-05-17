using FluentValidation;
using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Validators;

public class CreateProductoValidator : AbstractValidator<CreateProductoRequest>
{
    public CreateProductoValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Precio).GreaterThan(0);
        RuleFor(x => x.Categoria).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductoValidator : AbstractValidator<UpdateProductoRequest>
{
    public UpdateProductoValidator()
    {
        RuleFor(x => x.Precio).GreaterThan(0).When(x => x.Precio.HasValue);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).When(x => x.Stock.HasValue);
        RuleFor(x => x.Nombre).MaximumLength(200).When(x => x.Nombre is not null);
        RuleFor(x => x.Slug).MaximumLength(200).When(x => x.Slug is not null);
        RuleFor(x => x.Categoria).MaximumLength(100).When(x => x.Categoria is not null);
    }
}
