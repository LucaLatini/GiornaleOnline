using GiornaleOnline.DataContext;
using GiornaleOnline.DataContext.Models;
using GiornaleOnline.Extensions;
using GiornaleOnline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GiornaleOnline.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategorieController : ControllerBase
    {
        private readonly ILogger<CategorieController> _logger;
        private readonly GOContext _dc;

        public CategorieController(ILogger<CategorieController> logger, GOContext dc)
        {
            _logger = logger;
            _dc = dc;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaModel>>> GetAll()
        {
            _logger.LogInformation("GetById");
            var data = await _dc.Categorie.Select(s => s.ToCategoriaModel()).ToListAsync();

            return Ok(data);
        }


        [HttpGet("{id}")] //nelle graffe specifico i parametri se non trovo nessun id eseguo la funzione sopra qualsiasi cosa ci sia dopo Categorie/ diventa argomento della funzione
        public async Task<ActionResult<CategoriaModel>> GetById(int id)
        {
            var categoria = await _dc.Categorie.FindAsync(id);
            if (categoria == null)
            {
                return NotFound(); //status code 404
            }
            return Ok(categoria.ToCategoriaModel());

        }

        [HttpGet("edit/{id}")] //nelle graffe specifico i parametri se non trovo nessun id eseguo la funzione sopra qualsiasi cosa ci sia dopo Categorie/ diventa argomento della funzione
        public async Task<ActionResult<CategoriaDTO>> GetDTOById(int id)
        {
            var categoria = await _dc.Categorie.FindAsync(id);
            if (categoria == null)
            {
                return NotFound(); //status code 404
            }
            var DTO = new CategoriaDTO
            {
                Nome = categoria.Nome
            };
            return Ok(DTO);

        }

        [HttpPost]
        public async Task<ActionResult<CategoriaModel>> Add(CategoriaDTO item)
        {
            var categoria = new Categoria
            {
                Nome = item.Nome
            };
            try
            {
                
                _dc.Categorie.Add(categoria);
                await _dc.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex, "Errore durante l'inserimento della categoria");
                return StatusCode(500, "Errore interno del server");
            }
            return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria.ToCategoriaModel());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoriaModel>> Update(int id , CategoriaDTO item)
        {
            
            var categoria = await _dc.Categorie.FindAsync(id);
            if (categoria == null)
            {
                return NotFound(); //status code 404
            }
            categoria.Nome = item.Nome;
            try
            {
                
                await _dc.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex, "Errore durante l'inserimento della categoria");
                return StatusCode(500, "Errore interno del server");
            }
            return Ok(categoria.ToCategoriaModel());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            var categoria = await _dc.Categorie.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(); //status code 404
            }
            _dc.Categorie.Remove(categoria);

            try
            {

                await _dc.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: ex.Message, statusCode: 500, title: "Errore durante l'eliminazione della categoria");
            }
            return Ok(categoria.ToCategoriaModel());
        }


    }

}
