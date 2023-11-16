using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Comentarios
{
    public class ComentariosCreateDto
    {
        [Required]
        public string Comentario { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public int ValoracionId { get; set; }
        public int? ComentarioId { get; set; }

    }
}
