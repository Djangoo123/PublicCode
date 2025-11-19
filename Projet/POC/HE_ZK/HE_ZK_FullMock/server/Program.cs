
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using HEZK.Server.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHomomorphicService, MockHomomorphicService>();
builder.Services.AddSingleton<IZkService, MockZkService>();
var app = builder.Build();

app.MapPost("/compute/sum", async (HttpRequest req, IHomomorphicService he, IZkService zk) =>
{
    var body = await req.ReadFromJsonAsync<RequestPayload>();
    if(body == null) return Results.BadRequest();
    if(!zk.Verify(body.Ciphertext, body.Proof)) return Results.BadRequest("Invalid proof");
    var result = he.Sum(body.Ciphertext);
    return Results.Ok(new { ciphertext = result });
});

app.Run();

record RequestPayload(string[] Ciphertext, string Proof);
