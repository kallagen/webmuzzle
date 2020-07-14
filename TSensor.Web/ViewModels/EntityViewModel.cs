namespace TSensor.Web.ViewModels
{
    public class EntityViewModel<T> : ViewModelBase
    {
        public T Data { get; set; }
    }
}