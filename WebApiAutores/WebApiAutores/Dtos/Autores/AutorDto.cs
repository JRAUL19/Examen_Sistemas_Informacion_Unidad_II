using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Autores
{
    public class AutorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
