using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Models
{
   public class ClassModel
    {
       [Key]
       public int Class { get; set; }

       public int ValueFrom { get; set; }
       public int ValueTo { get; set; }
       public string Title { get; set; }
       public string Description { get; set; }

    }
}
