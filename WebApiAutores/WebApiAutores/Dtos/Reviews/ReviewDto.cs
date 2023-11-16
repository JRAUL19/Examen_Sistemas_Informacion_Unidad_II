using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Puntuacion { get; set; }
        public string Comentario { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public Guid BookId { get; set; }
        public string BookName { get; set; }
    }
}
