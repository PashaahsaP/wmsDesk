using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsDesk.Classes
{
    public class IncomeSession
    {
        public string Id { get; set; } = ";";
        public int SupplierId { get; set; } = -1;
        public string IncomeCellId { get; set; } = "";
        public string ToCellId { get; set; } = "";
        public int Status { get; set; } = -1;
        public long CreatedAt { get; set; } = -1;
        public long StartedAt { get; set; } = -1;
        public long FinishedAt { get; set; } = -1;
        public long UpdatedAt { get; set; } = -1;
        public long DeletedAt { get; set; } = -1;
        public bool IsDeleted { get; set; } = false;
        public string Other { get; set; } = "";

    }
}
