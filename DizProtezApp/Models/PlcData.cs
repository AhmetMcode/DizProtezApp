using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DizProtezApp.Models
{
    public class PlcData
    {
        public int Id { get; set; }
        public string DataKey { get; set; }
        public string DataValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
