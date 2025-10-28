namespace BugStore.Endpoints
{
    public static class EndpointsExtension
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapCustomerEndpoints();
            app.MapProductEndpoints();
            app.MapOrderEndpoints();
        }
    }
}