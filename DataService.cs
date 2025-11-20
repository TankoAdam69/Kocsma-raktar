using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kocsma_raktár
{
    public class DataService
    {
        private readonly string stockPath = "Data/stock.json";
        private readonly string categoryPath = "Data/categories.json";

        public List<DrinkItem> LoadStock()
        {
            if (File.Exists(stockPath)) return new List<DrinkItem>();
            return JsonSerializer.Deserialize<List<DrinkItem>>(File.ReadAllText(path: stockPath));
        }

        public void SaveStock(List<DrinkItem> stock)
        {
            File.WriteAllText(stockPath, JsonSerializer.Serialize(stock, new JsonSerializerOptions { WriteIndented = true }));
        }

        public List<string> LoadCategories()
        {
            if (!File.Exists(categoryPath)) return new List<string>();
            return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(categoryPath));
        }
    }
}
