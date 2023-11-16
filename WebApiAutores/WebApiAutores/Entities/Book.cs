
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entities
{
    [Table("books", Schema = "transaccional")]
    public class Book
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("isbn")]
        [StringLength(50)]
        [Required]
        public string ISBN { get; set; }

        [Column("title")]
        [StringLength(50)]
        [Required]
        public string Title { get; set; }

        [Column("publication_date")]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [Column("autor_id")]
        public int AutorId { get; set; }

        [Column("valoracion")]
        
        public double Valoracion {  get; set; }

        [ForeignKey(nameof(AutorId))]
        public virtual Autor Autor { get; set; }

    }
}
