using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// View model dari memo yang berisikan object Memo, MemoPRO, Product, Feedback, MemoType, SelectedPRO
    /// integer ProductID, FeedbackID, MemoTypeID, list object ListofMemoPRO, ProductCompositions, ListofMemos, ListofMemoComponents, PROCollections
    /// integer array selectedProID, selectedPROComponentID, List integer SelectedComponent, dan method MemoViewModel
    /// </summary>
    public class MemoViewModel
    {
        public Memo Memo { get; set; }
        public int ProductID { get; set; }
        public int FeedbackID { get; set; }
        public int MemoTypeID { get; set; }
        public List<MemoPRO> ListofMemoPRO { get; set; }
        public MemoPRO MemoPRO { get; set; }
        public List<ProductComposition> ProductCompositions { get; set; }
        public Products Product { get; set; }
        public Feedback Feedback { get; set; }
        public MemoType MemoType { get; set; }
        public List<Memo> ListofMemos { get; set; }
        public List<MemoComponent> ListofMemoComponents { get; set; }


        public PRO SelectedPRO { get; set; }
        public int[] selectedProID { get; set; }
        public int[] selectedPROComponentID { get; set; } 
        public List<PRO> PROCollections { get; set; }
        public List<int> SelectedComponent { get; set; }

        public MemoViewModel()
        {
            Memo = new Memo();
            SelectedComponent = new List<int>();
        }
    }
}