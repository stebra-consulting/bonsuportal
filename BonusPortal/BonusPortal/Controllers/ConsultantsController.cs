using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BonusPortal.Models;
using System.Web.Script.Serialization;

namespace BonusPortal.Controllers
{
    public class ConsultantsController : ApiController
    {

        IEnumerable<Consultant> consultants = BonsuPortal.AzureManager.LoadData();

        public IEnumerable<Consultant> GetAllConsultants()
        {
            return consultants;
        }

        [ActionName("add")]
        public HttpResponseMessage Add(Consultant item)
        {

            BonsuPortal.AzureManager.AddData(item);

            //BonsuPortal.AzureManager.EditData(item);

            var response = Request.CreateResponse<Consultant>(HttpStatusCode.Created, item);

            //string uri = Url.Link("DefaultApi", new { name = item.Name });
            //response.Headers.Location = new Uri(uri);
            return response;
        }

        [ActionName("remove")]
        public HttpResponseMessage Remove(Consultant item)
        {

            HttpResponseMessage response;

            var target = consultants.FirstOrDefault((c) => c.RowKey == item.RowKey);
            if (target == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
                return response;
            }

            BonsuPortal.AzureManager.DeleteData(item);

            response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [ActionName("edit")]
        public HttpResponseMessage Edit(Consultant item)
        {

            HttpResponseMessage response;

            var target = consultants.FirstOrDefault((c) => c.RowKey == item.RowKey);
            if (target == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
                return response;
            }

            BonsuPortal.AzureManager.EditData(item);

            response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [ActionName("array")]
        public HttpResponseMessage Array(IEnumerable<Consultant> partialConsultantsHours, [FromUri] int netIncome)
        {


            float debitPool = DebitSum(partialConsultantsHours);

            setBonuses(partialConsultantsHours, debitPool, netIncome);

            HttpResponseMessage response;

            //IEnumerable to array
            Consultant[] array = consultants.Cast<Consultant>().ToArray();

            string javaScriptArray = Serialize(array);

            response = Request.CreateResponse(HttpStatusCode.OK, javaScriptArray);

            return response;
        }

        public void setBonuses(IEnumerable<Consultant> partialConsultantsHours, float debitPool, int netIncome)
        {

            int tick = 0;
            using (var sequenceEnum = partialConsultantsHours.GetEnumerator())
            {
                while (sequenceEnum.MoveNext())
                {
                    int hoursToLog = sequenceEnum.Current.Hours;

                    Consultant current = consultants.ElementAtOrDefault(tick);

                    current.setMyBonus(debitPool, netIncome);

                    tick++;
                }
            }
        }

        public static string Serialize(object o)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(o);
        }

        private float DebitSum(IEnumerable<Consultant> partialConsultantsHours)
        {
            float debitPointsPool = 0.0f;
            int tick = 0;
            using (var sequenceEnum = partialConsultantsHours.GetEnumerator())
            {
                while (sequenceEnum.MoveNext())
                {
                    int hoursToLog = sequenceEnum.Current.Hours;

                    Consultant current = consultants.ElementAtOrDefault(tick);
                    current.Hours = hoursToLog;

                    float debitPoints = current.debitPoints();
                    debitPointsPool += debitPoints;

                    tick++;
                }
            }

            return debitPointsPool;
        }

    }
}
