using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace ArbuzApi.Helpers
{
    public static class HtmlHelpers
    {
        public static IEnumerable<HtmlNode> DivsByClass(this HtmlNode node, string className)
        {
            var descendants = node.Descendants("div").Where(d => d.GetAttributeValue("class", "").Contains(className));
            return descendants;
        }
    }
}
