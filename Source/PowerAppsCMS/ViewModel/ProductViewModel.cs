using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// View model dari product yang berisikan object Product, list object MasterProcesses, Components ProcessDependencies, ProductCompositions,
    /// ComponentMaterialPreparationProcesses, integer array SelectedComponentID, dan IEnumberable object ComponentCollections
    /// </summary>
    public class ProductViewModel
    {
        public Products Product { get; set; }
        public List<MasterProcess> MasterProcesses { get; set; }
        public List<Component> Components { get; set; }
        public List<ProcessDependency> ProcessDependencies { get; set; }
        public List<ProductComposition> ProductCompositions { get; set; }
        public List<ComponentMaterialPreparationProcess> ComponentMaterialPreparationProcesses { get; set; }
        public string ProductID { get; set; }
        [NotMapped]
        public int[] SelectedComponentID { get; set; }
        [NotMapped]
        public IEnumerable<Component> ComponentCollections { get; set; }
    }
}