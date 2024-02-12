namespace External
{
    using Models;
    using System.Threading.Tasks;

    public interface IMessagePublisher
    {
        Task Publish(TrackData trackData);
    }
}