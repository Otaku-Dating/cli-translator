using System.CommandLine;
using lang.translator.lib;
using Newtonsoft.Json.Linq;
using translator.validators;

namespace translator;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<JObject?>(
            name: "--file",
            description: "An option whose argument is parsed as a FileInfo",
            isDefault: true,
            parseArgument: result =>
            {
                var filePath = result.Tokens.SingleOrDefault()?.Value;
                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = "File does not exist";
                    return null;
                }
                else if (FileValidator.IsValidJsonObject(filePath))
                {
                    result.ErrorMessage = "File is not json object";
                    return null;
                }
                else
                {
                    return JObject.Parse(File.ReadAllText(filePath));
                }
            })
        {
            IsRequired = true,
        };

        var sourceLanguage = new Option<String?>(
            name: "--from",
            description: "language of the original json to be translated.",
            getDefaultValue: () => null);

        var targetLanguages = new Option<List<String>?>(
            name: "--to",
            description: "This is the language into which to translate. (ISO codes)",
            parseArgument: result =>
            {
                return result.Tokens
                    .SingleOrDefault()?.Value?.Split(",")?
                    .Select(v => v.Trim())?.ToList();
            })
        {
            IsRequired = true
        };

        var outFolderPath = new Option<String?>(
            name: "--out",
            description: "Translated files or folders will be saved here.  A new folder will be created (if necessary) at this location named after the abbreviation for the language you are translating into.");

        var apiKey = new Option<String>(
            name: "--key",
            description: "access api key.")
        {
            IsRequired = true
        };

        var rootCommand = new RootCommand("Translator app for Otaku Dating.");
        var readCommand = new Command("json", "translate the file.")
        {
            fileOption,
            targetLanguages,
            sourceLanguage,
            outFolderPath,
            apiKey,
        };

        rootCommand.AddCommand(readCommand);
        readCommand.SetHandler(async (file, languagesToTranslate, sourceLanguage, outFolderPath, apiKey) =>
            {
                await TranslateFile(
                    file!,
                    languagesToTranslate!,
                    apiKey,
                    sourceLanguage: sourceLanguage,
                    outFolderPath: outFolderPath);
            },
            fileOption, targetLanguages, sourceLanguage, outFolderPath, apiKey);

        return await Task.FromResult(rootCommand.InvokeAsync(args).Result);
    }

    internal static async Task TranslateFile(JObject jObject, List<String> targetLanguages, string apiKey, string? sourceLanguage = null, string? outFolderPath = null)
    {
        var translator = new JsonTranslator(apiKey);
        var translations = translator.Translate(jObject, targetLanguages, sourceLanguage);
        WriteFiles(translations, outFolderPath ?? Directory.GetCurrentDirectory());
        await Task.CompletedTask;
    }

    public static void WriteFiles(Dictionary<string, JObject> translations, string targetDir)
    {
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }
        foreach (var translation in translations)
        {
            var pathToUpdate = Path.Combine(targetDir, $"{translation.Key}.json");
            if (File.Exists(pathToUpdate))
            {
                var jObjToUpdate = JObject.Parse(File.ReadAllText(pathToUpdate));
                foreach (var field in translation.Value)
                {
                    if (!jObjToUpdate.ContainsKey(field.Key))
                    {
                        jObjToUpdate[field.Key] = field.Value;
                    }
                }
                File.WriteAllText(pathToUpdate, jObjToUpdate.ToString());
            }
            else
            {
                File.WriteAllText(pathToUpdate, translation.Value.ToString());
            }
        }
    }
}
