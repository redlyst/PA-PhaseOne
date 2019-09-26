using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerAppsCMS.Models;

namespace PowerAppsCMS.ViewModel
{
    /// <summary>
    /// view model dari component yang berisi beberapa data seperti Object component, integer MaterialID, Object list ComponentmaterialPreparationProcess,
    /// array integer SelectedMaterialPreparationProcess, IEnumberable object MaterialPreparationProcessesCollections,
    /// object list ComponentCollections, dan method ComponentViewModel
    /// </summary>
    public class ComponentViewModel
    {
        public Component Component { get; set; }
        public int MaterialID { get; set; }
        public List<ComponentMaterialPreparationProcess> ComponentMaterialPreparationProcesses { get; set; }
        public int[] SelectedMaterialPreparationProcess { get; set; }
        public IEnumerable<MaterialPreparationProcess> MaterialPreparationProcessesCollections { get; set; }
        public List<Component> ComponentCollections { get; set; }
        public ComponentViewModel()
        {
            ComponentMaterialPreparationProcesses = new List<ComponentMaterialPreparationProcess>();
        }
        
    }
}