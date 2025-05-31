using FUNewsManagementSystem.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DataAccess
{
    public class TagRepository : ITagRepository
    {
        public List<Tag> GetTags()
        {
            var listTags = new List<Tag>();
            try
            {
                using var context = new FunewsManagementContext();
                listTags = context.Tags.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCategories: " + ex.Message);

            }
            return listTags;
        }
    }
}
