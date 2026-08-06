using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsDesk.Enums;

namespace WmsDesk.Classes
{
    public class IncomeItem
    {
        public string Id { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string GoodsId { get; set; } = string.Empty;
        public int Status{ get; set; } = (int)StatusType.Created;
        public long CreatedAt { get; set; } = -1;
        public long UpdatedAt { get; set; } = -1;
        public long DeletedAt { get; set; } = -1;
        public bool IsDeleted { get; set; } = false;
        public string Other { get; set; } = string.Empty;
    }
}
