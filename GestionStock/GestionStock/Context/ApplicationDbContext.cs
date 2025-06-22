using GestionStock.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LignesCommande { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Stocker l'énumération Role comme string
            modelBuilder.Entity<Utilisateur>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // Relation Utilisateur -> Commandes (1 client à plusieurs commandes)
            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Client)
                .WithMany(u => u.Commandes)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Commande -> LignesCommande (1 commande à plusieurs lignes)
            modelBuilder.Entity<LigneCommande>()
                .HasOne(lc => lc.Commande)
                .WithMany(c => c.LignesCommande)
                .HasForeignKey(lc => lc.CommandeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Produit -> LignesCommande (1 produit à plusieurs lignes)
            modelBuilder.Entity<LigneCommande>()
                .HasOne(lc => lc.Produit)
                .WithMany(p => p.LignesCommande)
                .HasForeignKey(lc => lc.ProduitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Produit -> Fournisseur (plusieurs produits pour 1 fournisseur)
            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Fournisseur)
                .WithMany(f => f.Produits)
                .HasForeignKey(p => p.FournisseurId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
