using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2600Demo
{
    public interface ScpiOper
    {
        bool Open();

        bool Write(string inStr);

        bool Read(out string outStr);

        bool Query(string inStr, out string outStr);

        byte EndByte { get; set; }

        void Close();

        bool TryClearBuff();
    }
}
