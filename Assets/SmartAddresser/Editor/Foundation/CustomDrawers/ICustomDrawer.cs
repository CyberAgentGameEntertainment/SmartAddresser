namespace SmartAddresser.Editor.Foundation.CustomDrawers
{
    public interface ICustomDrawer
    {
        void Setup(object target);
        
        void DoLayout();
    }
}
