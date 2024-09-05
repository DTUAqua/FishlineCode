using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Babelfisk.UnitTests
{
    [TestClass]
    public class Warehouse
    {
        private static int id = 0;

        public static int GetId()
        {
            return id++;
        }


        private static bool IsIdentical(Babelfisk.Warehouse.Model.Sample s, List<decimal?> lst)
        {
            if (s.SpeciesLists.Count != lst.Count)
                return false;

            int i = 0;
            foreach (var sl in s.SpeciesLists)
            {
                if (sl.raisingFactor != lst[i++])
                    return false;
            }

            return true;
        }


        private static string GetResultString(Babelfisk.Warehouse.Model.Sample sDW)
        {
            return string.Join(";", sDW.SpeciesLists.Select(x => x.raisingFactor.HasValue ? x.raisingFactor.Value.ToString().Replace(",", ".") : ""));
        }

        private static void AddWeightStep(int intStep, bool blnRep, decimal? _subSampleWeight, decimal? _landingWeight, Entities.Sprattus.SpeciesList sl, bool? _sumAnimalWeights = null)
        {
            sl.SubSample.Add(new Entities.Sprattus.SubSample() { subSampleId = GetId(), stepNum = intStep, subSampleWeight = _subSampleWeight, landingWeight = _landingWeight, sumAnimalWeights = _sumAnimalWeights, representative = (blnRep ? "Ja" : "Nej") });
        }

        [TestMethod]
        public void TestGetStock1()
        {
            List<L_Stock> stocks = new List<L_Stock>()
            {
                 new L_Stock() { L_stockId = 1, stockCode = "hom.27.3a4bc7d" },
                 new L_Stock() { L_stockId = 2, stockCode = "hom.27.3a4bc7g" },
                 new L_Stock() { L_stockId = 3, stockCode = "hom.27.2a4a5b6a7a-ce-k8" },
                 new L_Stock() { L_stockId = 4, stockCode = "cod.27.21" }
            };

            List<R_StockSpeciesArea> defs = new List<R_StockSpeciesArea>()
            {
                new R_StockSpeciesArea() { L_stockId = 1, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 1 },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 2 },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = "44G1", quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 3 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "6A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 4, speciesCode = "COD", DFUArea = "21", statisticalRectangle = null, quarter = 4 }
            };
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, stocks, defs);

            var stock = ef.GetStockCode("HMK", "21", "44G1", 4);

            Assert.IsTrue(stock == null);
        }


        [TestMethod]
        public void TestGetStock2()
        {
            List<L_Stock> stocks = new List<L_Stock>()
            {
                 new L_Stock() { L_stockId = 1, stockCode = "hom.27.3a4bc7d" },
                 new L_Stock() { L_stockId = 2, stockCode = "hom.27.3a4bc7g" },
                 new L_Stock() { L_stockId = 3, stockCode = "hom.27.2a4a5b6a7a-ce-k8" },
                 new L_Stock() { L_stockId = 4, stockCode = "cod.27.21" }
            };
           
            List<R_StockSpeciesArea> defs = new List<R_StockSpeciesArea>()
            {
                new R_StockSpeciesArea() { L_stockId = 1, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 1 },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 2 },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = "44G1", quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 3 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "6A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 4, speciesCode = "COD", DFUArea = "21", statisticalRectangle = null, quarter = 4 }
            };
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, stocks, defs);

            var stock = ef.GetStockCode("HMK", "21", "44G0", 4);

            Assert.IsTrue(stock == "hom.27.2a4a5b6a7a-ce-k8");
        }

        [TestMethod]
        public void TestGetStock3()
        {
            List<L_Stock> stocks = new List<L_Stock>()
            {
                 new L_Stock() { L_stockId = 1, stockCode = "hom.27.3a4bc7d" },
                 new L_Stock() { L_stockId = 2, stockCode = "hom.27.3a4bc7g" },
                 new L_Stock() { L_stockId = 3, stockCode = "hom.27.2a4a5b6a7a-ce-k8" },
                 new L_Stock() { L_stockId = 4, stockCode = "cod.27.21" }
            };

            List<R_StockSpeciesArea> defs = new List<R_StockSpeciesArea>()
            {
                new R_StockSpeciesArea() { L_stockId = 1, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 2 },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = "44G1", quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 3 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "6A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 4, speciesCode = "COD", DFUArea = "21", statisticalRectangle = null, quarter = 4 }
            };
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, stocks, defs);

            var stock = ef.GetStockCode("HMK", "21", "44G0", 4);

            Assert.IsTrue(stock == null);
        }

        [TestMethod]
        public void TestGetStock4()
        {
            List<L_Stock> stocks = new List<L_Stock>()
            {
                 new L_Stock() { L_stockId = 1, stockCode = "hom.27.3a4bc7d" },
                 new L_Stock() { L_stockId = 2, stockCode = "hom.27.3a4bc7g" },
                 new L_Stock() { L_stockId = 3, stockCode = "hom.27.2a4a5b6a7a-ce-k8" },
                 new L_Stock() { L_stockId = 4, stockCode = "cod.27.21" }
            };

            List<R_StockSpeciesArea> defs = new List<R_StockSpeciesArea>()
            {
                new R_StockSpeciesArea() { L_stockId = 1, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "6A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 4, speciesCode = "COD", DFUArea = "21", statisticalRectangle = null, quarter = 4 }
            };
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, stocks, defs);

            var stock = ef.GetStockCode("HMK", "21", "44G0", 4);

            Assert.IsTrue(stock == "hom.27.3a4bc7d");
        }

        /*
        [TestMethod]
        public void TestGetStock6()
        {
            
            List<L_Stock> stocks = new List<L_Stock>()
            {
                 new L_Stock() { L_stockId = 1, stockCode = "hom.27.3a4bc7d" },
                 new L_Stock() { L_stockId = 2, stockCode = "hom.27.3a4bc7g" },
                 new L_Stock() { L_stockId = 3, stockCode = "hom.27.2a4a5b6a7a-ce-k8" },
                 new L_Stock() { L_stockId = 4, stockCode = "cod.27.21" }
            };
            List<R_StockSpeciesArea> defs = new List<R_StockSpeciesArea>()
            {
                new R_StockSpeciesArea() { L_stockId = 1, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 2 },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = "44G1", quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 3 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "6A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 4, speciesCode = "COD", DFUArea = "21", statisticalRectangle = null, quarter = 4 }
            };
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, stocks, defs);

            var stock = ef.GetStockCode("HMK", "7a", null, 4);

            Assert.IsTrue(stock == "hom.27.2a4a5b6a7a-ce-k8");
            
        }*/

        [TestMethod]
        public void TestGetStock7()
        {
            List<L_Stock> stocks = new List<L_Stock>()
            {
                 new L_Stock() { L_stockId = 1, stockCode = "hom.27.3a4bc7d" },
                 new L_Stock() { L_stockId = 2, stockCode = "hom.27.3a4bc7g" },
                 new L_Stock() { L_stockId = 3, stockCode = "hom.27.2a4a5b6a7a-ce-k8" },
                 new L_Stock() { L_stockId = 4, stockCode = "cod.27.21" },
                 new L_Stock() { L_stockId = 5, stockCode = "hom.27.2a4a5b6a7a-ce-k8a" }
            };
            List<R_StockSpeciesArea> defs = new List<R_StockSpeciesArea>()
            {
                new R_StockSpeciesArea() { L_stockId = 1, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 2 },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = "44G1", quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 3 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "6A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 5, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 4, speciesCode = "COD", DFUArea = "21", statisticalRectangle = null, quarter = 4 }
            };
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, stocks, defs);

            var stock = ef.GetStockCode("HMK", "7a", null, 3);

            Assert.IsTrue(stock == "hom.27.2a4a5b6a7a-ce-k8a");
        }


        /*
        [TestMethod]
        public void TestGetStock5()
        {
            List<L_Stock> stocks = new List<L_Stock>()
            {
                 new L_Stock() { L_stockId = 1, stockCode = "hom.27.3a4bc7d" },
                 new L_Stock() { L_stockId = 2, stockCode = "hom.27.3a4bc7g" },
                 new L_Stock() { L_stockId = 3, stockCode = "hom.27.2a4a5b6a7a-ce-k8" },
                 new L_Stock() { L_stockId = 4, stockCode = "cod.27.21" }
            };

            List<R_StockSpeciesArea> defs = new List<R_StockSpeciesArea>()
            {
                new R_StockSpeciesArea() { L_stockId = 1, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 2, speciesCode = "HMK", DFUArea = "21", statisticalRectangle = "44G0", quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "6A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = null },
                new R_StockSpeciesArea() { L_stockId = 3, speciesCode = "HMK", DFUArea = "7A", statisticalRectangle = null, quarter = 4 },
                new R_StockSpeciesArea() { L_stockId = 4, speciesCode = "COD", DFUArea = "21", statisticalRectangle = null, quarter = 4 }
            };
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, stocks, defs);

            var stock = ef.GetStockCode("HMK", "21", "44G0", 4);

            Assert.IsTrue(stock == "hom.27.3a4bc7g");
        }*/


        [TestMethod]
        public void CalculateRaisingFactorsType1()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = "IND", dfuBase_Category = "IND", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 200.0M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 13.0M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 4.0M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(0, false, 1.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TBM", landingCategory = "IND", dfuBase_Category = "IND", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 28M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "DIS", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 220.0M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 0.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = "DIS", dfuBase_Category = "DIS", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 7.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "MAK", landingCategory = "DIS", dfuBase_Category = "DIS", treatment = "UR", sexCode = "F" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 7.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { Math.Round((200.0M / 13.0M) * (13.0M / 4.0M), 3), 1M, Math.Round(220.0M / 0.3M, 3), Math.Round(9M / 7.3M, 3), null };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType3()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1};
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1};

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = "IND", dfuBase_Category = "IND", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count-1].speciesListId });
            AddWeightStep(1, true, 1.15M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TBM", landingCategory = "IND", dfuBase_Category = "IND", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 28M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 0.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "IND", dfuBase_Category = "IND", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 50000M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "DIS", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 14.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "MAK", landingCategory = "DIS", dfuBase_Category = "DIS", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 7.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "DIS", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 105M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "DIS", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 30M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 8.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
           
            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 1715.266M, 80045.740M, null, 2.027M, 2.027M, null, 7.416M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType32()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.03M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.035M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.035M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 29.24M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 4.485M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KTX", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.05M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ULK", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.105M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 496M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.05M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "PLK", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.05M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.035M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "THS", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.025M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 15.143M, 15.143M, 15.143M, 98.723M, 15.143M, 15.143M, null, 15.143M, 15.143M, 15.143M, 15.143M, 15.143M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType33()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 68.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 13.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.55M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.75M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 24.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 849.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 41.077M, 11.102M, 55.027M, 11.102M, 11.102M, 11.102M, 1M, 11.102M, null };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType34()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ANA", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.469M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BMS", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 1.636M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 4.352M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 2.176M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.069M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 2.002M, 2.002M, null, 2.002M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType342()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TBM", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.07M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 3.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKR", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.455M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "MAK", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.22M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 5.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "PGH", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.965M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 137.865M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 9.62M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.37M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 136.55M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 37.19M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ULK", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.305M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SLH", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2.21M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "LSG", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 44.39M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 10.31M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 14.512M, 14.512M, 14.512M, 1, 14.512M, 1, null, 14.512M, 3.672M, 14.512M, 1, 4.306M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType35()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 689.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 0.005M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 43.78M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 5.28M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 5.45M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SFF", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.06M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3.34M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 16.45M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 2.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.69M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKR", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.315M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 118.585M, 1, 14.302M, 14.302M, 5.875M, 14.302M, 14.302M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType35X()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 613.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 60M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(0, false, 9.185M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 48, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 24, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(0, false, 8.073M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 60, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 30, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, (decimal)Math.Round((613.3 / (60 + 48 + 60)), 3), (decimal)Math.Round((613.3M / (60M + 48M + 60M)) * (48.0M / 24.0M), 3), (decimal)Math.Round((613.3M / (60M + 48M + 60M)) * (60.0M / 30.0M), 3) };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType37()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 302.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 77.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 9.27M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKR", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 15.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TNG", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 56.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.42M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KLM", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.167M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 9.125M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "STB", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 4.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 44.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 16.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 12.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 4.455M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BRS", landingCategory = null, dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.54M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 1, 1, 1, 1, 1, 3.346M, 13.730M, 1, 2.663M, 9.463M, 13.730M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType4()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 171, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "PGH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 5, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 11, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 15.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "M", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 105, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "ja", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TRB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 4.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SFF", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.075M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RTG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "GHJ", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 11, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RTG", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 9, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 13.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KNH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 18.65M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 6, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 50.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 6.65M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 43.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.15M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 6.84M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M",  cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 272.12M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 1, 1, 1, null, 1, null, 1.972M, 1, 1.972M, 1, 1, 1, 1.972M, 6.131M, 14.919M, 1.972M, 1, 1.972M, 1.972M, 1.972M, null };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType51()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 18, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KNH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.45M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TRB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 22.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TRB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 17M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TRB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 81, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 1, 1, 1, 2.051M, 2.051M, null };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType511()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 415.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 61.65M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 1.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 107.863M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 2.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "FHK", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 8.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 5.14M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 8.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KLM", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KNH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "PFF", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RTG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.04M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SLH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.206M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SPE", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 4.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TRB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ULK", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "F" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 6.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 223, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 6, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HAT", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RTG", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 7, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 102.652M, 102.628M, 1.998M, 1.998M, 1.998M, 102.702M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 1.998M, 17.287M, null, 17.287M, 1, 1, 1, 1, 1 };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType512()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.308M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.395M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.734M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 540.385M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 18.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HAT", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 22, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.465M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 24.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 12, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KLM", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.08M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.705M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "LNG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.015M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "MSJ", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.725M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.63M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.065M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SPE", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 14.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 3.96M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SVT", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.01M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 7.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            ef.CalculateRaisingFactors(sDW, s);

            //25.128 and 43.955 differs from kirstens.
            List<decimal?> lstRes = new List<decimal?>() { 28.174M, null, 28.174M, null, 3.569M, null, 3.569M, 1, 12.258M, 25.128M, 12.258M, 12.258M, 1, 1, 12.258M, 1, 43.955M, 12.258M, 1 };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        //Missing Type 5.2


        [TestMethod]
        public void CalculateRaisingFactorsType521()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2923.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HMK", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "SMÅ" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "SMÅ" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "STO" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 19.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "SMÅ" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 0.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 48.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "STO" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "SMÅ" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 4.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SPE", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "SMÅ" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "STO" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 76.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sizeSortingDFU = "SMÅ" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 10, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TRB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);


            List<decimal?> lstRes = new List<decimal?>() { null, 41.007M, 41.007M, 97.848M, 97.848M, null, 41.007M, 41.007M, 41.007M, 1M, 41.007M, 41.007M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType54()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 24, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", ovigorous = "nej", sexCode = "F", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.65M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "FJS", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR"  });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.15M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "GLY", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.05M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 5.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 3.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 30.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 17.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 10, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KLM", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 5, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KNH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 4.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RTG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SPE", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 0.35M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 17.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 7.164M, 7.164M, 1, 1, 1.903M, 1.727M, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2.571M, 1 };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType541()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 20, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 190, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "FHK", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "GLY", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KNH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "M", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "ja", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.07M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 180, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 6, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "MSJ", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 7, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 9, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "KON", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 20.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 6.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 76, 76, null, 1.739M, 1.739M, 1.739M, 1.739M, 1.739M, 1.739M, 1.739M, 1.739M, 1.739M, 38.544M, 38.544M, 38.544M, null, 1, 1, 1, 3.059M };
            string strRes = GetResultString(sDW);
            bool idential = IsIdentical(sDW, lstRes);
            Assert.IsTrue(idential);
        }



        [TestMethod]
        public void CalculateRaisingFactorsType542()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 690.55M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 4.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 31, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(2, true, 3.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "FHK", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "GLY", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 28, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(2, true, 13.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 17, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KLM", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KNH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 2.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 18M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 4.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 26.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 155, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 8.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR", sexCode = "M" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 4.6M, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 3, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 4.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RTG", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 0.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 4.5M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "KON", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 29.25M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 21.162M, null, 21.162M, 5.188M, 5.188M, 5.188M, 10.761M, 5.188M, 1, 5.188M, 5.188M, 5.188M, 5.188M, 5.188M, null, 12.016M, 12.016M, 1, 1, 1, 1, 1, 1 };
            string strRes = GetResultString(sDW);
            var idIdentical = IsIdentical(sDW, lstRes);
            Assert.IsTrue(idIdentical);
        }



        [TestMethod]
        public void CalculateRaisingFactorsType543()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 572.305M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "ja", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.12M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M", cuticulaHardness = "Soft" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.19M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 74.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej", cuticulaHardness = "Soft" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.29M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "F", ovigorous = "nej", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.72M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR", sexCode = "M", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3.69M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HSG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "HVL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 3.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "INV", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.7M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "ISG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 5.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KNH", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.9M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "KUL", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.4M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "PFF", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RSP", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RTG", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SKI", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SLB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.05M, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "THK", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.25M, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TOR", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 12.25M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TRB", landingCategory = "DIS", dfuBase_Category = "VID", treatment = "RH" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 1.1M, null, s.SpeciesList[s.SpeciesList.Count - 1]);


            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 12.463M, 12.463M, null, 12.463M, 12.463M, 12.463M, 17.857M, 17.857M, 17.857M, 17.857M, 17.857M, 17.857M, 17.857M, 17.857m, 17.857M, 17.857m, 17.857M, 17.857M, 17.857M, 17.857M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsLanType31()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 2.1M, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, null, 0.713M, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RJX", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 3.2M, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, null, 0.151M, s.SpeciesList[s.SpeciesList.Count - 1]);


            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 2.945M, 21.192M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsLanType31X()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 1.1M, 2.1M, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, null, 0.713M, s.SpeciesList[s.SpeciesList.Count - 1]);


            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RJX", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 3.2M, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, null, 0.151M, s.SpeciesList[s.SpeciesList.Count - 1]);


            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 21.192M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsLanType32()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, null, 0.713M, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DTO", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true,null, 1.257M, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RJX", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, null, 0.151M, s.SpeciesList[s.SpeciesList.Count - 1]);


            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, null, null };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsLanType3X1()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR", sexCode = "M", cuticulaHardness = "Hard" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 12.2M, s.SpeciesList[s.SpeciesList.Count - 1], true);
            AddWeightStep(1, true, null, 1.2M, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 12.2M, s.SpeciesList[s.SpeciesList.Count - 1], true);
            AddWeightStep(1, true, null, 1.2M, s.SpeciesList[s.SpeciesList.Count - 1], true);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RJX", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 12.2M, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, null, 1.2M, s.SpeciesList[s.SpeciesList.Count - 1], true);


            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { Math.Round(12.2M / 1.2M, 3), Math.Round(12.2M / 1.2M, 3), Math.Round(12.2M / 1.2M, 3) };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsLanTable()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            //Case 1
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, null, 12.2M, s.SpeciesList[s.SpeciesList.Count - 1]);

            //Case 2
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "SIL", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 12.2M, s.SpeciesList[s.SpeciesList.Count - 1]);

            //Case 3
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "RJX", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 12.2M, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, null, 1.2M, s.SpeciesList[s.SpeciesList.Count - 1]);

            //Case 4
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "BBB", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 2.3M, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, 1.2M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            //Case 5
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TGB", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2.3M, null, s.SpeciesList[s.SpeciesList.Count - 1]);
            AddWeightStep(1, true, null, 12.2M, s.SpeciesList[s.SpeciesList.Count - 1]);

            //Case 6
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "TTT", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 2.4M, s.SpeciesList[s.SpeciesList.Count - 1], true);
            AddWeightStep(1, true, 4.4M, 1.4M, s.SpeciesList[s.SpeciesList.Count - 1], true);

            //Case 7
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "YYY", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, null, 2.4M, s.SpeciesList[s.SpeciesList.Count - 1], true);
            AddWeightStep(1, true, 4.4M, null, s.SpeciesList[s.SpeciesList.Count - 1], true);

            //Case 8
            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "UUU", landingCategory = "KON", dfuBase_Category = "KON", treatment = "UR" });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2.3M, 2.4M, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 1.0M, Math.Round(12.2M / 1.2M, 3), null, null, Math.Round(2.4M / 1.4M, 3), null, 1 };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }



        [TestMethod]
        public void CalculateRaisingFactorsType61()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "HA", sizeSortingEU = null, sizeSortingDFU = null, sexCode = null, ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 14.8M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 1, sizeSortingDFU = null, sexCode = null, ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 195M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 1, sizeSortingDFU = null, sexCode = "F", ovigorous = "nej", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 4.45M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 1, sizeSortingDFU = null, sexCode = "F", ovigorous = "ja", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.35M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 1, sizeSortingDFU = null, sexCode = "M", ovigorous = "nej", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 4.45M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 2, sizeSortingDFU = null, sexCode = null, ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 38M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 2, sizeSortingDFU = null, sexCode = "F", ovigorous = "nej", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.95M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 2, sizeSortingDFU = null, sexCode = "F", ovigorous = "ja", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.030M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DVH", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = 2, sizeSortingDFU = null, sexCode = "M", ovigorous = "nej", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.45M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { 1, null, 21.081M, 21.081M, 21.081M, null, 26.573M, 26.573M, 26.573M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }


        [TestMethod]
        public void CalculateRaisingFactorsType62()
        {
            Babelfisk.Warehouse.EntityFactory ef = new Babelfisk.Warehouse.EntityFactory(null, null, null);

            var s = new Entities.Sprattus.Sample() { sampleId = 1 };
            var sDW = new Babelfisk.Warehouse.Model.Sample() { sampleId = 1 };

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = null, sizeSortingDFU = "SMÅ", sexCode = null, ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 2400.0M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = null, sizeSortingDFU = "SMÅ", sexCode = "F", ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.025M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = null, sizeSortingDFU = "SMÅ", sexCode = "F", ovigorous = "ja", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.078M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = null, sizeSortingDFU = "SMÅ", sexCode = "M", ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.283M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = null, sizeSortingDFU = "SMÅ", sexCode = "T", ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.074M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "KH", sizeSortingEU = null, sizeSortingDFU = "STO", sexCode = null, ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(0, true, 770.0M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = null, sizeSortingDFU = "STO", sexCode = "F", ovigorous = "ja", cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.799M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            s.SpeciesList.Add(new Entities.Sprattus.SpeciesList() { speciesListId = GetId(), speciesCode = "DRJ", landingCategory = "KON", dfuBase_Category = null, treatment = "UR", sizeSortingEU = null, sizeSortingDFU = "STO", sexCode = "T", ovigorous = null, cuticulaHardness = null });
            sDW.SpeciesLists.Add(new Babelfisk.Warehouse.Model.SpeciesList() { speciesListId = s.SpeciesList[s.SpeciesList.Count - 1].speciesListId });
            AddWeightStep(1, true, 0.008M, null, s.SpeciesList[s.SpeciesList.Count - 1]);

            ef.CalculateRaisingFactors(sDW, s);

            List<decimal?> lstRes = new List<decimal?>() { null, 5217.391M, 5217.391M, 5217.391M, 5217.391M, null, 954.151M, 954.151M };
            string strRes = GetResultString(sDW);
            Assert.IsTrue(IsIdentical(sDW, lstRes));
        }
    }
}
