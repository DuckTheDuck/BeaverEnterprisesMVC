using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BeaverEnterprisesMVC.Models;

public partial class BeaverEnterprisesContext : DbContext
{
    public BeaverEnterprisesContext()
    {
    }

    public BeaverEnterprisesContext(DbContextOptions<BeaverEnterprisesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Aircraft> Aircraft { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Flightschedule> Flightschedules { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Passenger> Passengers { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-N4C79H0;Database=BeaverEnterprises;Trusted_Connection=True;Persist Security Info=True;TrustServerCertificate=True;MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("ACCOUNT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code).HasColumnName("CODE");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
        });

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AIRCRAFT__7111506BA1FDCA55");

            entity.ToTable("AIRCRAFT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Capacity).HasColumnName("CAPACITY");
            entity.Property(e => e.IdManufacturer).HasColumnName("ID_MANUFACTURER");
            entity.Property(e => e.MinimumLicense)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MINIMUM_LICENSE");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("MODEL");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.SerialNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SERIAL_NUMBER");

            entity.HasOne(d => d.IdManufacturerNavigation).WithMany(p => p.Aircraft)
                .HasForeignKey(d => d.IdManufacturer)
                .HasConstraintName("FK_AIRCRAFT_MANUFACTURER");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("CART");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdAccount).HasColumnName("ID_ACCOUNT");
            entity.Property(e => e.IdTicket).HasColumnName("ID_TICKET");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.IdAccountNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdAccount)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CART_ACCOUNT");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CART_TICKET");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CLASS__AA1D43788A822154");

            entity.ToTable("CLASS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Capacity).HasColumnName("CAPACITY");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("PRICE");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EMPLOYEE__3214EC27E508C827");

            entity.ToTable("EMPLOYEE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.IsPilot).HasColumnName("IS_PILOT");
            entity.Property(e => e.License)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LICENSE");
            entity.Property(e => e.LicenseNum).HasColumnName("LICENSE_NUM");
            entity.Property(e => e.MonthlySalary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MONTHLY_SALARY");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.NumHours).HasColumnName("NUM_HOURS");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("PHONE");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FLIGHTS__5BB92D77BAE5A61F");

            entity.ToTable("FLIGHTS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ArrivalTime)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("ARRIVAL_TIME");
            entity.Property(e => e.DepartureTime)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("DEPARTURE_TIME");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("END_DATE");
            entity.Property(e => e.FlightNumber).HasColumnName("FLIGHT_NUMBER");
            entity.Property(e => e.IdAircraft).HasColumnName("ID_AIRCRAFT");
            entity.Property(e => e.IdClass).HasColumnName("ID_CLASS");
            entity.Property(e => e.IdDestination).HasColumnName("ID_DESTINATION");
            entity.Property(e => e.IdOrigin).HasColumnName("ID_ORIGIN");
            entity.Property(e => e.Periocity)
                .HasDefaultValue(-1)
                .HasColumnName("PERIOCITY");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("START_DATE");

            entity.HasOne(d => d.IdAircraftNavigation).WithMany(p => p.Flights)
                .HasForeignKey(d => d.IdAircraft)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FLIGHTS_AIRCRAFT");

            entity.HasOne(d => d.IdClassNavigation).WithMany(p => p.Flights)
                .HasForeignKey(d => d.IdClass)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FLIGHTS_CLASS");

            entity.HasOne(d => d.IdDestinationNavigation).WithMany(p => p.FlightIdDestinationNavigations)
                .HasForeignKey(d => d.IdDestination)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FLIGHTS__DESTINA__4BAC3F29");

            entity.HasOne(d => d.IdOriginNavigation).WithMany(p => p.FlightIdOriginNavigations)
                .HasForeignKey(d => d.IdOrigin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FLIGHTS__ORIGIN___4CA06362");
        });

        modelBuilder.Entity<Flightschedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FLIGHTSC__3214EC2738A1E547");

            entity.ToTable("FLIGHTSCHEDULE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FlightDate).HasColumnName("FLIGHT_DATE");
            entity.Property(e => e.IdFlight).HasColumnName("ID_FLIGHT");

            entity.HasOne(d => d.IdFlightNavigation).WithMany(p => p.Flightschedules)
                .HasForeignKey(d => d.IdFlight)
                .HasConstraintName("FK__FLIGHTSCH__ID_FL__5EBF139D");
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FUNCTION__3214EC27DCAFEE8F");

            entity.ToTable("FUNCTIONS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FunctionDescription)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FUNCTION_DESCRIPTION");
            entity.Property(e => e.IdEmployee).HasColumnName("ID_EMPLOYEE");
            entity.Property(e => e.IdFlight).HasColumnName("ID_FLIGHT");

            entity.HasOne(d => d.IdEmployeeNavigation).WithMany(p => p.Functions)
                .HasForeignKey(d => d.IdEmployee)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FUNCTIONS__EMPLO__4E88ABD4");

            entity.HasOne(d => d.IdFlightNavigation).WithMany(p => p.Functions)
                .HasForeignKey(d => d.IdFlight)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FUNCTIONS_FLIGHTS");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LOCATION__3214EC27545A544E");

            entity.ToTable("LOCATION");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CITY");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("COUNTRY");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MANUFACT__3214EC275D5F6E67");

            entity.ToTable("MANUFACTURER");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CountryOfOrigin)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("COUNTRY_OF_ORIGIN");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Passenger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PASSENGE__AA1D4378A951ADE7");

            entity.ToTable("PASSENGER");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Gender)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.SeatNumber).HasColumnName("SEAT_NUMBER");
            entity.Property(e => e.Surname)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("SURNAME");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RESERVAT__AA1D43784082D5C0");

            entity.ToTable("RESERVATION");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Confirmed).HasColumnName("CONFIRMED");
            entity.Property(e => e.IdFlight).HasColumnName("ID_FLIGHT");
            entity.Property(e => e.IdPassenger).HasColumnName("ID_PASSENGER");

            entity.HasOne(d => d.IdFlightNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdFlight)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RESERVATION_FLIGHTS");

            entity.HasOne(d => d.IdPassengerNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdPassenger)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RESERVATI__PASSE__5070F446");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("TICKET");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IdFlightSchedule).HasColumnName("ID_FLIGHT_SCHEDULE");
            entity.Property(e => e.IdPassager).HasColumnName("ID_PASSAGER");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("SEAT_NUMBER");

            entity.HasOne(d => d.IdFlightScheduleNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdFlightSchedule)
                .HasConstraintName("FK_TICKET_FLIGHT_SCHEDULE");

            entity.HasOne(d => d.IdPassagerNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdPassager)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TICKET_PASSENGER");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
