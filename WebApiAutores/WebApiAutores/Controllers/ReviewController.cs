using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dtos.Book;
using WebApiAutores.Dtos;
using WebApiAutores.Entities;
using WebApiAutores.Dtos.Reviews;
using System.Security.Claims;

namespace WebApiAutores.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        //Configuraciones
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public ReviewController(
            ApplicationDbContext context,
            IMapper mapper,
            IHttpContextAccessor contextAccessor)
        {
            this._context = context;
            this._mapper = mapper;
            this._contextAccessor = contextAccessor;
        }

        //Crear Review
        [HttpPost]
        public async Task<ActionResult> Post(ReviewCreateDto dto)
        {
            //Validar si el libro existe
            if (!ModelState.IsValid)
            {
                return BadRequest("Error en la peticion");
            }

            var bookExiste = await _context.Books
                .AnyAsync(x => x.Id == dto.BookId);

            if (!bookExiste)
            {
                return base.NotFound(new ResponseDto<ReviewDto>
                {
                    Status = false,
                    Message = $"No existe el libro: {dto.BookId}"
                });
            }

            //Calculo del promedio de valoraciones para libro
            var book = await _context.Books.FindAsync(dto.BookId);

            if (book != null)
            {
                var reviewsValues = await _context.Reviews
                    .Where(r => r.BookId == dto.BookId)
                    .Select(r => r.Puntuacion)
                    .ToListAsync();

                if (reviewsValues.Any())
                {
                    double promedio = reviewsValues.Average();
                    book.Valoracion = Math.Round(promedio, 1);
                }
            }

            //Identificar lenguaje ofesivo en comentario de review
            if (ContienePalabrasOfensivas(dto.Comentario))
            {
                return BadRequest("El comentario contiene palabras ofensivas. Por favor, modifícalo.");
            }

            //Mapeo de datos
            var userEmail = HttpContext.User.Claims.FirstOrDefault
                (c => c.Type == ClaimTypes.Email)?.Value;

            var review = _mapper.Map<Review>(dto);

            review.Fecha = DateTime.Now;
            review.Usuario = userEmail;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            //Devolver datos
            var reviewDto = _mapper.Map<ReviewDto>(review);

            return base.StatusCode(StatusCodes.Status201Created, new ResponseDto<ReviewDto>
            {
                Status = true,
                Message = "Review creada exitosamente",
                Data = reviewDto
            });
        }


        //Obtener Reviews
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<IReadOnlyList<ReviewDto>>>> get()
        {
            var reviewDb = await _context.Reviews
                .Include(b => b.Book)
                .ToListAsync();

            var reviewDto = _mapper.Map<List<ReviewDto>>(reviewDb);

            return new ResponseDto<IReadOnlyList<ReviewDto>>
            {
                Status = true,
                Data = reviewDto
            };
        }


        //Obtener Reviews por Id
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<ReviewDto>>> Get(int id)
        {
            var reviewDb = await _context.Reviews
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (reviewDb is null)
            {
                return NotFound(new ResponseDto<ReviewDto>
                {
                    Status = false,
                    Message = $"La review con id: {id} no fue encontrado"
                });
            }

            var reviewDto = _mapper.Map<ReviewDto>(reviewDb);

            return Ok(new ResponseDto<ReviewDto>
            {
                Status = true,
                Data = reviewDto
            });
        }

        //Editar Review
        [HttpPut("{id:int}")]
        
        public async Task<ActionResult<ResponseDto<ReviewDto>>> Put(ReviewUpdateDto dto, int id)
        {
            //Verificar que review existe
            var reviewDB = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);

            if (reviewDB is null)
            {
                return NotFound(new ResponseDto<ReviewDto>
                {
                    Status = false,
                    Message = $"No existe la review: {id}"
                });
            }

            var bookExiste = await _context.Books
                .AnyAsync(x => x.Id == dto.BookId);

            if (!bookExiste)
            {
                return NotFound(new ResponseDto<ReviewDto>
                {
                    Status = false,
                    Message = $"No existe el lirbo: {dto.BookId}"
                });
            }

            //Identificar lenguaje ofesivo en comentario de review
            if (ContienePalabrasOfensivas(dto.Comentario))
            {
                return BadRequest("El comentario contiene palabras ofensivas. Por favor, modifícalo.");
            }
            //Mapeo de datos
            var userEmail = HttpContext.User.Claims.FirstOrDefault
                (c => c.Type == ClaimTypes.Email)?.Value;

            //Mapeo de datos
            var review = _mapper.Map<ReviewUpdateDto, Review>(dto, reviewDB);
            _context.Update(reviewDB);

            review.Fecha = DateTime.Now;
            review.Usuario = userEmail;

            await _context.SaveChangesAsync();
            var reviewDto = _mapper.Map<ReviewDto>(reviewDB);

            //Calcular promedios
            await RecalcularAvgReview(dto.BookId);

            return Ok(new ResponseDto<ReviewDto>
            {
                Status = true,
                Message = "Datos actualizados correctamente",
                Data = reviewDto
            });
        }


        //Eliminar Review
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto<string>>> Delete(int id)
        {
            var reviewExist = await _context.Reviews.AnyAsync(x => x.Id == id);

            if (!reviewExist)
            {
                return NotFound(new ResponseDto<ReviewDto>
                {
                    Status = false,
                    Message = $"La review: {id} no existe"
                });
            }

            _context.Remove(new Review { Id = id });
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<string>
            {
                Status = true,
                Message = "Review eliminada correctamente"
            });
        }


            //Calculo de promedio
            private async Task RecalcularAvgReview(Guid bookId)
            {
                var reviews = await _context.Reviews
                    .Where(r => r.BookId == bookId)
                    .Select(r => r.Puntuacion)
                    .ToListAsync();

                var book = await _context.Books.FindAsync(bookId);

                if (book != null)
                {
                    if (reviews.Any())
                    {
                        double promedio = reviews.Average();
                        book.Valoracion = Math.Round(promedio, 1);
                    }
                    else
                    {
                        book.Valoracion = 0;
                    }

                    await _context.SaveChangesAsync();
                }
            }

        //Validacion de palabras ofensivas 
        private bool ContienePalabrasOfensivas(string contenido)
        {
            var palabrasOfensivas = new string[] { "idiota", "lerdo", "mameluco", "mentecato", "pazguato",
                "imbécil", "retrasado", "estúpido", "loco", "subnormal", "deficiente", "cenutrio", "zoquete",
                "analfabeto", "ignorante", "sinvergüenza", "ladrón"};

            foreach (var palabra in palabrasOfensivas)
            {
                if (contenido != null && contenido.Contains(palabra, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
