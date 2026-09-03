using LibrarySystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibrarySystem.Infrastructure.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(l => l.Id);

        builder.Property(x => x.BorrowedAt)
            .IsRequired();
        builder.Property(x => x.ReturnedAt)
            .IsRequired(false);

        builder.HasOne<Book>()
            .WithMany()
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.BookId);
        builder.HasIndex(l => l.MemberId);
    }
}
