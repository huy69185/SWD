using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrowthTracking.ShareLibrary.Response
{
    public record Response(bool Flag = false, string Message = null!);

}
