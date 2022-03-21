using Trident.Extensions.OpenApi.Visitors;

using Newtonsoft.Json.Serialization;

namespace Trident.Extensions.OpenApi.Abstractions
{
    /// <summary>
    /// This provides interfaces to the acceptor classes.
    /// </summary>
    public interface IAcceptor
    {
        /// <summary>
        /// Accepts the visitors.
        /// </summary>
        /// <param name="collection"><see cref="VisitorCollection"/> instance.</param>
        /// <param name="state"></param>
        /// <param name="namingStrategy"><see cref="NamingStrategy"/> instance.</param>
        void Accept(VisitorCollection collection, VisitorState state, NamingStrategy namingStrategy);
    }
}
