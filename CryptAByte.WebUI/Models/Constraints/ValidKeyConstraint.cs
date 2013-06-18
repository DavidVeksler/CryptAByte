using System.Web;
using System.Web.Routing;

namespace CryptAByte.WebUI.Constraints
{
    public class ValidKeyConstraint : IRouteConstraint
    {
        public bool Match
            (
                HttpContextBase httpContext, 
                Route route, 
                string parameterName, 
                RouteValueDictionary values, 
                RouteDirection routeDirection
            )
        {
            return values.ContainsKey("key") &&  values["key"].ToString().Length == 16;
        }
    }
}