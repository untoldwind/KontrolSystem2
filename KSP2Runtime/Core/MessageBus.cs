using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.Core;

public class MessageBus {
    public abstract class Subscription {
        public abstract Type MessageType { get; }

        public abstract void Unsubscribe();
        
        internal abstract void Publish(object? message);
    }

    public class Subscription<T>(MessageBus messageBus) : Subscription {
        private readonly Queue<T> inbox = new();

        public override Type MessageType => typeof(T);

        internal override void Publish(object? message) {
            if (message is not T typedMessage) return;
            lock (inbox) {
                inbox.Enqueue(typedMessage);
            }
        }

        public Option<T> Peek() {
            lock (inbox) {
                return inbox.TryPeek(out var message) ? Option.Some(message) : Option.None<T>();
            }
        }

        public Option<T> NextMessage() {
            lock (inbox) {
                return inbox.TryDequeue(out var message) ? Option.Some(message) : Option.None<T>();
            }
        }

        public override void Unsubscribe() {
            messageBus.Unsubscribe(this);
        }
    }

    private readonly Dictionary<Type, HashSet<Subscription>> subscriptions = new();

    public void Publish<T>(T message) {
        lock (subscriptions) {
            if (subscriptions.TryGetValue(typeof(T), out var typeSubscriptions)) {
                foreach (var subscription in typeSubscriptions) {
                    subscription.Publish(message);
                }
            }
        }
    }

    public Subscription<T> Subscribe<T>() {
        var subscription = new Subscription<T>(this);
        lock (subscriptions) {
            if (subscriptions.TryGetValue(typeof(T), out var typeSubscriptions)) {
                typeSubscriptions.Add(subscription);
            } else {
                subscriptions.Add(typeof(T), [subscription]);
            }
        }

        return subscription;
    }

    public bool Unsubscribe(Subscription subscription) {
        lock (subscriptions) {
            if (subscriptions.TryGetValue(subscription.MessageType, out var typeSubscriptions)) {
                return typeSubscriptions.Remove(subscription);
            }
        }

        return false;
    }
}
