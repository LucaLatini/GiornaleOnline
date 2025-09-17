using GiornaleOnline.DataContext.Models; // Include i modelli di dati Utente, Categoria e Articolo.
using Microsoft.EntityFrameworkCore;     // Fornisce la classe DbContext e gli strumenti di EF Core.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiornaleOnline.DataContext // Definisce lo spazio dei nomi per organizzare il codice.
{
    // La classe GOContext eredita da DbContext, rendendola un contesto di database.
    public class GOContext : DbContext
    {
        // Costruttore vuoto, utile per gli strumenti di Entity Framework Core.
        public GOContext()
        {
        }

        // Costruttore usato per la Dependency Injection, che riceve le opzioni di configurazione dal servizio di startup dell'applicazione.
        public GOContext(DbContextOptions<GOContext> options)
            : base(options)
        {
        }

        // Queste proprietà DbSet mappano le classi modello a tabelle nel database.
        // Ad esempio, DbSet<Utente> Utenti creerà una tabella "Utenti".
        public DbSet<Utente> Utenti => Set<Utente>();
        public DbSet<Categoria> Categorie => Set<Categoria>();
        public DbSet<Articolo> Articoli => Set<Articolo>();

        // Metodo per configurare la connessione al database.
        // È stato commentato, quindi la configurazione della stringa di connessione deve avvenire altrove, tipicamente nel file Program.cs.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /* if (!optionsBuilder.IsConfigured)
             {
                 optionsBuilder.UseSqlServer("...");
             }*/
        }

        // Metodo per configurare il modello di database e il Data Seeding.
        // Viene eseguito prima che lo schema del database venga creato.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API: Configura la proprietà 'Pubblicato' della tabella 'Articolo' per avere un valore predefinito 'false'.
            modelBuilder.Entity<Articolo>()
                .Property(p => p.Pubblicato)
                .HasDefaultValue(false);

            // Fluent API: Configura la proprietà 'DataCreazione' per usare la funzione SQL "getdate()"
            // (la data e l'ora attuali del server) come valore predefinito.
            modelBuilder.Entity<Articolo>()
                .Property(p => p.DataCreazione)
                .HasDefaultValueSql("getdate()");

            // Fluent API: Configura la proprietà 'DataUltimaModifica' per usare la funzione SQL "getdate()"
            // (la data e l'ora attuali del server) come valore predefinito.
            modelBuilder.Entity<Articolo>()
                .Property(p => p.DataUltimaModifica)
                .HasDefaultValueSql("getdate()");

            // Fluent API: Crea un indice univoco sulla colonna 'Username' nella tabella 'Utente'.
            // Questo assicura che non ci possano essere due utenti con lo stesso username.
            modelBuilder.Entity<Utente>()
                .HasIndex(u => u.Username)
                .IsUnique();

            #region Data Seed (Inserimento di dati in fase di creazione)
            // Usa il metodo HasData per inserire un record iniziale nella tabella 'Utente'
            // durante l'aggiornamento del database tramite le migrazioni.
            modelBuilder.Entity<Utente>().HasData(
                new Utente
                {
                    Id = 1,
                    Nome = "Admin",
                    Username = "admin",
                    Password = "pass", // In un'applicazione reale, la password dovrebbe essere hashata.

                }
                );

            // Usa il metodo HasData per inserire un record iniziale nella tabella 'Categoria'.
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 1,
                    Nome = "Cronaca",
                }
                );

            #endregion
        }
    }
}