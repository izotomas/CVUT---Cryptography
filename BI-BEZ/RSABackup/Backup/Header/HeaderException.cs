using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupUtility.Backup.Header
{
    public class HeaderException: Exception
    {
        public HeaderException(string message) : base(message) { }
    }
}
