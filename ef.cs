
public class OrderLine
{
    public int Id { get; set; }
    public string ToyId { get; set; }
    public virtual Toy Toy { get; set; }
    public string Status { get; set; }
    public string SantaNote { get; set; }
    public int Quantity { get; set; }
    public string ToyName { get; set; }
}

public class Toy
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Price { get; set; }
    public string ElfIds { get; set; }
    public string CreatedOn { get; set; }
    public virtual List<OrderLine> OrderLines { get; set; }
}

public class Elf { 
    public int Id { get; set; }
     public string Name { get; set; } 
}

public IList<(string Toy, string Elf, int Quantity)> SantasFavQuery(DbContextOptions<AppDbContext> options)
{

    using var ctx = new AppDbContext(options);

    return ctx.OrderLines
        .Include(o => o.Toy)
        .Where(o => o.Status == "Wrapped" && o.SantaNote == "nice")
        .AsEnumerable()
        .Where(o => decimal.Parse(o.Toy.Price) > 10)
        .OrderBy(o => DateTime.Parse(o.Toy.CreatedOn))
        .Select(o => (o.ToyName, o.Toy.ElfIds, o.Quantity))
        .ToList();

}

It's **23 December, 14:00**. Elves Inc. has exactly one customer: **Santa**. His "Naughty-or-Nice Order Report" takes 4 minutes and times out. He is *very* disappointed. The DB has 12M `OrderLines` and the site must stay up. The previous developer (an elf on a 3-week contract) left the model below. **Fix it without losing a single toy.**


| # | Item | Level |
| --- | --- | --- |
| 1 | `AsEnumerable()` forces client evaluation. Move `Where`, `OrderBy` and `Select` into SQL. | **M** |
| 2 | Project with `Select` into a DTO/tuple *before* materializing. `Include` is unnecessary. | **M** |
| 3 | `Price` → `decimal`, `CreatedOn` → `DateTime` (UTC). Parsing in a predicate means no index use. | **M** |
| 4 | `Status` and `SantaNote` as enums (no "Wrapped" vs "wrapped" or "nice" vs "Nice" risk). | **M** |
| 5 | `ToListAsync(ct)` with a `CancellationToken`, `await` end to end. | **M** |
| 6 | Index the filter columns `OrderLine.Status` and `OrderLine.SantaNote`. | **M** |
| 7 | `AsNoTracking()` (redundant with a projection; knowing why is a bonus). | **M** |
| 8 | Consistent key types (`Toy.Id` string vs int elsewhere) and a proper FK. | **M/S** |
| 9 | Normalizes `ElfIds` to many-to-many (`List<Elf> Elves` ↔ `List<Toy> Toys`). Notes the row-per-elf effect and the `Elf` vs IDs return mismatch. | **S** |


REFERENCE SOLUTION:
MODEL
public enum OrderStatus : byte { Pending = 0, Wrapped = 1, Shipped = 2 }
public enum SantaNote : byte { Naughty = 0, Nice = 1 }

public class OrderLine
{
    public int Id { get; set; }
    public int ToyId { get; set; }
    public Toy Toy { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public SantaNote SantaNote { get; set; }
    public int Quantity { get; set; }
    // ToyName dropped: use Toy.Name
}

public class Toy
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public DateTime CreatedOn { get; set; }      // UTC
    public List<Elf> Elves { get; set; } = new();
    public List<OrderLine> OrderLines { get; set; } = new();
}

public class Elf
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Toy> Toys { get; set; } = new();
}

DTO 
public record SantaReportRow(string Toy, string Elf, int Quantity);

QUERY
public async Task<IReadOnlyList<SantaReportRow>> SantasFavQuery(
    IDbContextFactory<AppDbContext> factory,
    decimal minPrice,
    CancellationToken ct)
{
    await using var ctx = await factory.CreateDbContextAsync(ct);

    return await ctx.OrderLines
        .AsNoTracking()
        .Where(o => o.Status == OrderStatus.Wrapped
                 && o.SantaNote == SantaNote.Nice
                 && o.Toy.Price > minPrice)
        .OrderBy(o => o.Toy.CreatedOn)
        .ThenBy(o => o.Id)
        .SelectMany(
            o => o.Toy.Elves,
            (o, e) => new SantaReportRow(o.Toy.Name, e.Name, o.Quantity))
        .ToListAsync(ct);
}

INDEX 
modelBuilder.Entity<OrderLine>()
    .HasIndex(o => new { o.Status, o.SantaNote });
