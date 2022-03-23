using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Samples.AzureFunctions
{


    public interface ITestInject {

        string HellowWorld(string val);
    
    }


    public class TestInject : ITestInject
    {
        public string HellowWorld(string val)
        {
            return nameof(HellowWorld);
        }
    }
}
