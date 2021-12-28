using SvtRoboticsTakehome;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Businesslayer>(new Businesslayer());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/robots/getbots", () =>
{
    var url = "https://60c8ed887dafc90017ffbd56.mockapi.io/robots";
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri(url);
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    HttpResponseMessage response = client.GetAsync(url).Result;
    if (response.IsSuccessStatusCode)
    {
        var result = response.Content.ReadFromJsonAsync<IEnumerable<bots>>().Result;

        return result?.ToList();
    }
    else return null;
})
.WithName("Getbots");
app.MapPost("/robots/closest", (Load load, Businesslayer layer) =>
{
    object dataset;
    List<bots> result = layer.Getbots();
    Dictionary<int, double> robots = new Dictionary<int, double>();
    foreach(bots bot in result)
    {
        double d2 = 0;
        d2 = Math.Sqrt(Math.Pow(load.CordinateX - bot.X, 2) + Math.Pow(load.CordicateY - bot.Y, 2));
        robots.Add(bot.robotId, d2);
    }
    var shortdistancebots = robots.OrderBy(m=>m.Value);
    var selectedbot = new List<bots>();
    if (shortdistancebots.Any(m => m.Value<10))
    {
        foreach (var item in shortdistancebots)
        {
            if (item.Value<10)
            {
                selectedbot.AddRange(result.Where(bot => bot.robotId.Equals(item.Key)));
            }
        }
    }
    else { dataset= result.FirstOrDefault(m => m.robotId.Equals(shortdistancebots.Select(p=>p.Key))); }

    dataset =selectedbot.OrderByDescending(m => m.batteryLevel).FirstOrDefault();

    return dataset;

}).WithName("Payload");
app.Run();

