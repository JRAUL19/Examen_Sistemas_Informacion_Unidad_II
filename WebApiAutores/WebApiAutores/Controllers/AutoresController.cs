using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Autores;
using WebApiAutores.Dtos.Book;
using WebApiAutores.Entities;
using WebApiAutores.Filters;

namespace WebApiAutores.Controllers
{
    [Route("api/autores")]
    [ApiController]
    //[Authorize]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IReadOnlyList<AutorDto>>>> get()
        {
            var autorDb = await _context.Autores.ToListAsync();

            var autoresDto = _mapper.Map<List<AutorDto>>(autorDb);

            return new ResponseDto<IReadOnlyList<AutorDto>>
            {
                Status = true,
                Data = autoresDto
            };
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto<AutorGetByIdDto>>> GetOneById(int id) 
        {
            var autorDb = await _context.Autores
                .Include(a => a.Books)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autorDb is null)
            {
                return NotFound(new ResponseDto<AutorDto>
                {
                    Status = false,
                    Message = $"El autor con el Id {id} no fue encontrado"
                });
            }

            var autorDto = _mapper.Map<AutorGetByIdDto>(autorDb);

            return Ok(new ResponseDto<AutorDto>
            {
                Status = true,
                Data = autorDto
            });
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreateDto dto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error en la peticion");
            }

            var autor = _mapper.Map<Autor>(dto);

            _context.Add(autor);
            await _context.SaveChangesAsync();

            var autorDto = _mapper.Map<AutorDto>(autor);

            return StatusCode(StatusCodes.Status201Created, new ResponseDto<AutorDto>
            {
                Status = true,
                Message = "Autor creado correctamente",
                Data = autorDto
            });
        }

        [HttpPut("{id:int}")] // api/autores/4
        public async Task<IActionResult> Put(int id, AutorUpdateDto dto) 
        {
            var autorDb = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autorDb is null)
            {
                return NotFound(new ResponseDto<AutorDto>
                {
                    Status = false,
                    Message = $"El autor con el Id {id} no fue encontrado"
                });
            }

            _mapper.Map<AutorUpdateDto, Autor>(dto, autorDb);

            _context.Update(autorDb);

            await _context.SaveChangesAsync();

            var autorDto = _mapper.Map<AutorDto>(autorDb);

            return Ok(new ResponseDto<AutorDto>
            {
                Status = true,
                Message = "Autor actualizado correctamente",
                Data = autorDto
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) 
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autor is null)
            {
                return NotFound(new ResponseDto<AutorDto>
                {
                    Status = false,
                    Message = $"El autor con el Id {id} no fue encontrado"
                });
            }

            _context.Remove(autor);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<string>
            {
                Status = true,
                Message = "autor borrado correctamente"
            });
        }
    }
}
