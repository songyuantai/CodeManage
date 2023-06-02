using CodeManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeManager.Busi.Gen
{
    public class BuilderParam
    {
        public string NameSpaceName { get; set; }

        public DbTable Table { get; set; }

        public List<DbTableColumn> Columns { get; set; }
    }
}
