using Google.Cloud.Translation.V2;

namespace lang.translator.lib
{
    public class GoogleTranslator : ITranslatorClient
    {
        private readonly TranslationClient client;

        public GoogleTranslator(string apiKey)
        {
            client = TranslationClient.CreateFromApiKey(apiKey);
        }

        List<string> ITranslatorClient.TranslateText(List<string> textList, string isoLangCode, string? sourceLanguage)
        {
            var results = client.TranslateText(textList, isoLangCode, sourceLanguage);
            return results.Select(r => r.TranslatedText).ToList();
        }
    }
}
