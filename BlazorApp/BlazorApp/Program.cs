using BlazorApp;
using BlazorApp.Client.Model.Enums;
using BlazorApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);

app.MapGet("/api/solution", () => SolutionEncryption.Encrypt((int)(DateTime.Now.Ticks % 100000)));
app.MapGet("/api/check", (string key, string guess) =>
{
    var solution = SolutionEncryption.Decrypt(key);
    var splitSolution = solution.ToCharArray();

    var splitGuess = guess.ToCharArray();

    var solutionCharsTaken = Enumerable.Repeat(false, splitSolution.Length).ToArray();

    var statuses = new BoardCellState[splitSolution.Length];

    /*
    Correct Cases
    */

    foreach (var item in splitGuess.Select((letter, i) => new { i, letter }))
    {
        if (item.letter == splitSolution[item.i])
        {
            statuses[item.i] = BoardCellState.Correct;
            solutionCharsTaken[item.i] = true;
        }
    }

    if (Array.TrueForAll(statuses, x => x == BoardCellState.Correct))
    {
        return statuses;
    }

    /*
    Wrong Cases
    */

    foreach (var item in splitGuess.Select((letter, i) => new { i, letter }))
    {
        if (statuses[item.i] == BoardCellState.Correct) continue;

        if (!Array.Exists(splitSolution, elem => elem == item.letter))
        {
            statuses[item.i] = BoardCellState.Wrong;
            continue;
        }

        /*
        IncorrectPosition Cases
        */

        int indexOfPresentChar = Array.FindIndex(splitSolution, x => x == item.letter);

        if (indexOfPresentChar > -1)
        {
            if (!solutionCharsTaken[indexOfPresentChar])
            {
                statuses[item.i] = BoardCellState.IncorrectPosition;
                solutionCharsTaken[indexOfPresentChar] = true;
            }
            else
            {
                statuses[item.i] = BoardCellState.Wrong;
            }
        }
        else
        {
            statuses[item.i] = BoardCellState.Wrong;
        }
    }

    return statuses;
});

await app.RunAsync();