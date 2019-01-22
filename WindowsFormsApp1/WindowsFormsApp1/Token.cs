using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class UploadCredentials
    {
        public string env { get; set; }
        public string upload_token { get; set; }
        public string url { get; set; }
    }

    public class Token
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in_refresh { get; set; }
        public int expires_in_access { get; set; }
        public int user_type { get; set; }
        public UploadCredentials upload_credentials { get; set; }
    }
}
