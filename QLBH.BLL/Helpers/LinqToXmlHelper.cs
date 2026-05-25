using System.Xml.Linq;

namespace QLBH.BLL.Helpers
{
    public static class LinqToXmlHelper
    {
        public static XDocument ExportOrdersToXml<T>(IEnumerable<T> orders, string rootName = "Orders", string elementName = "Order")
        {
            var doc = new XDocument(
                new XElement(rootName,
                    from o in orders
                    select new XElement(elementName,
                        typeof(T).GetProperties().Select(prop =>
                            new XElement(prop.Name, prop.GetValue(o, null) ?? "")
                        )
                    )
                )
            );
            return doc;
        }
    }
}