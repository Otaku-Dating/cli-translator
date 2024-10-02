# CLI Translator

This command line tool delivers text translation capabilities to your console, powered by GoogleTranslator

## Requires

- .NET 7.0
- System.CommandLine
- Newtonsoft.Json

## Deploy

```bash
dotnet publish
```

## Usage
```shell
  translator json [options]
````

## Options
```
  --file <file> (REQUIRED)  An option whose argument is parsed as a FileInfo []
  --to <to> (REQUIRED)      This is the language into which to translate. (ISO 
                            codes)
  --from <from>             language of the original json to be translated. []
  --out <out>               Translated files or folders will be saved here.  A 
                            new folder will be created (if necessary) at this 
                            location named after the abbreviation for the 
                            language you are translating into.
  --key <key> (REQUIRED)    access api key.
  -?, -h, --help            Show help and usage information

```

Example
```shell
./translator json --file en.json --to en,es,fr --out ./assets/langs --key <api-key>

```

## License

[MIT](https://choosealicense.com/licenses/mit/)