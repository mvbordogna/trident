using System;
using System.Threading.Tasks;

namespace Trident.Contracts.Api.Client
{
    public interface IServiceProxyBase<TModel, TId> : IReadOnlyServiceBase<TModel, TId>
         where TModel : class, IGuidModelBase
    {
        Task<Response<TModel>> Update(TModel model);
        Task<Response<TModel>> Delete(TModel model);
        Task<Response<TModel>> Create(TModel model);
        Task<Response<TModel>> Patch(Guid id, TModel model);
    }
}
