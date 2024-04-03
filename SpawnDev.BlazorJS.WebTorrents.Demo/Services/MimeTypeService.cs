using System.Net.Http.Json;

namespace SpawnDev.BlazorJS.WebTorrents.Demo.Services
{
    public class MimeTypeService : IAsyncBackgroundService
    {
        public bool Loaded { get; private set; }
        public bool NotFound { get; private set; }
        string _contentPath = "";
        bool InitRunning = false;
        HttpClient HttpClient;
        Dictionary<string, string[]>? __MimeTypeExtensionsMap = null;
        public MimeTypeService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
        public async Task InitAsync()
        {
            if (InitRunning || Loaded || NotFound) return;
            InitRunning = true;
            try
            {
                var resp = await HttpClient.GetAsync("mime.json");
                if (resp.IsSuccessStatusCode)
                {
                    __MimeTypeExtensionsMap = await resp.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                    Loaded = true;
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NotFound = true;
                }
            }
            catch
            {
                //
            }
            finally
            {
                InitRunning = false;
            }
        }
        Dictionary<string, string> ReverseMap = new Dictionary<string, string>();
        public string? GetExtensionMimeType(string? extension)
        {
            if (extension == null) return null;
            if (__MimeTypeExtensionsMap == null) return null;
            if (extension.Contains(".")) extension = extension.Substring(extension.LastIndexOf(".") + 1);
            if (ReverseMap.TryGetValue(extension, out var ext)) return ext;
            foreach (var kvp in __MimeTypeExtensionsMap)
            {
                if (kvp.Value.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    ReverseMap[extension] = kvp.Key;
                    return kvp.Key;
                }
            }
            return "";
        }
        public string GetExtensionImageHref(string? extension)
        {
            var mimeType = GetExtensionMimeType(extension);
            var baseType = string.IsNullOrEmpty(mimeType) || !mimeType.Contains("/") ? "" : mimeType.Substring(0, mimeType.IndexOf("/"));
            switch (baseType)
            {
                case "text": return $"{_contentPath}media/{baseType}.png";
                case "image": return $"{_contentPath}media/{baseType}.png";
                case "video": return $"{_contentPath}media/{baseType}.png";
                case "audio": return $"{_contentPath}media/{baseType}.png";
                case "application": return $"{_contentPath}media/binary.png";
                default: return $"{_contentPath}media/empty.png";
            }
        }
    }
}
