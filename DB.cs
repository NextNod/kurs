using System;
using Microsoft.EntityFrameworkCore;

namespace kurs
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime Order_date { get; set; }
        public int ID_servese { get; set; }
        public int ID_Client { get; set; }
        public string? ID_Special { get; set; }
        public double Total_time { get; set; }
        public double Cost { get; set; }
    }
    public class Serves
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public int Time_to_done { get; set; }
        public double Cost { get; set; }
        public int ID_job { get; set; }
    }
    public class Special
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public DateTime Birthday { get; set; }
        public int Expiriance { get; set; }
        public int ID_job { get; set; }
    }
    public class job
    {
        public int ID { get; set; }
        public string? Name_of_job { get; set; }
    }
    public class Acc 
    {
        public int ID { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public int SysAdmin { get; set; }
        public int Serves { get; set; }
        public int Job { get; set; }
        public int Client { get; set; }
        public int Order { get; set; }
        public int Special { get; set; }
    }
    public class Client
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
    public class DB : DbContext
    {
        public enum Acsess { Read, Write, Denien };
        public DbSet<Order> Order { get; set; }
        public DbSet<Serves> Serves { get; set; }
        public DbSet<Special> Special { get; set; }
        public DbSet<job> job { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Acc> Acc { get; set; }

        public DB()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True");
        }
    }
}
