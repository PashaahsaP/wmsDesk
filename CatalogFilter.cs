using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsDesk.Classes;

 

namespace WmsDesk
{
        public class CatalogFilter
        {
        public string Sort { get; set; } = "";
        public Supplier Supplier { get; set; }
        public string Text { get; set; }
        public List<CatalogItemBase> Items { get; set; }
        public List<Barcode> Barcodes { get; set; }
        public CatalogFilter(List<CatalogItemBase> items, List<Barcode> barcodes)
        {
            Items = items;
            Barcodes = barcodes;
        }
        /// <summary>
        /// Apply filters and return data
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<CatalogItemBase> Apply()
        {
            var result = new ObservableCollection<CatalogItemBase>(Items);

            //apply supplier filter
            if (Supplier != null && Supplier.Id != -1 && Supplier.Name != "")
                result = new ObservableCollection<CatalogItemBase>(Items.Where(item => item.SupplierId == Supplier.Id));


            //apply text sorting
            if (Sort != "")
                result = new ObservableCollection<CatalogItemBase>(result.Where(item =>
                {
                    var barcodes = new List<Barcode>(Barcodes.Where(it =>
                        it.CatalogId == item.CatalogId
                        && it.Name.Contains(Sort.ToLower())));

                    return item.Name.ToLower().Contains(Sort.ToLower())
                    || item.Sku.ToLower().Contains(Sort.ToLower())
                    || barcodes.Count != 0;
                }));

            return result;
        }

    }
}

