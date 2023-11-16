using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Reviews
{
    public class ReviewCreateDto
    {
        [Display(Name = "puntuacion")]
        [Required(ErrorMessage = "La {0} es requerida")]
        [Range(0, 5, ErrorMessage = "La {0} debe estar en el rango de 0 a 5")]
        public double Puntuacion { get; set; }

        [Display(Name = "comentario")]
        [StringLength(200)]
        [Required(ErrorMessage = "El {0} es requerido")]
        public string Comentario { get; set; }

        [Display(Name = "book id")]
        [Required(ErrorMessage = "El {0} es requerido")]
        public Guid BookId { get; set; }
    }
}
