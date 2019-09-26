using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace PowerAppsCMS.Models
{
    [Serializable]
    public class MemoProxy
    {
        private Memo Memo = new Memo();
        private int selectedProduct;
        private List<MemoPRO> ListOfMemoPRO = new List<MemoPRO>();

        public MemoProxy()
        { }

        public MemoProxy(Memo memo, int selectedProduct, List<MemoPRO> listOfMemoPRO)
        {
            this.Memo = memo;
            this.SelectedProduct = selectedProduct;
            this.ListOfMemoPROs = listOfMemoPRO;

        }

        public Memo memo { get => memo; set => memo = value; }
        public int SelectedProduct { get => selectedProduct; set => selectedProduct = value; }
        public List<MemoPRO> ListOfMemoPROs { get => ListOfMemoPRO; set => ListOfMemoPRO = value; }
    }
}