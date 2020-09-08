using System.Collections.Generic;
using System.Linq;

namespace TSensor.FakeSensor
{
    public class Coordinates
    {
        public double lon { get; private set; }
        public double lat { get; private set; }        

        private Coordinates Clone(Coordinates delta = null)
        {
            return new Coordinates
            {
                lon = this.lon + (delta?.lon ?? 0),
                lat = this.lat + (delta?.lat ?? 0)                
            };
        }

        private static readonly Coordinates Start =
            new Coordinates { lon = 30.309684, lat = 59.939871 };

        private static Coordinates current = null;
        private static int idx = 0;

        public static Coordinates Next
        {
            get
            {
                if (current == null || idx == Delta.Count())
                {
                    current = Start.Clone();
                    idx = 0;
                }
                else
                {
                    current = current.Clone(Delta[idx++]);
                }

                return current;
            }
        }

        private static readonly List<Coordinates> Delta = new[]
        {
            new Coordinates { lon = -0.001028, lat = -0.000395 },
            new Coordinates { lon = -0.000605, lat = -0.000304 },
            new Coordinates { lon = -0.001301, lat = -0.000379 },
            new Coordinates { lon = -0.000847, lat = -0.000455 },
            new Coordinates { lon = -0.001391, lat = -0.000349 },
            new Coordinates { lon = -0.000575, lat = -0.000349 },
            new Coordinates { lon = -0.001361, lat = -0.000440 },
            new Coordinates { lon = -0.000877, lat = -0.000243 },
            new Coordinates { lon = -0.000605, lat = -0.000273 },
            new Coordinates { lon = 0.000333, lat = -0.000577 },
            new Coordinates { lon = 0.000756, lat = -0.000425 },
            new Coordinates { lon = 0.000605, lat = -0.000395 },
            new Coordinates { lon = 0.000575, lat = -0.000486 },
            new Coordinates { lon = 0.000514, lat = -0.000455 },
            new Coordinates { lon = 0.000817, lat = -0.000319 },
            new Coordinates { lon = 0.000696, lat = -0.000516 },
            new Coordinates { lon = 0.000907, lat = -0.000288 },
            new Coordinates { lon = 0.001422, lat = 0.000607 },
            new Coordinates { lon = 0.001694, lat = 0.000531 },
            new Coordinates { lon = 0.001089, lat = 0.000387 },
            new Coordinates { lon = 0.001180, lat = 0.000304 },
            new Coordinates { lon = 0.000726, lat = -0.000167 },
            new Coordinates { lon = 0.000726, lat = -0.000471 },
            new Coordinates { lon = 0.000847, lat = -0.000106 },
            new Coordinates { lon = 0.000968, lat = 0.000288 },
            new Coordinates { lon = 0.000877, lat = 0.000319 },
            new Coordinates { lon = 0.000998, lat = 0.000440 },
            new Coordinates { lon = 0.000302, lat = 0.000698 },
            new Coordinates { lon = -0.000212, lat = 0.000395 },
            new Coordinates { lon = -0.000998, lat = 0.000273 },
            new Coordinates { lon = -0.001149, lat = -0.000243 },
            new Coordinates { lon = -0.000938, lat = -0.000486 },
            new Coordinates { lon = -0.000726, lat = -0.000213 },
            new Coordinates { lon = -0.001240, lat = -0.000501 },
            new Coordinates { lon = -0.000665, lat = 0.000501 },
            new Coordinates { lon = -0.000726, lat = 0.000364 },
            new Coordinates { lon = -0.000363, lat = 0.000334 },
            new Coordinates { lon = 0.001059, lat = 0.000455 },
            new Coordinates { lon = 0.001240, lat = 0.000288 },
            new Coordinates { lon = 0.000151, lat = 0.000562 },
            new Coordinates { lon = -0.000272, lat = 0.000668 },
            new Coordinates { lon = -0.000544, lat = 0.000349 },
            new Coordinates { lon = -0.000665, lat = 0.000349 },
            new Coordinates { lon = -0.000575, lat = 0.000364 }
        }.ToList();        
    }
}
