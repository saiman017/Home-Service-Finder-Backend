using Newtonsoft.Json;

namespace Home_Service_Finder.Authentication.Handler

{
    public static class EncodingHandler
    {

        public static string EncodeToBase64(object obj)
        {

            var jsonString = JsonConvert.SerializeObject(obj);
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            return Convert.ToBase64String(bytes);
        }

        public static T DecodeFromBase64<T>(string base64String)
        {

            var bytes = Convert.FromBase64String(base64String);
            var jsonString = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
