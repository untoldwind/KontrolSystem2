using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.Core;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPGame;

public static class MessageBusBinding {
    public static readonly BoundType SubscriptionType = Direct.BindType("ksp::game", "Subscription",
        "Central message bus", typeof(MessageBus.Subscription<>),
        [],
        [],
        new() {
            {
                "peek", new BoundMethodInvokeFactory("Peek for next message without consuming it", true,
                    () => new OptionType(new GenericParameter("T")), () => [], false,
                    typeof(MessageBus.Subscription<>), typeof(MessageBus.Subscription<>).GetMethod("Peek"))
            }, {
                "recv", new BoundMethodInvokeFactory("Receive next message", true,
                    () => new OptionType(new GenericParameter("T")), () => [], false,
                    typeof(MessageBus.Subscription<>), typeof(MessageBus.Subscription<>).GetMethod("Recv"))
            }, {
                "unsubscribe", new BoundMethodInvokeFactory(
                    "Unsubscribe from the message bus. No further messages will be received", true,
                    () => BuiltinType.Unit, () => [], false,
                    typeof(MessageBus.Subscription<>), typeof(MessageBus.Subscription<>).GetMethod("Unsubscribe"))
            }
        },
        new() {
            {
                "has_messages", new BoundPropertyLikeFieldAccessFactory("Check if subscription has pending messages",
                    () => BuiltinType.Bool, typeof(MessageBus.Subscription<>), typeof(MessageBus.Subscription<>).GetProperty("HasMessages"))
            }
        });

    private static List<BoundType>? messageBusTypes;

    internal static IEnumerable<BoundType> MessageBusTypes {
        get {
            if (messageBusTypes == null) {
                messageBusTypes = [SubscriptionType];
                foreach (var type in messageBusTypes) {
                    BindingGenerator.RegisterTypeMapping(type.runtimeType, type);
                }
            }

            return messageBusTypes;
        }
    }
}
