using System.Collections.Generic;

namespace Experiments {
    public class Container2D : GLUIDrawer.IGLUIDrawable{
        protected List<GLUIDrawer.IGLUIDrawable> elements = new List<GLUIDrawer.IGLUIDrawable>();

        public E Add<E>(E element) where E : GLUIDrawer.IGLUIDrawable {
            elements.Add(element);
            return element;
        }

        public void Clear() => elements.Clear();
        
        public virtual void OnDraw(GLUIDrawer.GLUIDraw draw) {
            foreach (var element in elements) {
                element.OnDraw(draw);                
            }
        }
    }
}
