using UnityEngine;

namespace KontrolSystem.SpaceWarpMod.UI {
    public class CommonStyles {
        public readonly GUISkin baseSkin;
        public readonly GUISkin panelSkin;
        public readonly GUIStyle headingLabelStyle;

        public CommonStyles(GUISkin baseSkin, GUISkin skin) {
            this.baseSkin = baseSkin;
            panelSkin = skin;

            panelSkin.window = new GUIStyle(skin.window);
            panelSkin.box.fontSize = 11;
            panelSkin.box.padding = new RectOffset(5, 3, 3, 5);
            panelSkin.box.margin = new RectOffset(1, 1, 1, 1);
            panelSkin.label.fontSize = 11;
            panelSkin.textField.fontSize = 11;
            panelSkin.textField.padding = new RectOffset(0, 0, 0, 0);
            panelSkin.textField.margin = new RectOffset(1, 1, 1, 1);
            panelSkin.textArea.fontSize = 11;
            panelSkin.textArea.padding = new RectOffset(0, 0, 0, 0);
            panelSkin.textArea.margin = new RectOffset(1, 1, 1, 1);
            panelSkin.toggle.fontSize = 10;
            panelSkin.button.fontSize = 11;

            headingLabelStyle = new GUIStyle(panelSkin.label) {
                fontSize = 13,
                padding = new RectOffset(2, 2, 2, 2)
            };
        }
    }
}
