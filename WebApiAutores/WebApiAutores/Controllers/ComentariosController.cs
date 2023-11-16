using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Comentarios;
using WebApiAutores.Entities;
using WebApiAutores.Services;

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

            var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Asignar el email del usuario al campo "Usuario" del DTO
            dto.Usuario = userEmail;

            var comentario = _mapper.Map<Comentarios>(dto);

            // Verifica si el comentario tiene un comentario secundario asociado
            if (dto.ComentarioId.HasValue)
            {
                var comentarioPrincipal = await _context.Comentarios.FindAsync(dto.ComentarioId.Value);

                // Si el comentario padre existe, asócialo
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


    }
}
