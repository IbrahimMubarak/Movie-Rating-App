using DotNet5CRUD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5CRUD.ViewModel
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; }
        [Required, Range(1900, 2022)]
        public int Year { get; set; }
        [Range(1, 10)]
        public float Rate { get; set; }
        [Required]
        public string Storyline { get; set; }
        
        [Display(Name="Select Poster")]
        public byte[] Poster { get; set; }
        [Display(Name ="Genre")]
        public int GenreId { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }

}
