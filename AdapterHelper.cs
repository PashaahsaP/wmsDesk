/*using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WmsDesktop
{
    static class AdapterHelper
    {
        /// <summary>
        /// Словарь функций для получения данных из файла.
        /// </summary>
        /// <remarks>
        /// Ключ (int) — идентификатор типа данных.
        /// Значение (Func) — функция вида:
        /// <para>string filePath → путь к файлу</para>
        /// <para>Client client → объект клиента</para>
        /// <para>string option → дополнительная строка (например, фильтр)</para>
        /// <para>Возвращает: Task&lt;List&lt;IUiItem&gt;&gt; — список элементов UI.</para>
        /// </remarks>
        public static Dictionary<int, Func<string, Client,string, Task<ObservableCollection<IUiItem>>>> getDataFromFile = new Dictionary<int, Func<string, Client, string, Task<ObservableCollection<IUiItem>>>>
        {
            [1] = async (path, client, ip) =>
            {
                var items = new ObservableCollection<IUiItem>();
                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    var art = new List<string>();
                    var barcode = new List<string>();
                    var name = new List<string>();
                    var count = new List<string>();
                    var ws = package.Workbook.Worksheets[0];
                    int rows = ws.Dimension.End.Row;
                    int cols = ws.Dimension.End.Column;
                    for (int r = 1; r <= rows; r++)
                    {
                        for (int c = 1; c <= cols; c++)
                        {
                            var value = ws.Cells[r, c].Text.ToString();
                            var valueLength = value.Length;
                            if (c == 1 && valueLength > 0)
                            {
                                art.Add(value);
                            }
                            else if (c == 2 && valueLength > 0)
                            {
                                barcode.Add(value);
                            }
                            else if (c == 3 && valueLength > 0)
                            {
                                name.Add(value);
                            }
                            else if (c == 4 && valueLength > 0)
                            {
                                count.Add(value);
                            }

                        }
                    }
                    var result = await client.GetBarcodeAndCatalogBork(barcode, count, ip);
                    for (int r = 0; r < art.Count; r++)
                    {
                        var funcBalance = AdapterHelper.getGoodsBalance[1];
                        var temp = new BorkItem()
                        {
                            Art = art[r],
                            Name = name[r],
                            Barcode = barcode[r],
                            Count = int.Parse(count[r]),
                            Amount = result[r] != null ? await funcBalance(result[r].catalogId, client, ip): 0,
                            Catalog = result[r] != null ? new OrderItem { Id = result[r].catalogId, Name = name[r] } : null
                        };
                        items.Add(temp);
                    }
                    
                    return items;
                }
            },
            [0] = async (path, client, ip) =>
            {
                var items = new ObservableCollection<IUiItem>();
                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    var art = new List<string>();
                    var barcode = new List<string>();
                    var name = new List<string>();
                    var count = new List<string>();
                    var ws = package.Workbook.Worksheets[0];
                    int rows = ws.Dimension.End.Row;
                    int cols = ws.Dimension.End.Column;
                    for (int r = 1; r <= rows; r++)
                    {
                        for (int c = 1; c <= cols; c++)
                        {
                            var value = ws.Cells[r, c].Text.ToString();
                            var valueLength = value.Length;
                            if (c == 2 && valueLength > 0)
                            {
                                art.Add(value);
                            }
                            else if (c == 4 && valueLength > 0)
                            {
                                barcode.Add(value);
                            }
                            else if (c == 1 && valueLength > 0)
                            {
                                name.Add(value);
                            }
                            else if (c == 3 && valueLength > 0)
                            {
                                count.Add(value);
                            }

                        }
                    }
                    var result = await client.GetBarcodeAndCatalogAtomy(barcode, count, ip);
                    for (int r = 0; r < art.Count; r++)
                    {
                        var funcBalance = AdapterHelper.getGoodsBalance[0];
                        var temp = new AtomyItem()
                        {
                            TE = barcode[r],
                            Name = name[r],
                            Barcode = art[r],
                            Count = int.Parse(count[r]),
                            Amount = result[r] != null ?  await funcBalance(barcode[r], client, ip): 0,
                            Catalog = result[r] != null ? new OrderItem { Id = result[r].id, Name = name[r] } : null
                        };
                        items.Add(temp);
                    }

                    return items;
                }
            }
        };
        /// <summary>
        /// Получение остатка по товару
        /// </summary>
        /// <para>CatalogId</para>
        /// <para>Client</para>
        /// <para>Ip</para>
        public static Dictionary<int, Func<string, Client, string, Task<int>>> getGoodsBalance = new Dictionary<int, Func<string, Client, string, Task<int>>>
        {
            [1] = async (orderItem, client, ip) =>
            {
                return await client.GetGoodsBorkBalance(orderItem, ip);
            },
            [0] = async (TE, client, ip) =>
            {
                var goods = await client.GetAtomyGoodsByTE(TE, ip);
                var cell = await client.GetCellIdById(goods.cellId, ip);
                if(cell != null && cell.name.ToString() != "Z900")
                {
                    return 1;
                }
                return 0;
            }
        };

        public static Dictionary<int, Func<Client, ObservableCollection<IUiItem>, string, int, int, int, Task<int>>> createAssebmlySession = new Dictionary<int, Func<Client, ObservableCollection<IUiItem>, string, int, int, int, Task<int>>>
        { 
            [1] = async (client, items, ip, countOfItems, linesOfItem, supplier) =>
            {
                var sessionId = await client.SendAssemlbySession(
                new AssemblySession
                {
                    supplierId = 2,
                    outCell = 1,//TODO make selection cell
                    status = "created",
                    date = "25.08.2025",//TODO set date outship
                    createdAt = DateTime.Now.ToShortDateString(),
                    finishedAt = "",
                    amount = countOfItems,
                    lines = linesOfItem
                }, ip);
                List<AssemblyBorkItem> assemblyItems = new List<AssemblyBorkItem>();
                ID cellId = await client.GetCellIdByName("Z900", ip);
                if (cellId == null)
                {
                    cellId = await client.SendCell("Z900", ip);
                }
                foreach (var item in items)
                {

                    List<GoodsBorkItem> collection = await client.GetAllBorkGoodsByCatalogId(item.Catalog.Id.ToString(), ip);
                    List<GoodsBorkItem> changedCol = new List<GoodsBorkItem>();
                    collection = collection.Where(i => i.cellId != cellId.id.ToString()).ToList();
                    int amount = item.Count;
                    foreach (var elem in collection)
                    {
                        if (elem.amount == amount)
                        {
                            assemblyItems.Add(new AssemblyBorkItem
                            {
                                assemblyId = Int32.Parse(sessionId.id.ToString()),
                                cellId = Int32.Parse(elem.cellId),
                                goodsId = Int32.Parse(elem.id),
                                finishedAt = "",
                                startedAt = "",
                                status = "created"
                            });
                            amount = amount - elem.amount;
                            await client.UpdateBorkGoodsAsync(new GoodsBorkItem()
                            {
                                id = elem.id,
                                amount = elem.amount,
                                catalogId = elem.catalogId,
                                createdAt = elem.createdAt,
                                cellId = cellId.id.ToString(),
                            }, ip);
                            break;
                        }
                    }
                    foreach (var elem in collection)
                    {
                        if (elem.amount < amount)
                        {
                            assemblyItems.Add(new AssemblyBorkItem
                            {
                                assemblyId = Int32.Parse(sessionId.id.ToString()),
                                cellId = Int32.Parse(elem.cellId),
                                goodsId = Int32.Parse(elem.id),
                                finishedAt = "",
                                startedAt = "",
                                status = "created"
                            });
                            await client.UpdateBorkGoodsAsync(new GoodsBorkItem()
                            {
                                id = elem.id,
                                amount = elem.amount,
                                catalogId = elem.catalogId,
                                createdAt = elem.createdAt,
                                cellId = cellId.id.ToString(),
                            }, ip);

                            amount = amount - elem.amount;
                        }
                        else { changedCol.Add(elem); }

                    }
                    foreach (var elem in changedCol)
                    {
                        if (amount == 0)
                            break;
                        if (elem.amount >= amount)
                        {
                            //Если больше надо создать новый элемент с равным количеством, поместить на z900, создать к ниму goodsItem, обновить оставшийся goods 
                            //create new goods
                            var splittedGoods = new GoodsBorkItem()
                            {
                                id = null,
                                catalogId = elem.catalogId,
                                cellId = cellId.id.ToString(),
                                createdAt = DateTime.Now.ToString(),
                                amount = amount,
                            };
                            var goodsId = await client.SendGoodsBork(splittedGoods, ip);
                            //update old goods
                            var oldGoods = new GoodsBorkItem()
                            {
                                id = elem.id,
                                catalogId = elem.catalogId,
                                cellId = elem.cellId,
                                createdAt = elem.createdAt,
                                amount = elem.amount - amount,
                            };
                            var oldId = client.UpdateBorkGoodsAsync(oldGoods, ip);
                            //set in session id of goods
                            assemblyItems.Add(new AssemblyBorkItem
                            {
                                assemblyId = Int32.Parse(sessionId.id.ToString()),
                                cellId = Int32.Parse(elem.cellId),
                                goodsId = Int32.Parse(goodsId.id.ToString()),
                                finishedAt = "",
                                startedAt = "",
                                status = "created"
                            });
                            amount = amount - elem.amount;
                            break;
                        }

                    }

                }
                foreach (var item in assemblyItems)
                {
                    var id = client.SendAssemblyItemBork(item, ip);
                }
                MessageBox.Show($"Заказ создан! Количество:{countOfItems} Количество строк:{linesOfItem}");
                return 1;
            },
            [0] = async (client, items, ip, countOfItems, linesOfItem, supplier) =>
            {
                var sessionId = await client.SendAssemlbySession(
                new AssemblySession
                {
                    supplierId = 1,
                    outCell = 1,//TODO make selection cell
                    status = "created",
                    date = "25.08.2025",//TODO set date outship
                    createdAt = DateTime.Now.ToShortDateString(),
                    finishedAt = "",
                    amount = countOfItems,
                    lines = linesOfItem
                }, ip);
                List<AssemblyAtomyItem> assemblyItems = new List<AssemblyAtomyItem>();
                ID cellId = await client.GetCellIdByName("Z900", ip);
                if (cellId == null)
                {
                    cellId = await client.SendCell("Z900", ip);
                }

                foreach (var item in items)
                {
                    var goodsItem = item as AtomyItem;
                    AtomyGoodsItem goods = await client.GetAtomyGoodsByTE(goodsItem.TE, ip);
                    if (goods.cellId != cellId.id.ToString())
                    {
                        //create new assembly item
                        var newAssemblyItem = new AssemblyBorkItem
                        {
                            assemblyId = Int32.Parse(sessionId.id.ToString()),
                            cellId = Int32.Parse(goods.cellId),
                            goodsId = Int32.Parse(goods.id),
                            finishedAt = "",
                            startedAt = "",
                            status = "created"
                        };
                        var id = client.SendAssemblyItemAtomy(newAssemblyItem, ip);

                        //update goods cellId
                        await client.UpdateAtomyGoodsAsync(
                            new AtomyGoodsItem()
                            {
                                id = goods.id,
                                amount = goods.amount,
                                catalogId = goods.catalogId,
                                createdAt = goods.createdAt,
                                TE = goods.TE,
                                date = goods.date,
                                cellId = cellId.id.ToString(),

                            }, ip);
                    }


                }
                return 1;
            }
        };

        
    }
}
*/