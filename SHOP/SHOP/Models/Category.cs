using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHOP.Models
{
    [Table("Categoria")]
    public class Category
    {
        [Key]
        public int Id { set; get; }

        [Required(ErrorMessage = "Esse campo é obrigatório.")]
        [MaxLength(60, ErrorMessage = "Esse campo deve conter de 3 a 60 caracteres.")]
        [MinLength(3, ErrorMessage = "Esse campo deve conter de 3 a 60 caracteres.")]
        public string Title { set; get; }
    }
}
