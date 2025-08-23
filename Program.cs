using Microsoft.EntityFrameworkCore;
using ApiRestEf.Data;
using ApiRestEf.Models;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

// Configurar EF InMemory
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ApiDb"));

var app = builder.Build();

// Servir index.html diretamente em /
app.UseDefaultFiles();
app.UseStaticFiles();

// SEED: livros e dados
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Books.Any())
    {
        db.Books.AddRange(
             new Book { Titulo = "Programação Back End II", Autor = "LEDUR, Cleverson Lopes", Editora = "SAGAH", Ano = 2019, Local = "Porto Alegre" },
             new Book { Titulo = "Programação Back End III", Autor = "FREITAS, Pedro H. Chagas [et al.]", Editora = "SAGAH", Ano = 2021, Local = "Porto Alegre" },
             new Book { Titulo = "Ajax, RICH Internet Applications e desenvolvimento Web para programadores", Autor = "DEITEL, Paul J.", Editora = "Pearson Prentice Hall", Ano = 2008, Local = "São Paulo" }
        );
    }

    if (!db.StudentInfos.Any())
    {
        db.StudentInfos.Add(new StudentInfo { Id = 1, Nome = "Natalia de Azevedo Cardoso", RU = "4059988", Curso = "Mobile" });
    }

    db.SaveChanges();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (LoginRequest login) =>
{
    // usuário e senha fixos
    var validUser = "Cardoso";
    var validPassword = "4059988";

    if (login.Username == validUser && login.Password == validPassword)
    {
        return Results.Ok("Login realizado com sucesso!");
    }

    return Results.Unauthorized();
});

// Endpoints CRUD para Books
app.MapGet("/books", async (AppDbContext db) => await db.Books.AsNoTracking().ToListAsync());
app.MapGet("/books/{id:int}", async (int id, AppDbContext db) =>
    await db.Books.FindAsync(id) is Book found ? Results.Ok(found) : Results.NotFound());
app.MapPost("/books", async (Book book, AppDbContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Id}", book);
});
app.MapPut("/books/{id:int}", async (int id, Book updated, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    book.Titulo = updated.Titulo;
    book.Autor = updated.Autor;
    book.Editora = updated.Editora;
    book.Local = updated.Local;
    book.Ano = updated.Ano;

    await db.SaveChangesAsync();
    return Results.Ok(book);
});
app.MapDelete("/books/{id:int}", async (int id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoints para StudentInfo
app.MapGet("/student", async (AppDbContext db) => await db.StudentInfos.AsNoTracking().FirstOrDefaultAsync());
app.MapPut("/student", async (StudentInfo updated, AppDbContext db) =>
{
    var student = await db.StudentInfos.FirstOrDefaultAsync();
    if (student is null)
    {
        db.StudentInfos.Add(updated);
    }
    else
    {
        student.Nome = updated.Nome;
        student.RU = updated.RU;
        student.Curso = updated.Curso;
    }

    await db.SaveChangesAsync();
    return Results.Ok(updated);
});

app.Run();