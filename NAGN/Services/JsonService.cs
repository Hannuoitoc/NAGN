using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace NAGN.Services
{
    public class JsonService
    {
        public static void SaveToJson(Model.Program program,string filePath)
        {
            string jsonString = JsonConvert.SerializeObject(program, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
        }
        public static Model.Program LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string jsonString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Model.Program>(jsonString);
        }
    }
}
