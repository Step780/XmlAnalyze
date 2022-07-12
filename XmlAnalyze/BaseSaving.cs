using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlAnalyze
{

    [Serializable]
    public class BaseSaving
    {
        public string server { get; set; }
        public string login { get; set; }
        public string password { get; set; }

        public BaseSaving() { }
        public BaseSaving(string serverr, string loginn, string passwordd)
        {
            server = serverr;
            login = loginn;
            password = passwordd;
            
        }

    }
}
