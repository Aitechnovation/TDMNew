﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TDM.Models.ViewModels
{
    public class MAP_ViewModel
    {

        public string Name { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal ParcelPrice { get; set; }


        public MapStructureInfo MapStructure { get; set; }

    }
    public class MapStructureInfo
    {
        public string ParcelDrawingCode { get; set; }
        public string MarketDrawingCode { get; set; }
        public string Shape { get; set; }
    }
}