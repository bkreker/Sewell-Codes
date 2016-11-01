using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMining
{
    public class Headers : IEnumerable, IEnumerator
    {
        public int Word_ColIndex { get; set; }
        public int Query_ColIndex { get; set; }
        public int Cost_ColIndex { get; set; }
        public int GP_ColIndex { get; set; }
        public int NetProfit_ColIndex { get; set; }
        public int ROI_ColIndex { get; set; }
        public int NPPerConv_ColIndex { get; set; }
        public int GPPerConv_ColIndex { get; set; }
        public int Conversions_ColIndex { get; set; }
        public int Clicks_ColIndex { get; set; }
        public int Impressions_ColIndex { get; set; }
        public int ConvValPerCost_ColIndex { get; set; }
        public int CTR_ColIndex { get; set; }
        public int AvgCPC_ColIndex { get; set; }
        public int AvgPosition_ColIndex { get; set; }
        public int CostPerConv_ColIndex { get; set; }
        public int ConvRate_ColIndex { get; set; }
        public int ViewThroughConv_ColIndex { get; set; }
        int position = -1;
        public object Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
