using System.Threading.Tasks;
using BlazorTable.Components.ServerSide;

namespace BlazorTable.Interfaces
{
    public interface IDataLoader<T>
    {
        public Task<PaginationResult<T>> LoadDataAsync(FilterData parameters);
    }
}
