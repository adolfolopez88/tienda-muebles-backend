using FluentValidation;
using TiendaMuebles.Application.DTOs;

namespace TiendaMuebles.Application.Validators;

public class CreatePedidoValidator : AbstractValidator<CreatePedidoRequest>
{
    public CreatePedidoValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Nombre).NotEmpty();
            item.RuleFor(i => i.Cantidad).GreaterThan(0);
            item.RuleFor(i => i.PrecioUnitario).GreaterThanOrEqualTo(0);
        });
        RuleFor(x => x.DatosEnvio).NotNull();
        RuleFor(x => x.DatosEnvio.Nombre).NotEmpty();
        RuleFor(x => x.DatosEnvio.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Subtotal).GreaterThanOrEqualTo(0);
    }
}

public class UpdateEstadoValidator : AbstractValidator<UpdateEstadoRequest>
{
    private static readonly string[] ValidStates =
        { "Pendiente", "Procesando", "Enviado", "Entregado", "Cancelado" };

    public UpdateEstadoValidator()
    {
        RuleFor(x => x.Estado).NotEmpty().Must(s => ValidStates.Contains(s))
            .WithMessage($"Estado debe ser: {string.Join(", ", ValidStates)}");
    }
}
