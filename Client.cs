using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using WmsDesk;
using WmsDesk.Classes;
using WmsDesk.ViewModels;

namespace WmsDesk
{
    record Result(bool success);
    public class Client
    {
        private readonly HttpClient client = new HttpClient();
        
        internal async Task<Result> CreateAssebmlySession(AssemblySession session, List<Goods> goods, List<Goods> updateGoods, List<SessionItem> incomeItems,  string ip)
        {

            Result result;
            try
            {
                var payload = new
                {
                    session = session,
                    goods = goods,
                    goodsForUpdate = updateGoods,
                    incomeItems = incomeItems,
                };                
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/assembly_session/pc", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Result>(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Отправка данных для сессии на сборку", ex.Message);
                Log.CloseAndFlush();
                throw;
            }

            return result;
        }
        internal async Task<StringID> SendCellName(string name, string ip)
        {
            StringID result = null;
            try
            {
                var json = JsonConvert.SerializeObject(new Name() { name = name });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/cell/name", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<StringID>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<StringID> SendTE(string name, string parentCellId, string cellType, string ip)
        {
            StringID result = null;
            try
            {
                var json = JsonConvert.SerializeObject(new { name = name, parentCellId = parentCellId, cellType = cellType });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/cell/te", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<StringID>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<Cell> GetCellIdByName(string cellName, string ip)
        {
            Cell result = null;
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/cell/name/{cellName}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Cell>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<Cell> GetCellIdById(string cellId, string ip)
        {
            Cell result = null;
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/cell/id/{cellId}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Cell>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<ID> SendAssemlbySession(AssemblySession session, string ip)
        {

            ID sessionId = null;
            try
            {
                var json = JsonConvert.SerializeObject(session);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/assembly_session", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                sessionId = JsonConvert.DeserializeObject<ID>(data);
            }
            catch (Exception ex)
            {
                return sessionId;
            }

            return sessionId;
        }
        internal async Task<StringID> SendIncomeSession(IncomeSession session, string ip)
        {

            StringID sessionId = null;
            try
            {
                var json = JsonConvert.SerializeObject(session);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/income_session", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                sessionId = JsonConvert.DeserializeObject<StringID>(data);
            }
            catch (Exception ex)
            {
                return sessionId;
            }

            return sessionId;
        }
        internal async Task<StringID> SendIncomeItem(IncomeItem item, string ip)
        {

            StringID sessionId = null;
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/income_item", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                sessionId = JsonConvert.DeserializeObject<StringID>(data);
            }
            catch (Exception ex)
            {
                return sessionId;
            }

            return sessionId;
        }
        /*internal async Task<Int32> GetGoodsBorkBalance(string catalogId, string ip)
        {
       *//*     StringID cellId = await GetCellIdByName("Z900", ip);
            if (cellId == null)
            {
                cellId = await SendCellName("Z900", ip);
            }
            List<GoodsBorkItem> collection = await GetAllBorkGoodsByCatalogId(catalogId, ip);
            collection = collection.Where(i => i.cellId != cellId.id.ToString()).ToList();
            var count = collection.Sum(sum => sum.amount);
            return count;*//*
        }*/
        internal async Task<List<BarcodeItem>> GetBarcodeAndCatalogBork(List<string> barcodes, List<string> count, string ip)
        {
            var resultOfResponse = new List<BarcodeItem>();
            foreach (var barcode in barcodes)
            {
                try
                {
                    var response = await client.GetAsync($"http://{ip}:3000/barcodeBork/name/{barcode}");
                    response.EnsureSuccessStatusCode();
                    string data = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<BarcodeItem>(data);
                    resultOfResponse.Add(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return resultOfResponse;
        }
        internal async Task<List<AtomyGoodsItem>> GetBarcodeAndCatalogAtomy(List<string> barcodes, List<string> count, string ip)
        {
            var resultOfResponse = new List<AtomyGoodsItem>();
            foreach (var barcode in barcodes)
            {
                try
                {
                    var response = await client.GetAsync($"http://{ip}:3000/goods_atomy/TE/{barcode}");
                    response.EnsureSuccessStatusCode();
                    string data = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AtomyGoodsItem>(data);
                    resultOfResponse.Add(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return resultOfResponse;
        }
        internal async Task<BarcodeItem> GetBarcodeByName(string barcode, string ip)
        {
            BarcodeItem barcodeResult = null;
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/barcodeBork/name/{barcode}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BarcodeItem>(data);
                barcodeResult = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return barcodeResult;
        }
        internal async Task<List<GoodsBorkItem>> GetAllBorkGoodsByCatalogId(string catalogId, string ip)
        {
            List<GoodsBorkItem> barcodeResult = null;
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/goods_bork/catalogId/{catalogId}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<GoodsBorkItem>>(data);
                barcodeResult = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return barcodeResult;
        }
        internal async Task<AtomyGoodsItem> GetAtomyGoodsByTE(string TE, string ip)
        {
            AtomyGoodsItem barcodeResult = null;
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/goods_atomy/TE/{TE}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AtomyGoodsItem>(data);
                barcodeResult = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return barcodeResult;
        }
        /*internal async Task<BorkCatalog> GetCatalogBorkByName(string catalogName, string ip)
        {
            BorkCatalog barcodeResult = null;
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/catalogsBorks/name/{catalogName.Replace("/", "%2F")}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BorkCatalog>(data);
                barcodeResult = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return barcodeResult;
        }*/
        internal async Task<string> SendCatalog(Catalog catalog, string ip)
        {
            var resultId = "";
            try
            {
                var json = JsonConvert.SerializeObject(catalog);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/catalog/nameAndSupplier", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                resultId = JsonConvert.DeserializeObject<StringID>(data).id;
            }
            catch (Exception ex)
            {
            }

            return resultId;
        }
        internal async Task<StringID> SendGoods(Goods goods, string ip)
        {
            StringID result = null;
            try
            {
                var json = JsonConvert.SerializeObject(goods);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/goods", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<StringID>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<string> SendAssemblyItemBork(AssemblyBorkItem asItem, string ip)
        {
            string result = null;
            try
            {
                var json = JsonConvert.SerializeObject(asItem);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/assemblyBorkItem", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<string>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<string> SendAssemblyItemAtomy(AssemblyBorkItem asItem, string ip)
        {
            string result = null;
            try
            {
                var json = JsonConvert.SerializeObject(asItem);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/assemblyAtomyItem", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<string>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<string> UpdateCatalog(CatalogItemBase item, string ip)
        {
            string result = null;
            StringID[] str = null;
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"http://{ip}:3000/catalog/update/", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                str  = JsonConvert.DeserializeObject<StringID[]>(data);
            }
            catch (Exception ex)
            {
            }

            return str[0].id;
        }
        internal async Task<string> UpdateAtomyGoodsAsync(AtomyGoodsItem item, string ip)
        {
            string result = null;
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"http://{ip}:3000/goodsAtomy/update/", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<string>(data);
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        internal async Task<string> SendBarcode(BarcodeItem barcode, string ip)
        {
            string barcodeId = null;
            try
            {
                var json = JsonConvert.SerializeObject(barcode);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/barcode/nameAndCatalogAndSup", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                barcodeId = JsonConvert.DeserializeObject<StringID>(data).id;
            }
            catch (Exception ex)
            {

            }

            return barcodeId;
        }

        internal async Task<int> SendBatch(Batch batch, string ip)
        {
            int barcodeId = -1;
            try
            {
                var json = JsonConvert.SerializeObject(batch);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{ip}:3000/batch/", content);
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                barcodeId = JsonConvert.DeserializeObject<int>(data);
                Console.WriteLine();
            }
            catch (Exception ex)
            {

            }

            return barcodeId;
        }
        internal async Task<string> GetAllCatalogsWithSuppliers(string ip, long time)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/catalogsAndSuppliers/{time}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetSuppliers(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/suppliers/catalog/0");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetGoodsIncomeSession(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/goods/incomeSession/0");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetAllGoods(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/goods/0");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetBarcodes(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/barcodes/catalog/0");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> RemoveBarcode(Barcode item, string ip)
        {
            string result = null;
            StringID[] str = null;
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var response = await client.DeleteAsync($"http://{ip}:3000/catalog/delete/{item.Id}");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                str = JsonConvert.DeserializeObject<StringID[]>(data);
            }
            catch (Exception ex)
            {
            }

            return str[0].id;
        }
        internal async Task<string> GetCellsIncomeSession(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/cells/incomeSession/0");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetIncomeCells(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/incomeCell/");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetOutcomeCells(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/outcomeCell/");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetCells(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/cells/0");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetCellTypes(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/cellTypesIncomeSession/0");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        internal async Task<string> GetBatches(string ip)
        {
            var result = "";
            try
            {
                var response = await client.GetAsync($"http://{ip}:3000/batches/");
                response.EnsureSuccessStatusCode();
                string data = await response.Content.ReadAsStringAsync();
                result = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
    }

}
