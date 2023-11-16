using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Comentarios;
using WebApiAutores.Entities;

namespace WebApiAutores.Controllers
{
    [Route("api/comentarios")]
    [ApiController]
    [Authorize]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ComentariosController(ApplicationDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        //obtener todos los comentarios
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<IReadOnlyList<ComentariosDto>>>> Get()
        {
            var comentariosDb = await _context.Comentarios.ToListAsync();

            var comentariosDto = _mapper.Map<List<ComentariosDto>>(comentariosDb);

            return new ResponseDto<IReadOnlyList<ComentariosDto>>
            {
                Status = true,
                Data = comentariosDto.AsReadOnly()
            };
        }

        //Obtener por id
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<ComentariosDto>>> GetById(int id)
        {
            var comentarioDb = await _context.Comentarios
                .FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDb is null)
            {
                return NotFound(new ResponseDto<ComentariosDto>
                {
                    Status = false,
                    Message = $"El Comentario con el Id {id} no fue encontrado"
                });
            }

            var comentarioDto = _mapper.Map<ComentariosDto>(comentarioDb);

            return Ok(new ResponseDto<ComentariosDto>
            {
                Status = true,
                Data = comentarioDto
            });
        }

        //Post de comentarios
        [HttpPost]
        public async Task<ActionResult> Post(ComentariosCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error en la petición");
            }

            //Obtener el email
            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Asignar el email campo "Usuario"
            dto.Usuario = userEmail;

            //validar palabras ofensivas
            if (ContienePalabrasOfensivas(dto.Comentario))
            {
                return BadRequest("El comentario contiene palabras ofensivas. Por favor, modifícalo.");
            }

            var comentario = _mapper.Map<Comentarios>(dto);

            if (dto.ComentarioId.HasValue)
            {
                var comentarioPrincipal = await _context.Comentarios.FindAsync(dto.ComentarioId.Value);

                if (comentarioPrincipal != null)
                {
                    comentario.fk_Comentarios = comentarioPrincipal;
                }
                else
                {
                    return BadRequest("El comentario no existe.");
                }
            }

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            var comentarioDto = _mapper.Map<ComentariosDto>(comentario);

            return StatusCode(StatusCodes.Status201Created, new ResponseDto<ComentariosDto>
            {
                Status = true,
                Message = "Comentario creado correctamente",
                Data = comentarioDto
            });
        }

        //Editar comentario
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<ComentariosDto>>> Put(ComentariosUpdateDto dto, int id)
        {
            var comentarioDb = await _context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDb is null)
            {
                return NotFound(new ResponseDto<ComentariosDto>
                {
                    Status = false,
                    Message = $"No existe el comentario: {id}"
                });
            }

            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            dto.Usuario = userEmail;

            //validar palabras ofensivas
            if (ContienePalabrasOfensivas(dto.Comentario))
            {
                return BadRequest("El comentario contiene palabras ofensivas. Por favor, modifícalo.");
            }

            var valoracionExiste = await _context.Autores
                .AnyAsync(x => x.Id == dto.ValoracionId);

            if (!valoracionExiste)
            {
                return NotFound(new ResponseDto<ComentariosDto>
                {
                    Status = false,
                    Message = $"No existe la Review: {dto.ValoracionId}"
                });
            }

            _mapper.Map<ComentariosUpdateDto, Comentarios>(dto, comentarioDb);

            _context.Update(comentarioDb);

            await _context.SaveChangesAsync();

            var comentarioDto = _mapper.Map<ComentariosDto>(comentarioDb);
            return StatusCode(StatusCodes.Status202Accepted, new ResponseDto<ComentariosDto>
            {
                Status = true,
                Message = "Comentario Actualizado correctamente",
                Data = comentarioDto
            });
        }

        //Borrar comentarios
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comentario = await _context.Comentarios.FirstOrDefaultAsync(a => a.Id == id);
            if (comentario is null)
            {
                return NotFound(new ResponseDto<ComentariosDto>
                {
                    Status = false,
                    Message = $"El comentario con el Id {id} no fue encontrado"
                });
            }

            _context.Remove(comentario);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<string>
            {
                Status = true,
                Message = "comentario borrado correctamente"
            });
        }

        // validacion de palabras ofensivas 
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
