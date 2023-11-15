using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entities
{
    [Table("reviews")]
    public class Review
    {
        [Column("id")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("puntuacion")]
        [Required]
        public int Puntuacion { get; set; }

        [Column("comentario")]
        [StringLength(200)]
        [Required]
        public string Comentario { get; set; }

        [Column("promedio")]
        public decimal Promedio { get; set; }

        [Column("usuario")]
        public string Usuario { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("book_id")]
        public Guid BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; }
    }
}
