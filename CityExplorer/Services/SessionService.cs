using Newtonsoft.Json;

namespace CityExplorer.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetSession<T>(string key, T value)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
        }

        public T? GetSession<T>(string key)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var value = session.GetString(key);
                return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
            return default(T);
        }

        public void RemoveSession(string key)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(key);
        }

        public void ClearSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Clear();
        }

        public bool IsLoggedIn()
        {
            return GetSession<int?>("UserId") != null;
        }

        public bool IsAdmin()
        {
            var role = GetSession<string>("UserRole");
            return role == "Admin";
        }

        public bool IsCustomer()
        {
            var role = GetSession<string>("UserRole");
            return role == "Customer";
        }
    }
}
