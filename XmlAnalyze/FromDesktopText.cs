using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlAnalyze
{
    [Serializable]
    public class FromDesktopText
    {

        public string textDesktop { get; set; }

        public FromDesktopText() { }
        public FromDesktopText(string textdesktop)
        {
            textDesktop = textdesktop;
        }
    }
}
