using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record ProductDto
        (
        int Id,
        [Required] string Name,
        [Required, Range(1, int.MaxValue)] int Quantity,
        [Required, DataType(DataType.Currency)] decimal Price
        );

}
