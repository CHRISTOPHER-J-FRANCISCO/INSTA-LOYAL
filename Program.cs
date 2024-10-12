var builder = WebApplication.CreateBuilder(args);

// Loading configuration settings
builder.Configuration.AddJsonFile("secrets.json", optional: false, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Verify user provided a username
app.MapGet("/verifyuser", async (string username, IConfiguration configuration) => {

    // Setup http client
    using var httpClient = new HttpClient();
    // Save the bearer token
    string bearerToken = configuration["BearerToken"];

    // Add the authorization header
    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

    // Build the url
    string url = $"https://api.twitter.com/2/users/by/username/{username}";

    // Check
    //Console.WriteLine(bearerToken);
    //Console.WriteLine(url);

    // Make request
    HttpResponseMessage response = await httpClient.GetAsync(url);
    try
    {
        // Ensure success
        response.EnsureSuccessStatusCode();

        // Return response
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
    catch (HttpRequestException e)
    {
        return await response.Content.ReadAsStringAsync();
    }
})
.WithName("VerifyUser")
.WithOpenApi();

app.Run();
