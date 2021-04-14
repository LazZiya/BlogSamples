using System.ComponentModel.DataAnnotations;

namespace GoogleReCaptchaV3
{
    public class Person
    {
        [Required, Display(Name = "Type your name")]
        public string Name { get; set; }
    }
}
