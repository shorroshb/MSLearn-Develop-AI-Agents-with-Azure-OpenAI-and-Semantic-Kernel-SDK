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
void AddUserMessage(string msg) {
    Console.WriteLine(msg);
    chatHistory.AddUserMessage(msg);
}

void GetInput() {
    string input = Console.ReadLine()!;
    chatHistory.AddUserMessage(input);
}

async Task GetReply() {
    ChatMessageContent reply = await chatCompletionService.GetChatMessageContentAsync(
        chatHistory,
        kernel: kernel
    );
    Console.WriteLine(reply.ToString());
    chatHistory.AddAssistantMessage(reply.ToString());
}

// Add the plugin
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

/*
// https://learn.microsoft.com/en-us/training/modules/give-your-ai-agent-skills/5-exercise-configure-available-functions
// Add the plugin
kernel.Plugins.AddFromType<CurrencyExchangePlugin>("CurrencyExchange");

// Select the plugin functions
KernelFunction searchFlight = kernel.Plugins.GetFunction("FlightBooking", "search_flights");

KernelFunction convertCurrency = kernel.Plugins.GetFunction("CurrencyExchange", "convert_currency")
// Enable planning
PromptExecutionSettings openAIPromptExecutionSettings = new() 
{ 
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [searchFlight, convertCurrency]) 
};    
var history = new ChatHistory();
// history.AddSystemMessage("The year is 2025 and the current month is January");
AddUserMessage("Please convert $30 USD to Japanese Yen");
await GetReply();  


// Add the plugins
// kernel.Plugins.AddFromType<FlightBookingPlugin>("FlightBooking");
// kernel.Plugins.AddFromType<CurrencyExchangePlugin>("CurrencyExchange");
kernel.Plugins.AddFromType<WeatherPlugin>("Weather");
KernelFunction getWeather = kernel.Plugins.GetFunction("Weather", "get_weather");

PromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [getWeather]) 
};

var history = new ChatHistory();
history.AddSystemMessage("The year is 2025 and the current month is January");
AddUserMessage("What is the weather in Tokyo");
await GetReply();

*/
