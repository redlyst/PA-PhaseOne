using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerAppsCMS.Models
{
    /// <summary>
    /// Merupakan class yang digunakan untuk menampung data PRO List dari SP 
    /// </summary>
    public class ProSP
    {
        private PowerAppsCMSEntities entities = new PowerAppsCMSEntities();

        /// <summary>
        /// Method GetProByProductGroupID, method yang mengambil data PRO berdasarkan ID Produk Group
        /// </summary>
        /// <param name="productGroupID">Parameter productGroupID dengan tipe data integer</param>
        /// <returns>List/daftar PRO</returns>
        public List<PRO> GetProByProductGroupID(int productGroupID)
        {
            return entities.FunctionGetProByProductGroupID(productGroupID).ToList();
        }       
    }
}