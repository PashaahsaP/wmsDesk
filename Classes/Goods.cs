using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsDesk.Classes
{
    internal class Goods
    {
        public string id { get; set; } = string.Empty;
        public int amount { get; set; } = -1;
        public string cellId { get; set; } = string.Empty;
        public string catalogId { get; set; } = string.Empty;
        public long createdAt { get; set; } = -1;
        public bool isAvailable{  get; set; } = false;
        public long updatedAt { get; set;  } = -1;
        public long? deletedAt { get; set;  } = -1;
        public bool isDeleted{ get; set; } = false;
        public string other{ get; set; } = string.Empty;
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        // ТИПИЗИРОВАННЫЙ МЕТОД (Рекомендуется для удобства)
        // Избавляет от необходимости постоянно писать приведение типов (Goods) экземпляр.Clone()
        public Goods CloneGoods()
        {
            return (Goods)this.MemberwiseClone();
        }
    }
}
