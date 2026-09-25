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
        // Khai báo cấu hình chung cho Json.NET
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto, // Tự động lưu và đọc kiểu dữ liệu thực tế (Threshold, Blur,...)
            Formatting = Formatting.Indented           // Căn chỉnh đẹp dòng JSON
        };

        public static void SaveToJson(Model.Program program, string filePath)
        {
            // Truyền JsonSettings vào SerializeObject
            string jsonString = JsonConvert.SerializeObject(program, JsonSettings);
            File.WriteAllText(filePath, jsonString);
        }

        public static Model.Program LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string jsonString = File.ReadAllText(filePath);

            // Truyền JsonSettings vào DeserializeObject
            return JsonConvert.DeserializeObject<Model.Program>(jsonString, JsonSettings);
        }
    }
}
