using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    public class Serialize
    {

        public static string getParam(NameValueCollection st)
        {
            string str = "";
            for (int i = 0; i < st.Count; i++)
            {
                if (st.GetKey(i).IndexOf("CNZZDATA") > -1)
                { break; }
                str += st.GetKey(i) + ":";
                string[] values = st.GetValues(i);
                if (values.Length == 1)
                {
                    str += values[0] + " ";
                }
                else
                {
                    foreach (string s in values)
                    {
                        str += values[0] + " ";
                    }
                }
                str += " ; ";
            }
            return str;
        }
    }
}
