using System;

namespace BloodDonation
{
    public class BloodPictureItem
    {
        public double Hemocyt { get; set; }
        public double Erytrocyt { get; set; }
        public double Leukocyt { get; set; }
        public double Trombocyt { get; set; }
        public double Fibrinogen { get; set; }
        public double Protrombin { get; set; }
        public double Result1 { get; set; }
        public double Result2 { get; set; }

        public BloodPictureItem()
        {
            Random random = new Random();
            this.Erytrocyt = ((double)random.Next(250, 500)) / 1000.0;
            this.Fibrinogen = ((double)random.Next(250, 500)) / 1000.0;
            this.Hemocyt = ((double)random.Next(250, 500)) / 1000.0;
            this.Leukocyt = ((double)random.Next(250, 500)) / 1000.0;
            this.Protrombin = ((double)random.Next(250, 500)) / 1000.0;
            this.Trombocyt = ((double)random.Next(250, 500)) / 1000.0;
        }
    }
}