using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using WmsDesk.Classes;

namespace WmsDesk.Converter
{
    public static class Mapper
    {
        public static IncomeItemVm ToVm(this IncomeItemEntity item)
        {
            if(item is IncomeItemWithDateEntity)
            {
                return new IncomeItemWithDateVm()
                {
                    CatalogId = item.CatalogId,
                    Name = item.Name,
                    Other = item.Other,
                    Sku = item.Sku,
                    Date = DateTime.ParseExact((item as IncomeItemWithDateEntity).Date, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                    TE = "345"

                };
            }
            else if (item is IncomeItemWithBatchEntity)
            {
                return new IncomeItemWithBatchVm()
                {
                    CatalogId = item.CatalogId,
                    Name = item.Name,
                    Other = item.Other,
                    Sku = item.Sku,
                    Batches = (item as IncomeItemWithBatchEntity).Batches,
                    TE = "345"
                    
                };
            }
            else
            {
                return new IncomeItemVm()
                {
                    CatalogId = item.CatalogId,
                    Name = item.Name,
                    Other = item.Other,
                    Sku = item.Sku,
                    TE = "345",
                };
            }
            
        }
        public static List<IncomeItemVm> ToVmList(this List<IncomeItemEntity> collection)
        {
            var result = new List<IncomeItemVm>(); 
            foreach (var item in collection)
            {
                result.Add(item.ToVm());
            }
            return result;
        }
        public static List<IncomeItemVm> ToVmList(this IEnumerable<IncomeItemEntity> collection)
        {
            var result = new List<IncomeItemVm>();
            foreach (var item in collection)
            {
                result.Add(item.ToVm());
            }
            return result;
        }
        public static IncomeItemEntity ToEntity(this IncomeItemVm item, List<IncomeItemEntity> catalogData)
        {
            return new IncomeItemEntity()
            {
                CatalogId = item.CatalogId,
                Name = item.Name,
                Other = item.Other,
                Sku = item.Sku,
                SupplierId = catalogData.First(inner => inner.CatalogId == item.CatalogId).SupplierId,
                SupplierName = catalogData.First(inner => inner.CatalogId == item.CatalogId).SupplierName
            };
        }
        public static List<IncomeItemEntity> ToEntityList(this List<IncomeItemVm> item)
        {
            return new List<IncomeItemEntity>();
        }
        public static List<IncomeItemEntity> ToEntityList(this IEnumerable<IncomeItemVm> collection, List<IncomeItemEntity> catalogData)
        {
            var result =  new List<IncomeItemEntity>();
            foreach (var item in collection)
            {
                result.Add(item.ToEntity(catalogData));
            }
            return result;
        }
    }
}
