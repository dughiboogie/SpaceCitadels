
namespace Assets.DesignPatterns.ObserverPattern {
    public interface Observer<T> {

        public void SendUpdate(T notifyObject);
    }
}