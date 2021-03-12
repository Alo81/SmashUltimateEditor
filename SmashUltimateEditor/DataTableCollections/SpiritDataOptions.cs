﻿using SmashUltimateEditor;
using SmashUltimateEditor.DataTableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YesWeDo.DataTables.ui_spirit_db;

namespace YesWeDo.DataTableCollections
{
    public class SpiritDataOptions : BaseDataOptions, IDataOptions
    {
        private List<Spirit> _dataList;

        public List<IDataTbl> dataList { get { return _dataList.OfType<IDataTbl>().ToList(); } }
        internal static Type underlyingType = typeof(Spirit);

        public static Type GetUnderlyingType()
        {
            return underlyingType;
        }

        public Spirit GetSpiritByName(string name)
        {
            return _dataList.FirstOrDefault(x => x.ui_spirit_id == name);
        }

        public void SetData(List<IDataTbl> inSpiritBoard)
        {
            _dataList = inSpiritBoard.OfType<Spirit>().ToList();
        }

        public int GetCount()
        {
            return _dataList.Count;
        }
        public bool HasData()
        {
            return GetCount() > 0;
        }
    }
}