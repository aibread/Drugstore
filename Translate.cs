using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drugstore
{
    class Translator
    {
        private IDictionary<string, string> SubPairs;
        public Translator()
        {
            SubPairs = new Dictionary<string, string>();
            InitTranslator();
        }

        private void InitTranslator()
        {
            SubPairs.Add("GROUP_NAME","Имя группы");
            SubPairs.Add("PRODUCT_NAME","Продукт");
            SubPairs.Add("SALES_VOLUME","Суммарные продажи");
            SubPairs.Add("NATURAL_VOLUME","Количество товаров");
            SubPairs.Add("Drugstore", "ABC анализ для медицинских товаров");
        }

        public string Translate(string word)
        {
            string s = "";
            s = SubPairs[word];            
            return s; 
        }
    }
}
