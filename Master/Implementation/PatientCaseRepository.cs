﻿using System.Collections.Generic;
using System.Linq;
using Master.Models;

namespace Master.Implementation
{
    public static class PatientCaseRepository
    {
        private static readonly DomainDbContext _db = new DomainDbContext();

        public static PatientCase GetById(int id)
        {
            return _db.PatientCases.FirstOrDefault(x => x.Id == id);
        }

        public static void Add(PatientCase patientCase)
        {
            _db.PatientCases.Add(patientCase);
            _db.SaveChanges();
        }

        public static void Delete(int id)
        {
            var patientCase = GetById(id);
            if (patientCase != null)
            {
                _db.PatientCases.Remove(patientCase);
                _db.SaveChanges();
            }
        }

        public static List<PatientCase> GetAll()
        {
            return _db.PatientCases.ToList();
        }
    }
}