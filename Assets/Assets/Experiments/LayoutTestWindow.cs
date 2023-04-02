using UnityEngine;

namespace Experiments {
    public class LayoutTestWindow : UGUIResizableWindow {
        public void OnEnable() {
            Initialize("Test", new Rect(100, 700, 550, 400));

            var root = RootHorizontalLayout();
            
            var panel1 = UIFactory.Instance.CreatePanel();
            root.Add(panel1, UGUILayout.Align.STRETCH, new Vector2(30, 30));
            
            var panel2 = UIFactory.Instance.CreatePanel();
            root.Add(panel2, UGUILayout.Align.STRETCH, new Vector2(30, 30), 1f);

            var panel1a = UIFactory.Instance.CreatePanel();
            root.Add(panel1a, UGUILayout.Align.STRETCH, new Vector2(30, 30));

            var vertical = UGUILayoutContainer.Vertical();
            root.Add(vertical, UGUILayout.Align.STRETCH, 2);

            var panel3 = UIFactory.Instance.CreatePanel();
            vertical.Add(panel3, UGUILayout.Align.STRETCH, new Vector2(30, 30));

            var panel4 = UGUILayoutContainer.HorizontalPanel();
            vertical.Add(panel4, UGUILayout.Align.STRETCH, 2f);

            var panel5 = UIFactory.Instance.CreatePanel();
            panel4.Add(panel5, UGUILayout.Align.STRETCH, new Vector2(30, 30));

            var panel6 = UIFactory.Instance.CreatePanel();
            panel4.Add(panel6, UGUILayout.Align.STRETCH, new Vector2(30, 30), 1);

            MinSize = root.MinSize;
            root.Layout();
        }
    }
}
