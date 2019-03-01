using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace Trident.Common
{

    public sealed class GenericConfigSectionHandler<T> : IConfigurationSectionHandler

    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var xmlNodeReader = new XmlNodeReader(section);
            return xmlSerializer.Deserialize(xmlNodeReader);
        }

    }
}
