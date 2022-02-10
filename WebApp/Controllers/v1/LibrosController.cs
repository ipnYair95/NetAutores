﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTOs;
using WebApp.Entity;

namespace WebApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro = await context.Libros
                .Include( libroDb => libroDb.AutoresLibros )
                .ThenInclude( autorLibroDb => autorLibroDb.Autor )
                .FirstOrDefaultAsync(x => x.Id == id);

            if ( libro == null )
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy( x => x.Orden ).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost(Name = "crearLibro" )]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if ( libroCreacionDTO.AutoresId == null )
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await context.Autores
                    .Where(autorDb => libroCreacionDTO.AutoresId.Contains(autorDb.Id))
                    .Select(x => x.Id)
                    .ToListAsync();

            if ( libroCreacionDTO.AutoresId.Count != autoresIds.Count )
            {
                return BadRequest("No existe alguno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);

            AsignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);

        }

        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDb = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync( x => x.Id == id );

            if ( libroDb == null )
            {
                return NotFound();
            }

            libroDb = mapper.Map( libroCreacionDTO, libroDb );

            AsignarOrdenAutores(libroDb);

            await context.SaveChangesAsync();

            return NoContent();

        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {

                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }

            }
        }

        [HttpPatch("{id:int}", Name = "patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument )
        {
            if ( patchDocument == null )
            {
                return BadRequest();
            }

            var libroDb = await context.Libros.FirstOrDefaultAsync( x => x.Id == id );

            if ( libroDb == null )
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDb);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if ( !esValido )
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDb);

            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id:int}", Name = "borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }


    }
}
