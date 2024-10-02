using Application.Helpers;
using Application.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DtoFactory
    {
        public static BaseDTO CreateDto(string prefix)
        {
            switch (prefix)
            {
                case Constant.PREFIX_STUDENT:
                    return new StudentDTO(); // Instantiate StaffDto
                case Constant.PREFIX_STAFF:
                    return new StaffDTO
                    {
                        FirstName = "",
                        LastName = "",
                        MiddleName = "",
                        Gender = "",
                        DOB = DateTime.Now,
                        JoiningDate = DateOnly.FromDateTime(DateTime.Now),
                        Phone = "",
                        Position = "",
                        Image = "",

                    }; // Instantiate StudentDto
                default:
                    // Return null or handle the case where parts is null or empty
                    return null;
            }
        }
    }
}
