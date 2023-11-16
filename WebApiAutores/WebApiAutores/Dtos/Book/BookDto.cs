using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Entities;

namespace WebApiAutores.Dtos.Book
{
    public class BookDto
    {
        public Guid Id { get; set; }

        public string ISBN { get; set; }

        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }

        public int AutorId { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)]
        public double Valoracion { get; set; }

        public string AutorNombre { get; set; }

    }
}
