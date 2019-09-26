using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.Services
{
    /// <summary>
    /// Berfungsi sebagai class untuk menyimpan data sementara dari session
    /// </summary>
    [Serializable]
    public class NewMemoServices
    {
        private Memo memo = new Memo();
        private int selectedProduct;
        private List<MemoPRO> ListOfMemoPRO = new List<MemoPRO>();
        private string message;

        public NewMemoServices()
        { }

        public NewMemoServices(Memo memo, int selectedProduct, List<MemoPRO> listOfMemoPRO, string message)
        {
            this.Memo = memo;
            this.SelectedProduct = selectedProduct;
            this.ListOfMemoPROs = listOfMemoPRO;
            this.Message = message;
            
        }

        public Memo Memo { get => memo; set => memo = value; }
        public int SelectedProduct { get => selectedProduct; set => selectedProduct = value; }
        public List<MemoPRO> ListOfMemoPROs { get => ListOfMemoPRO; set => ListOfMemoPRO = value; }
        public string Message { get => message; set => message = value; }
    }
}