using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxKite.Twitter.Models;

namespace BoxKite.Twitter.Models.Service
{
    public class UserInListCursored : TwitterControlBase
    {
        public long previous_cursor { get; set; }
        public IEnumerable<TwitterList> lists { get; set; }
        public long next_cursor { get; set; }
    }

}
