using System;
using System.Collections.Generic;

namespace ResotelApp.ViewModels.Utils
{
    class Messaging<T> : IObservable<T>, IDisposable
    {
        private List<IObserver<T>> _observers;
        private static Messaging<T> _instance;

        private Messaging()
        {
            _observers = new List<IObserver<T>>();
        }

        public static Messaging<T> Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Messaging<T>();
                }
                return _instance;
            }
        }

        IDisposable IObservable<T>.Subscribe(IObserver<T> observer)
        {
            return Subscribe(observer);
        }

        public static IDisposable Subscribe(IObserver<T> observer)
        {
            Instance._observers.Add(observer);
            return Instance;
        }

        public static void SendMessage(T Message)
        {
            foreach(IObserver<T> observer in Instance._observers)
            {
                observer.OnNext(Message);
            }
        }

        public void Dispose()
        {
            foreach(IObserver<T> observer in _observers)
            {
                observer.OnCompleted();
            }
        }
    }
}
