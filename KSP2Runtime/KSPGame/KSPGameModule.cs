using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Game;
using UnityEngine;
using UnityEngine.Playables;

namespace KontrolSystem.KSP.Runtime.KSPGame;

[KSModule("ksp::game",
    Description = "Collection to game and runtime related functions."
)]
public partial class KSPGameModule {
    [KSConstant("MAINFRAME", Description = "KontrolSystem mainframe")]
    public static readonly MainframeAdapter MainframeInstance = new();

    [KSConstant("MESSAGE_BUS", Description = "Shared message bus")]
    public static readonly MessageBusAdapter MessageBusInstance = new();

    [KSFunction(
        Description = "Get the current universal time (UT) in seconds from start."
    )]
    public static double CurrentTime() {
        return KSPContext.CurrentContext.UniversalTime;
    }

    [KSFunction(
        Description = "Yield execution to allow Unity to do some other stuff inbetween."
    )]
    public static Future<object?> Yield() {
        KSPContext.CurrentContext.NextYield = new WaitForFixedUpdate();
        return new Future.Success<object?>(null);
    }

    [KSFunction(
        Description = "Stop execution of given number of seconds (factions of a seconds are supported as well)."
    )]
    public static Future<object?> Sleep(double seconds) {
        KSPContext.CurrentContext.NextYield = new WaitForSeconds((float)seconds);
        return new Future.Success<object?>(null);
    }

    [KSFunction(
        Description = "Stop execution until a given condition is met."
    )]
    public static Future<object?> WaitUntil(Func<bool> predicate) {
        var context = KSPContext.CurrentContext;
        KSPContext.CurrentContext.NextYield = new WaitUntil(() => {
            try {
                ContextHolder.CurrentContext.Value = context;
                context.ResetTimeout();
                return predicate();
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        });
        return new Future.Success<object?>(null);
    }

    [KSFunction(
        Description = "Stop execution as long as a given condition is met."
    )]
    public static Future<object?> WaitWhile(Func<bool> predicate) {
        var context = KSPContext.CurrentContext;
        KSPContext.CurrentContext.NextYield = new WaitWhile(() => {
            try {
                ContextHolder.CurrentContext.Value = context;
                context.ResetTimeout();
                return predicate();
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        });
        return new Future.Success<object?>(null);
    }

    [KSFunction(Description = "Show an alert notification")]
    public static void NotificationAlert(string title, string message, NotificationImportance importance = NotificationImportance.Low, double duration = 3) {
        var game = KSPContext.CurrentContext.Game;
        var data = new NotificationData();
        data.Tier = NotificationTier.Alert;
        data.Importance = importance;
        data.AlertTitle.Icon = game.UI.NotificationWarningIcon;
        data.AlertTitle.LocKey = title.Replace('/', ' '); // Disable '/' to avoid accidental clash with existing localization keys
        data.FirstLine.Icon = game.UI.NotificationRocketIcon;
        data.FirstLine.LocKey = message.Replace('/', ' ');
        data.TimerDuration = (float)duration;
        data.IsTimerActive = duration > 0;
        data.TimeStamp = game.UniverseModel.UniverseTime;
        KSPContext.CurrentContext.Game.Notifications.ProcessNotification(data);
    }

    [KSFunction(Description = "Show a passive notification")]
    public static void NotificationPassive(string message, double duration = 3) {
        var game = KSPContext.CurrentContext.Game;
        var data = new NotificationData();
        data.Tier = NotificationTier.Passive;
        data.Primary.LocKey = message.Replace('/', ' '); // Disable '/' to avoid accidental clash with existing localization keys
        data.TimerDuration = (float)duration;
        data.IsTimerActive = duration > 0;
        data.TimeStamp = game.UniverseModel.UniverseTime;
        KSPContext.CurrentContext.Game.Notifications.ProcessNotification(data);
    }

    public static (IEnumerable<RealizedType>, IEnumerable<IKontrolConstant>) DirectBindings() {
        var (enumTypes, enumConstants) = BindingGenerator.RegisterEnumTypeMappings("ksp::game",
            new[] {
                ("Importance", "Importance of a notification", typeof(NotificationImportance),
                    new (Enum value, string description)[] {
                        (NotificationImportance.Low, "Low"),
                        (NotificationImportance.Medium, "Medium"),
                        (NotificationImportance.High, "High")
                    }),
                });

        return (enumTypes.Concat(MessageBusBinding.MessageBusTypes), enumConstants);
    }
}
