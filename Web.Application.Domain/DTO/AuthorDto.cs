using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Domain.DTO
{
    public class CreateAuthorDto
    {
        [Required(ErrorMessage = "Невалидное наименование")]
        [Display(Name = "Краткое наименование")]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Невалидное имя")]
        [Display(Name = "Имя")]
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Невалидная фамилия")]
        [Display(Name = "Фамилия")]
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Невалидное отчество")]
        [Display(Name = "Отчество")]
        [StringLength(50, MinimumLength = 3)]
        public string SurName { get; set; }
        [Range(20, 100)]
        public int DeathAge { get; set; }
    }
    public class AuthorDto
    {
        [Display(Name = "Идентификатор")]
        public int Id { get; set; }
        [Display(Name = "Краткое наименование")]
        public string Title { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        public string SurName { get; set; }
    }
}
