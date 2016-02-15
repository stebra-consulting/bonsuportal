using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System.Globalization;

namespace BonusPortal.Models
{
    public class Consultant : TableEntity
    {


        public string Name { get; set; }
        public string EmploymentDate { get; set; }
        public int Hours { get; set; }
        public string LoyaltyFactor { get; set; }

        public float DebitPoints { get; set; }

        public int MyBonusAsSEK { get; set; }

        public Consultant()
        {
            
            PartitionKey = BonsuPortal.AzureManager.partitionKey;
            RowKey = Guid.NewGuid().ToString().Substring(0, 8); //3d094c0f

            
        }

        public float loyalty()
        {

            DateTime now = DateTime.Now;

            DateTime employmentDate = Convert.ToDateTime(EmploymentDate);

            string name = Name;

            DateTime zeroTime = new DateTime(1, 1, 1);

            TimeSpan span = now - employmentDate;
            // because we start at year 1 for the Gregorian 
            // calendar, we must subtract a year here.
            int years = (zeroTime + span).Year - 1;

            float factor = 1.0f;

            if (years <= 1)
            {
                return factor;
            }
            else if (years <= 5)
            {
                int baseValue = 1;

                double fraction = (double)years / 10;
                factor = (float)(baseValue + fraction);

                return factor;
            }
            factor = 1.5f;

            return factor;
        }
        public float debitPoints() {

            float loyaltyFactorAsFloat = float.Parse(LoyaltyFactor, CultureInfo.InvariantCulture.NumberFormat);
            float myDebitPoints = Hours * loyaltyFactorAsFloat;

            DebitPoints = myDebitPoints;

            return myDebitPoints;
        }

        public void setMyBonus(float debitPool, int netIncome)
        {

            double bonusFactor = (double)DebitPoints / debitPool;

            double bonusPool = Math.Floor((double)netIncome / 20);

            float roundedDown = (float)Math.Floor(bonusFactor * bonusPool);

            int bonusAsSEK = (int)roundedDown;

            MyBonusAsSEK = bonusAsSEK;
            //Konsultens andel av bonuspotten Bk = myDebitPoints / debitPool
        }


    }
}