using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testProject.Settings;

namespace testProject
{
    internal class Class1
    {
        public Class1(VerifyAccountSettings verify)
        {
            Verify = verify;
        }

        public VerifyAccountSettings Verify { get; }
    }
}
