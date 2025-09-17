using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiornaleOnline.Models
{
    public  class CategoriaDTO
    {
        [Required(ErrorMessage ="il nome del campo è obbligatorio")]
        public string? Nome { get; set; }
    }
}
