using GiornaleOnline.DataContext;
using GiornaleOnline.DataContext.Models;
using GiornaleOnline.Extensions;
using GiornaleOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiornaleOnline.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ArticoliController : Controller
    {
        private readonly ILogger<ArticoliController> _logger;
        private readonly GOContext _dc;

        public ArticoliController(ILogger<ArticoliController> logger, GOContext dc)
        {
            _logger = logger;
            _dc = dc;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticoloModel>>> GetAll()
        {
            var data = await _dc.Articoli
                .Include(a => a.Categoria)
                .Include(a => a.Autore)
                .Where(a => a.Pubblicato == true)
                .Select(s => s.ToArticoloModel())
                .ToListAsync();

            return Ok(data);
        }


        [HttpGet("{id}")] //nelle graffe specifico i parametri se non trovo nessun id eseguo la funzione sopra qualsiasi cosa ci sia dopo Categorie/ diventa argomento della funzione
        public async Task<ActionResult<ArticoloModel>> GetById(int id)
        {
            var articolo  = await _dc.Articoli
             .Include(Articolo => Articolo.Categoria)
             .Include(Articolo => Articolo.Autore)
             .SingleOrDefaultAsync(a => a.Id == id && a.Pubblicato == true);
            if(articolo == null)
            {
                return NotFound(); //status code 404
            }
            return Ok(articolo.ToArticoloModel());
        }

        [HttpGet("edit/{id}")] //nelle graffe specifico i parametri se non trovo nessun id eseguo la funzione sopra qualsiasi cosa ci sia dopo Categorie/ diventa argomento della funzione
        public async Task<ActionResult<ArticoloDTO>> GetDTOById(int id)
        {
            var articolo = await _dc.Articoli
             .Include(Articolo => Articolo.Categoria)
             .Include(Articolo => Articolo.Autore)
             .SingleOrDefaultAsync(a => a.Id == id);



            if (articolo == null)
            {
                return NotFound(); //status code 404
            }
            var DTO = new ArticoloDTO
            {
                Titolo = articolo.Titolo,
                Testo = articolo.Testo,
                AutoreId = articolo.Autore.Id,
                CategoriaId = articolo.Categoria.Id,
                DataCreazione = articolo.DataCreazione,
                DataUltimaModifica = articolo.DataUltimaModifica,
            };
            return Ok(DTO);

        }

        [HttpPost]
        public async Task<ActionResult<ArticoloModel>> Add(ArticoloDTO item)
        {

            var categoria = await _dc.Categorie.FindAsync(item.CategoriaId);
            if (categoria == null)
            {
                return Problem("Categoria non trovata , ", statusCode: StatusCodes.Status400BadRequest);
            }

            var autore = await _dc.Utenti.FindAsync(item.AutoreId);
            if (autore == null)
            {
                return Problem("Categoria non trovata , ", statusCode: StatusCodes.Status400BadRequest);
            }

            var Articolo = new Articolo
            {
                Titolo = item.Titolo,
                Testo = item.Testo,
                Autore = autore,
                Categoria = categoria,
                Pubblicato = item.Pubblicato
            };

            try
            {
                _dc.Articoli.Add(Articolo);
                await _dc.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex, "Errore durante l'inserimento del articolo");
                return StatusCode(500, "Errore interno del server");
            }
            return CreatedAtAction(nameof(Add), new { id = Articolo.Id }, Articolo.ToArticoloModel());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ArticoloModel>> Update(int id, ArticoloDTO item)
        {

            var articolo  = await _dc.Articoli.FindAsync(id);
            if (articolo == null)
            {
                return NotFound();
            }
            var categoria = await _dc.Categorie.FindAsync(item.CategoriaId);
            if (categoria == null)
            {
                return Problem("Categoria non trovata , ", statusCode: StatusCodes.Status400BadRequest);
            }

            var autore = await _dc.Utenti.FindAsync(item.AutoreId);
            if (autore == null)
            {
                return Problem("Categoria non trovata , ", statusCode: StatusCodes.Status400BadRequest);
            }

            articolo.Titolo = item.Titolo;
            articolo.Testo = item.Testo;
            articolo.Autore = autore;
            articolo.Categoria = categoria;
            articolo.DataUltimaModifica = DateTime.Now;
            articolo.Pubblicato = item.Pubblicato;

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
            return Ok(articolo.ToArticoloModel());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            var articolo = await _dc.Articoli.FindAsync(id);

            if (articolo == null)
            {
                return NotFound(); //status code 404
            }
            _dc.Articoli.Remove(articolo);

            try
            {

                await _dc.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Problem(detail: ex.Message, statusCode: 500, title: "Errore durante l'eliminazione della categoria");
            }
            return Ok(articolo.ToArticoloModel());
        }

    }
}
