using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace BTS_Assignment.Data
{
    public class CountryService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private Dictionary<string, string> _countries;
        public bool IsInitialized;

        public CountryService()
        {
            _countries = new Dictionary<string, string>();
            IsInitialized = false;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://restcountries.com/v3.1/all");
                var countriesData = JArray.Parse(response);

                foreach (var country in countriesData)
                {
                    string code = country["cca2"]?.ToString();
                    string name = country["translations"]?["rus"]?["official"]?.ToString();

                    if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(name))
                    {
                        _countries[code] = name;
                    }
                }

                IsInitialized = true;
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                throw ex;
            }
        }

        public string this[string code]
        {
            get
            {
                try
                {
                    if (!IsInitialized)
                        return code;

                    string fullTitle = String.Empty;
                    if (_countries.ContainsKey(code))
                    {
                        if (_countries.TryGetValue(code, out fullTitle))
                        {
                            return fullTitle;
                        }
                        else
                            return code;
                    }
                    else
                    {
                        return code;
                    }
                }
                catch(KeyNotFoundException ex)
                {
                    return code;
                }
            }
        }
    }
}
