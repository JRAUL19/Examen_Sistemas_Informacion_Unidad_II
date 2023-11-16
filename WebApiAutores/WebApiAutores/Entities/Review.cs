using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entities
{
    [Table("reviews", Schema = "transaccional")]
    public class Review
    {
        [Column("id")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("puntuacion")]
        [Required]
        public double Puntuacion { get; set; }

        [Column("comentario")]
        [StringLength(200)]
        [Required]
        public string Comentario { get; set; }


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
