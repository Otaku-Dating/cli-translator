using Newtonsoft.Json.Linq;
using translator.extensions;

namespace lang.translator.lib
{
    internal class JsonTranslator
    {
        private readonly ITranslatorClient client;
        const int MaxNumberOfStrings = 125;

        public JsonTranslator(string apiKey)
        {
            client = new GoogleTranslator(apiKey);
        }

        public Dictionary<string, JObject> Translate(JObject json, List<string> languages, string? sourceLanguage = null)
        {
            var jsonTranslations = new Dictionary<string, JObject>();
            foreach (var language in languages.Where(l => l != sourceLanguage))
            {
                var jsonObject = new JObject(json);
                var tokens = jsonObject.SelectTokens(@"$.*")
                    .Where(t =>
                        t.Parent?.ToObject<JProperty>()?.Name != "lang" &&
                        t.Value<string>()?.Contains("**") == false
                    ).ToList();
                jsonObject["lang"] = language;

                var list = tokens.Select(t => t.ToString()).ToList();
                var translations = new List<string>();
                list.ChunkBy(MaxNumberOfStrings).ForEach(chunk =>
                {
                    var results = client.TranslateText(chunk, language, sourceLanguage: sourceLanguage!);
                    translations.AddRange(results);
                });

                for (int i = 0; i < tokens.Count; i++)
                {
                    tokens[i].Replace(translations[i]);
                }
                Console.WriteLine(jsonObject.ToString());
                jsonTranslations.Add(language, jsonObject);
            }
            return jsonTranslations;
        }
    }
}
