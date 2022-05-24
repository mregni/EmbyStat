using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;

namespace EmbyStat.Common.Configuration;

public class WritableJsonConfigurationProvider: JsonConfigurationProvider
{
    public WritableJsonConfigurationProvider(JsonConfigurationSource source) : base(source)
    {
    }

    public override void Set(string key, string value)
    {
        base.Set(key,value);

        //Get Whole json file and change only passed key with passed value. It requires modification if you need to support change multi level json structure
        var fileFullPath = Source.FileProvider.GetFileInfo(Source.Path).PhysicalPath;
        var json = File.ReadAllText(fileFullPath);
        dynamic jsonObj = JsonConvert.DeserializeObject(json);

        var keys = key.ToLower().Split(':');

        if (key.Contains(':'))
        {
            jsonObj[keys[0]][keys[1]] = value;
        }
        else
        {
            jsonObj[key] = value;  
        }
        
        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        File.WriteAllText(fileFullPath, output);
    }
}