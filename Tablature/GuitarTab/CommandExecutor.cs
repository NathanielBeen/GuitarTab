using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class CommandExecutor
    {
        public bool executeCommand(IActionBuilder builder)
        {
            var validator = builder.buildValidator();
            if (validator.validateAction())
            {
                var command = builder.buildCommand();
                command?.executeAction();
                return true;
            }
            return false;
        }
    }
}
