using SecureCore_Backend.Modelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace SecureCore_Backend.data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Insurance> Insurance { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientInsurance> ClientInsurance { get; set; }
        public ApplicationDbContext(DbContextOptions options) : base(options){}
        //Muestra la relacción entre el login y el usuario mediante la obtención del Id
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClientInsurance>()
                        .HasOne(ci => ci.Client)
                        .WithMany()
                        .HasForeignKey(ci => ci.Id_Client);

            modelBuilder.Entity<ClientInsurance>()
                        .HasOne(ci => ci.Insurance)
                        .WithMany()
                        .HasForeignKey(ci => ci.Id_Insurance);

            //modelBuilder.Entity<ClientInsurance>()
            //.HasOne(ci => ci.Client)
            //.WithMany(c => c.ClientInsurance)
            //.HasForeignKey(ci => ci.Id_Client);

            //modelBuilder.Entity<ClientInsurance>()
            //    .HasOne(ci => ci.Insurance)
            //    .WithMany(i => i.ClientInsurance)
            //    .HasForeignKey(ci => ci.Id_Insurance);

            //modelBuilder.Entity<Client>()
            //    .HasOne(l => l.insurance)
            //    .WithMany()
            //    .HasForeignKey(l => l.id_Insurance);
        }
    }
}
