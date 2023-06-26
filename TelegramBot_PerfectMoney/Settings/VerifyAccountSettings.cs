using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TelegramBot_PerfectMoney.Settings
{
    public class VerifyAccountSettings
    {
        private string _address;
        private string _clientId;
        private string _clientSecret;
        private string _trackId;
        private string _scopes;
        private string _nid;

        [JsonPropertyName("address")]
        public string Address
        {
            get { return _address; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Address cannot be null, empty, or whitespace.");
                }
                _address = value;
            }
        }

        [JsonPropertyName("clientId")]
        public string ClientId
        {
            get { return _clientId; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("ClientId cannot be null, empty, or whitespace.");
                }
                _clientId = value;
            }
        }

        [JsonPropertyName("clientSecret")]
        public string ClientSecret
        {
            get { return _clientSecret; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("ClientSecret cannot be null, empty, or whitespace.");
                }
                _clientSecret = value;
            }
        }

        [JsonPropertyName("trackId")]
        public string TrackId
        {
            get { return _trackId; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("TrackId cannot be null, empty, or whitespace.");
                }
                _trackId = value;
            }
        }

        [JsonPropertyName("scopes")]
        public string Scopes
        {
            get { return _scopes; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Scopes cannot be null, empty, or whitespace.");
                }
                _scopes = value;
            }
        }

        [JsonPropertyName("nid")]
        public string Nid
        {
            get { return _nid; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Nid cannot be null, empty, or whitespace.");
                }
                _nid = value;
            }
        }
    }
}
