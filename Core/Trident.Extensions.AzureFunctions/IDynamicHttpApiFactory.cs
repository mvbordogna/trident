using Scholar.Framework.Core.Configuration;
using Scholar.Framework.Core.IoC;

namespace Scholar.Framework.Azure.Common
{
    public interface IDynamicHttpApiFactory
    {
        IDynamicHttpApi Get(string vesion, string resource);
    }

    public class DynamicHttpApiFactory : IDynamicHttpApiFactory
    {
        private readonly IDynamicConfiguration _configuration;
        private readonly IIoCServiceLocator _locator;

        public DynamicHttpApiFactory(IIoCServiceLocator locator)
        {
            _configuration = locator.Get<IDynamicConfiguration>();
            _locator = locator;
        }

        public IDynamicHttpApi Get(string version, string resource)
        {
            return _locator.GetNamed<IDynamicHttpApi>($"{resource}-{version}");
        }
    }
}
