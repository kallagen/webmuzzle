namespace TSensor.Web.ViewModels
{
    public class ViewModelBase
    {
        public string ErrorMessage { get; set; }
        public bool IsError =>
            !string.IsNullOrWhiteSpace(ErrorMessage);

        public string SuccessMessage { get; set; }
        public bool IsSuccess =>
            !string.IsNullOrWhiteSpace(SuccessMessage);
    }
}
