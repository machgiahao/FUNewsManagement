using FUNewsManagementSystem.BusinessObject;

namespace FUNewsManagementSystem.DataAccess
{
    public interface ITagRepository
    {
        List<Tag> GetTags();
    }
}
