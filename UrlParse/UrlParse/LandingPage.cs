using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlParse
{
    class LandingPage
    {
        private string _url, _stm_type, _stm_source, _stm_sku, _product_id, _creative, _targetid, _feeditemid, _matchtype;


        public LandingPage() { }

        public string url
        {
            get { return this._url; }
            set { this._url = value; }
        }

        public string stm_type
        {
            get { return this._stm_type; }
            set { this._stm_type = value; }
        }

        public string stm_source
        {
            get { return this._stm_source; }
            set { this._stm_source = value; }
        }

        public string stm_sku
        {
            get { return this._stm_sku; }
            set { this._stm_sku = value; }
        }

        public string product_id
        {
            get { return this._product_id; }
            set { this._product_id = value; }
        }

        public string creative
        {
            get { return this._creative; }
            set { this._creative = value; }
        }

        public string targetid
        {
            get { return this._targetid; }
            set { this._targetid = value; }
        }
        public string feeditemid
        {
            get { return this._feeditemid; }
            set { this._feeditemid = value; }
        }

        public string matchtype
        {
            get { return this._matchtype; }
            set { this._matchtype = value; }
        }
    }
}
