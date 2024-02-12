namespace TrackerApi;

public static class TrackerEndpoints
{
    public static RouteGroupBuilder MapTrackerApi(this RouteGroupBuilder group)
    {
        group.MapGet("/",
            (HttpRequest request,
            IConfiguration configuration,
            ILogger<Program> logger,
            External.IMessagePublisher publisher) =>
        {
            var trackData = new Models.TrackData
            {
                UserAgent = request?.Headers?.UserAgent,
                Referrer = request?.Headers?.Origin,
                VisitorIp = request?.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                VisitTime = DateTime.UtcNow
            };

            publisher.Publish(trackData);

            var response = configuration["Tracker:ResponseBase64"];
            return Results.Bytes(System.Text.Encoding.ASCII.GetBytes(response), contentType: "image/gif");
        }).ShortCircuit(200);

        return group;
    }
}