using System.ComponentModel.DataAnnotations;

namespace NewsManagementMVC.Models.ViewModels.Category
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Category's name is required.")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string CategoryDescription { get; set; }

        public short ParentCaegoryId { get; set; }
        public bool isActive { get; set; }
    }
}
