
namespace Assets.DesignPatterns.ObserverPattern {
    public interface Observable<T> {

        public abstract void SubscribeObserver(Observer<T> observer);
        public abstract void UnsubscribeObserver(Observer<T> observer);
        public abstract void NotifyObservers();

    }
}