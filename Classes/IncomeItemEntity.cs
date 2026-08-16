using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsDesk.Classes
{
    public class IncomeItemEntity
    {
        public string CatalogId { get; set; } = string.Empty;
        public int SupplierId { get; set; } = 0;

        public string Name { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Other { get; set; } = string.Empty;
        public long UpdatedAt { get; set; } = long.MaxValue;
        public override string ToString()
        {
            return Name;
        }
    }
    public class IncomeItemWithDateEntity : IncomeItemEntity
    {
        public string Date { get; set;  } = string.Empty;
    }
    public class IncomeItemWithBatchEntity : IncomeItemEntity 
    {
        public string Batches { get; set; } = string.Empty;
    }
}
