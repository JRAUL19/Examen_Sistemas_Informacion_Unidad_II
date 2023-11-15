using WebApiAutores.Dtos.Book;

namespace WebApiAutores.Dtos.Autores
{
    public class AutorGetByIdDto : AutorDto
    {
        public virtual IEnumerable<BookDto> Books { get; set; }
    }
}
