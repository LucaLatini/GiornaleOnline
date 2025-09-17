namespace GiornaleOnline.Models
{
    public class UtenteInfo //valore i ritorno quando un'utente effettua il login , sono le informazioni dell'utente + il token
                            //(tranne la password) successivamente userò il token e verrò riconosciuto come admin 
    {
        public UtenteModel? Utente { get; set; }
        public string? Token { get; set; }

    }
}
