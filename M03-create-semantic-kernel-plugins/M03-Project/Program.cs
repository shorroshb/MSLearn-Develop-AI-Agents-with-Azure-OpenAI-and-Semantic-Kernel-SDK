using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

string filePath = Path.GetFullPath("../../appsettings.json");
var config = new ConfigurationBuilder()
    .AddJsonFile(filePath)
    .Build();

// Set your values in appsettings.json
string modelId = config["modelId"]!;
string endpoint = config["endpoint"]!;
string apiKey = config["apiKey"]!;

// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

var kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();


// https://learn.microsoft.com/en-us/training/modules/give-your-ai-agent-skills/3-exercise-create-native-plugins

kernel.Plugins.AddFromType<FlightBookingPlugin>("FlightBooking");
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
var history = new ChatHistory();
history.AddSystemMessage("The year is 2025 and the current month is January");
AddUserMessage("Find me a flight to Tokyo on the 19");
await GetReply();
GetInput();
await GetReply();
