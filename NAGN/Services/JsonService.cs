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
            Formatting = Formatting.Indented,          // Căn chỉnh đẹp dòng JSON
            NullValueHandling = NullValueHandling.Ignore
        };

        public static void SaveToJson(Model.Program program, string filePath)
        {
            string jsonString = JsonConvert.SerializeObject(program, JsonSettings);
            File.WriteAllText(filePath, jsonString);
        }

        public static Model.Program? LoadFromJson(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;

            try
            {
                string jsonString = File.ReadAllText(filePath);
                var program = JsonConvert.DeserializeObject<Model.Program>(jsonString, JsonSettings);

                // Tự động khôi phục pipeline ảnh cho từng FOV nếu file ảnh tồn tại
                if (program != null)
                {
                    foreach (var fov in program.FOVlist)
                    {
                        if (!string.IsNullOrEmpty(fov.ImageFilePath) && File.Exists(fov.ImageFilePath))
                        {
                            fov.updateOutputImageslist(fov.ImageFilePath);
                        }
                    }
                }

                return program;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi mở file cấu hình: {ex.Message}", "Lỗi tải Model", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }
        }
    }
}
