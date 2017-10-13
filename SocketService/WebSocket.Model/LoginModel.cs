using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocket.Model
{
    public class LoginModel
    {
        public string Type { get; set; }
        public string DeviceId { get; set; }

        public double lng { get; set; }
        public double lat { get; set; }

        public DateTime Date { get; set; }

        
    }
}
