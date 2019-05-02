
using MobileBackend.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MobileBackend.Controllers
{
    public class WorkAssignmentController : ApiController
    {
        public string[] GetAll()
        {

            string[] assignmentNames = null;
            panconDatabaseEntities entities = new panconDatabaseEntities();

            try
            {
                assignmentNames = (from wa in entities.WorkAssignments
                                 where (wa.Active == true)
                                 select wa.Title).ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return assignmentNames;
        }
    }
}
