
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var products = new List<Product>
{
    new Product(1, "Keyboard", 799),
    new Product(2, "Mouse", 399)
};

int nextId = 3;

// GET: Retrieve all products
app.MapGet("/api/products", () =>
{
    return Results.Ok(products);
});

// GET: Retrieve one product by ID
app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);

    return product is null
        ? Results.NotFound(new { message = "Product not found." })
        : Results.Ok(product);
});

// POST: Create a product
app.MapPost("/api/products", ([FromBody] ProductRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new
        {
            message = "Product name is required."
        });
    }

    if (request.Price <= 0)
    {
        return Results.BadRequest(new
        {
            message = "Price must be greater than zero."
        });
    }

    var product = new Product(nextId++, request.Name.Trim(), request.Price);
    products.Add(product);

    return Results.Created($"/api/products/{product.Id}", product);
});

// PUT: Update an existing product
app.MapPut("/api/products/{id:int}", (int id, [FromBody] ProductRequest request) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);

    if (product is null)
    {
        return Results.NotFound(new { message = "Product not found." });
    }

    if (string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
    {
        return Results.BadRequest(new
        {
            message = "A product name and a price greater than zero are required."
        });
    }

    product.Name = request.Name.Trim();
    product.Price = request.Price;

    return Results.Ok(product);
});

// DELETE: Remove a product
app.MapDelete("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);

    if (product is null)
    {
        return Results.NotFound(new { message = "Product not found." });
    }

    products.Remove(product);

    return Results.Ok(new { message = "Product deleted successfully." });
});

app.Run();

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(int id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}

public class ProductRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
