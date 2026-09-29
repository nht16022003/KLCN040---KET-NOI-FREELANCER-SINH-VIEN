namespace FreelancerStudent.Web.ViewModels
{
    public class ApiReponse<T>
    {
        public bool success { get; set; }

        public string? message { get; set; }

        public T? data { get; set; }
    }
}