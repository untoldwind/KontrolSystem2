using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPGame;

public partial class KSPGameModule {
    [KSClass("MessageBus", Description = "Shared message bus")]
    public class MessageBusAdapter {
        [KSMethod(Description = "Publish a message to anyone interested (or the void)")]
        public void Publish<T>(T message) => KSPContext.CurrentContext.MessageBus.Publish(message);

        [KSMethod(Description = "Create a subscription to a specific message type")]
        public MessageBus.Subscription<T> Subscribe<T>() => KSPContext.CurrentContext.AddSubscription<T>();
    }
}
