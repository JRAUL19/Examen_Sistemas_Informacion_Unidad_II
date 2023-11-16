using AutoMapper;
using WebApiAutores.Dtos.Autores;
using WebApiAutores.Dtos.Book;
using WebApiAutores.Dtos.Comentarios;
using WebApiAutores.Dtos.Reviews;
using WebApiAutores.Entities;

namespace WebApiAutores.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            MapsForBooks();
            MapsForAutores();
            MapsForComentarios();
            MapsForReviews();
        }

        private void MapsForAutores()
        {
            CreateMap<Autor, AutorDto>();
            CreateMap<Autor, AutorGetByIdDto>();
            CreateMap<AutorCreateDto, Autor>();
        }

        private void MapsForReviews()
        {
            CreateMap<ReviewCreateDto, Review>();
            CreateMap<Review, ReviewDto>();
            CreateMap<Review, ReviewDto>()
                .ForPath(dest => dest.BookName, opt => opt.MapFrom(src => src.Book.Title));
        }

        private void MapsForComentarios()
        {
            CreateMap<ComentariosCreateDto, Comentarios>();
            CreateMap<Comentarios, ComentariosDto>();
        }

        private void MapsForBooks()
        {
            //CreateMap<BookDto, Book>().ReverseMap();

            CreateMap<Book, BookDto>()
                .ForPath(dest => dest.AutorNombre, opt => opt.MapFrom(src => src.Autor.Name));

            CreateMap<BookCreateDto, Book>();
        }


    }
}

