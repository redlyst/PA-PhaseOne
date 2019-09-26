using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// View model ProductSubGroup yang berisikan object ProductSubGroup, Pager, List object Products, dan IEnumerable object ListOfProducts
    /// </summary>
    public class ProductSubGroupViewModel
    {
        public ProductSubGroup ProductSubGroup { get; set; }
        public List<Products> Products { get; set; }

        public IEnumerable<Products> ListOfProducts { get; set; }
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// Berfunsgi sebagai model class untuk pagination di halaman product sub group
    /// </summary>
    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            
            if( startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }

        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
    }
}