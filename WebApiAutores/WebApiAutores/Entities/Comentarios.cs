using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace WebApiAutores.Entities
{
    [Table("comentarios", Schema = "transaccional")]
    public class Comentarios
    {
        [Column("id")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("comentario")]
        [StringLength(200)]
        [Required]
        public string Comentario { get; set; }

        [Column("usuario")]
        public string Usuario { get; set; }

        [Column("fecha")]
        [Required]
        public DateTime Fecha { get; set; }

        [Column("valoracion_id")]
        public int ValoracionId { get; set; }

        [Column("comentario_id")]
        public int? ComentarioId { get; set; } 

        [ForeignKey(nameof(ValoracionId))]
        public virtual Review fk_Review { get; set; }

        [ForeignKey(nameof(ComentarioId))]
        public virtual Comentarios fk_Comentarios { get; set; }
    }
}
