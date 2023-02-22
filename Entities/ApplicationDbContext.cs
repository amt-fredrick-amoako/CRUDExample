using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
                modelBuilder.Entity<Country>().HasData(country);


            //Seed to Persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
                modelBuilder.Entity<Person>().HasData(person);

            //Fluent API
            modelBuilder.Entity<Person>()
                .Property(entity => entity.TIN) //selects property of type Person
                .HasColumnName("TaxIdentificationNumber") //Sets Column name of the property to what you want
                .HasColumnType("varchar(8)") // manually changes the data type of the property
                .HasDefaultValue("ABC12345"); // generates a default value for any new entity that will be inserted into the table

            //modelBuilder.Entity<Person>()
            //    .HasIndex(entity => entity.TIN)
            //    .IsUnique();

            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

            //modelBuilder.Entity<Person>().ToTable(entity =>
            //entity.HasCheckConstraint("CHK_TIN", "len([TIN]) = 8"));

            //Table Relations
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne<Country>(p => p.Country)
            //    .WithMany(c => c.Persons)
            //    .HasForeignKey(p => p.CountryID);
            //});

        }

        /// <summary>
        /// stored procedure method execution
        /// </summary>
        /// <returns></returns>
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {

            SqlParameter[] parameters = new SqlParameter[]
            {
                //use matching parameters to match properties of the object type to the stored procedure parameters
                //e.g @PersonID, person.PersonID would pass property of person.PersonID into @PersonID parameter in the stored procedure command
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };

            //Database.ExecuteSqlRaw method: pass in the sql execute command as a string value alongside-
            //with 'parameters' array
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName," +
                "@Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
        }

        public int sp_UpdatePerson(Person person)
        {

            SqlParameter[] parameters = new SqlParameter[]
            {
                //use matching parameters to match properties of the object type to the stored procedure parameters
                //e.g @PersonID, person.PersonID would pass property of person.PersonID into @PersonID parameter in the stored procedure command
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };

            //Database.ExecuteSqlRaw method: pass in the sql execute command as a string value alongside-
            //with 'parameters' array
            return Database.ExecuteSqlRaw("EXECUTE [dbo].[UpdatePerson] @PersonID, @PersonName," +
                "@Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters", parameters);
        }

        public int sp_DeletePerson(Guid? personId)
        {
            SqlParameter parameter = new SqlParameter("@PersonID", personId);

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[DeletePerson] @personId", parameter);
        }
    }
}