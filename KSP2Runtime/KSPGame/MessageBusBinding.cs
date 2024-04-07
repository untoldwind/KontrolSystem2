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

    public static readonly BoundType EventProcessStartedType = Direct.BindType("ksp::game", "EventProcessStarted",
        "Process started event will be published to message bus when a process is started",
        typeof(MainframeEvents.ProcessStarted),
        [],
        [],
        new(),
        new() {
            {
                "name", new BoundPropertyLikeFieldAccessFactory("Process name", () => BuiltinType.String,
                typeof(MainframeEvents.ProcessStarted), typeof(MainframeEvents.ProcessStarted).GetProperty("Name"))
            },
            {
                "arguments", new BoundPropertyLikeFieldAccessFactory("Process start arguments", () => new ArrayType(BuiltinType.String),
                    typeof(MainframeEvents.ProcessStarted), typeof(MainframeEvents.ProcessStarted).GetProperty("Arguments"))
            }
        });

    public static readonly BoundType EventProcessStoppedType = Direct.BindType("ksp::game", "EventProcessStopped",
        "Process stop event will be published to message bus when a process is stopped",
        typeof(MainframeEvents.ProcessStopped),
        [],
        [],
        new(),
        new() {
            {
                "name", new BoundPropertyLikeFieldAccessFactory("Process name", () => BuiltinType.String,
                    typeof(MainframeEvents.ProcessStopped), typeof(MainframeEvents.ProcessStopped).GetProperty("Name"))
            },
            {
                "error", new BoundPropertyLikeFieldAccessFactory("Error message in case of abnormal termination", () => new OptionType(BuiltinType.String),
                    typeof(MainframeEvents.ProcessStopped), typeof(MainframeEvents.ProcessStopped).GetProperty("Error"))
            }
        });
    private static List<BoundType>? messageBusTypes;

    internal static IEnumerable<BoundType> MessageBusTypes {
        get {
            if (messageBusTypes == null) {
                messageBusTypes = [SubscriptionType, EventProcessStartedType, EventProcessStoppedType];
                foreach (var type in messageBusTypes) {
                    BindingGenerator.RegisterTypeMapping(type.runtimeType, type);
                }
            }

            return messageBusTypes;
        }
    }
}
