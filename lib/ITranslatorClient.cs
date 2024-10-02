// using Google.Cloud.Translation.V2;
// using Newtonsoft.Json.Linq;

namespace lang.translator.lib
{
    interface ITranslatorClient
    {
        List<string> TranslateText(List<string> textList, string isoLangCode, string? sourceLanguage);
    }
}
