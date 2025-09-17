using GiornaleOnline.DataContext;
using GiornaleOnline.DataContext.Models;
using GiornaleOnline.Extensions;
using GiornaleOnline.Models;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]//proteggo tutte tranne questa
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticoloModel>>> GetMiei()
        {

            // this è il nostor controller ma sicoome dervia da controller base allora ha anche una prprietà user evinta dalla richiesta http 
            //e possima cerca di esso i claim , nel nostro user avevamo messo che aveva i claim UserId
            var userIdClaim = this.User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return BadRequest();
            }
            var userId = Convert.ToInt32(userIdClaim.Value);

            var data = await _dc.Articoli
                .Include(a => a.Categoria)
                .Include(a => a.Autore)
                .Where(a => a.Autore!.Id == userId)
                .Select(s => s.ToArticoloModel())
                .ToListAsync();

            return Ok(data);
        }
 

        [Authorize] // in questo modo se lo metto solo sul metodo proteggo solo esso se lo metto prima del controller proteggo tutto
        [HttpGet("{id}")] //nelle graffe specifico i parametri se non trovo nessun id eseguo la funzione sopra qualsiasi cosa ci sia dopo Categorie/ diventa argomento della funzione
        public async Task<ActionResult<ArticoloModel>> GetById(int id)
        {
            
            var articolo = await _dc.Articoli
             .Include(Articolo => Articolo.Categoria)
             .Include(Articolo => Articolo.Autore)
             .SingleOrDefaultAsync(a => a.Id == id && a.Pubblicato == true);
            if (articolo == null)
            {
                return NotFound(); //status code 404
            }
            return Ok(articolo.ToArticoloModel());
        }

        [HttpGet("edit/{id}")] //nelle graffe specifico i parametri se non trovo nessun id eseguo la funzione sopra qualsiasi cosa ci sia dopo Categorie/ diventa argomento della funzione
        public async Task<ActionResult<ArticoloDTO>> GetDTOById(int id)
        {
            // this è il nostor controller ma sicoome dervia da controller base allora ha anche una prprietà user evinta dalla richiesta http 
            //e possima cerca di esso i claim , nel nostro user avevamo messo che aveva i claim UserId
            var userIdClaim = this.User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return BadRequest();
            }
            var userId = Convert.ToInt32(userIdClaim.Value);





            var articolo = await _dc.Articoli
             .Include(Articolo => Articolo.Categoria)
             .Include(Articolo => Articolo.Autore)
             .SingleOrDefaultAsync(a => a.Id == id);



            if (articolo == null)
            {
                return NotFound(); //status code 404
            }

            if(articolo.Autore!.Id != userId)
            {
                return Unauthorized();
            } 
            var DTO = new ArticoloDTO
            {
                Titolo = articolo.Titolo,
                Testo = articolo.Testo,
                CategoriaId = articolo.Categoria.Id,
                DataCreazione = articolo.DataCreazione,
                DataUltimaModifica = articolo.DataUltimaModifica,
            };
            return Ok(DTO);

        }

        [HttpPost]
        public async Task<ActionResult<ArticoloModel>> Add(ArticoloDTO item)
        {
            // this è il nostor controller ma sicoome dervia da controller base allora ha anche una prprietà user evinta dalla richiesta http 
            //e possima cerca di esso i claim , nel nostro user avevamo messo che aveva i claim UserId
            var userIdClaim = this.User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return BadRequest();
            }
            var userId = Convert.ToInt32(userIdClaim.Value);

           
            var categoria = await _dc.Categorie.FindAsync(item.CategoriaId);
            if (categoria == null)
            {
                return Problem("Categoria non trovata , ", statusCode: StatusCodes.Status400BadRequest);
            }

            var autore = await _dc.Utenti.FindAsync(userId); // per inserire l'autore non prendo l'autore contenuto nel token 

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
            // this è il nostor controller ma sicoome dervia da controller base allora ha anche una prprietà user evinta dalla richiesta http 
            //e possima cerca di esso i claim , nel nostro user avevamo messo che aveva i claim UserId
            var userIdClaim = this.User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return BadRequest();
            }
            var userId = Convert.ToInt32(userIdClaim.Value);

            var articolo = await _dc.Articoli.FindAsync(id);
            if (articolo == null)
            {
                return NotFound();
            }

            if (articolo.Autore.Id != userId)
            {
                return Unauthorized();
            }

            var categoria = await _dc.Categorie.FindAsync(item.CategoriaId);
            if (categoria == null)
            {
                return Problem("Categoria non trovata , ", statusCode: StatusCodes.Status400BadRequest);
            }

            var autore = await _dc.Utenti.FindAsync(userId);
            if (autore == null)
            {
                return Problem("Autore non trovato , ", statusCode: StatusCodes.Status400BadRequest);
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
