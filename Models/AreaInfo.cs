using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerovskiteTest.Models
{
    public class AreaInfo
    {
        public int id { get; set; }
        public string serialNum { get; set; }
        public int areaCode { get; set; }

        public ushort stepId { get; set; }
        public string imgName { get; set; }
        public DateTime createTime { get; set; }
    }
}
