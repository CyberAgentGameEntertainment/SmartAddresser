namespace SmartAddresser.Editor.Foundation.CustomDrawers
{
    public abstract class GUIDrawer<T> : ICustomDrawer
    {
        protected T Target { get; private set; }
        
        public virtual void Setup(object target)
        {
            Target = (T)target;
        }

        public void DoLayout()
        {
            GUILayout(Target);
        }

        protected abstract void GUILayout(T target);
    }
}
