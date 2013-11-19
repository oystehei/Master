using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Master.Models;

namespace Master.Implementation
{
   public static class ClassRepository
    {
       private static readonly DomainDbContext _db = new DomainDbContext();

       public static List<ClassModel> GetAll()
       {
           return _db.ClassModels.ToList();
       }

       public static ClassModel GetByClassId(int id)
       {
           return _db.ClassModels.FirstOrDefault(x => x.Class == id);
       }

       public static ClassModel GetByDiagnose(string diagnose)
       {
           var intiFyed = Regex.Replace(diagnose, "[^0-9.]", "");
           var diagnoseToInt = -9;
           int.TryParse(intiFyed, out diagnoseToInt);
           var result = _db.ClassModels.FirstOrDefault(x => x.ValueFrom <= diagnoseToInt && x.ValueTo >= diagnoseToInt);

           return result;
       }

    }
}
