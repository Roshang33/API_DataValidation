using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataValidation
{
    

    public interface IValidationPlugin
    {
        Dictionary<string, Func<string, Task<bool>>> GetValidationFunctions();
    }

}
