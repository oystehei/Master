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
          // var intiFyed = Regex.Replace(diagnose, "[^0-9]", "");
           var intiFyed = diagnose.Substring(0, 3);
           var diagnoseToInt = -9;
           int.TryParse(intiFyed, out diagnoseToInt);
           var result = _db.ClassModels.FirstOrDefault(x => x.ValueFrom <= diagnoseToInt && x.ValueTo >= diagnoseToInt);

           return result;
       }

       public static void AddWithList(string[] list)
       {
           for (var i = 0; i < list.Length; i++)
           {
               try
               {
               }
               catch (Exception)
               {


               }
               
               var element = list[i];
             //  var splitted = element.Split('(');
               //var title = splitted[0];
               //var spillted2 = splitted[1].Split(')');
               var range = Regex.Replace(element, "[^0-9–]", "");
               var from = 0;
               var to = 0;
               if (range.Contains('–'))
               {
                 var limits = range.Split('–');
                 from = int.Parse(limits[0]);
                 to = int.Parse(limits[1]);

               }
               else
               {
                   from = int.Parse(range);
                   to = int.Parse(range);
               }
               if(from == 0 && to == 0) continue;

               _db.ClassModels.Add(new ClassModel()
               {
                   Title = element,
                   ValueFrom = from,
                   ValueTo = to,
                   Description = ""
               });
               _db.SaveChanges();
               
               }
       }

    }
}
