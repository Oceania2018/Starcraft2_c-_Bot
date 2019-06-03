using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2APIProtocol;

namespace Bot
{
    internal class ProtBot : Bot
    {
        public IEnumerable<SC2APIProtocol.Action> OnFrame()
        {
            Controller.OpenFrame();

            List<Unit> resourceCenters = Controller.GetUnits(Units.ResourceCenters);

            foreach(var rc in resourceCenters)
            {
                if (Controller.CanConstruct(Units.PROBE))
                {
                    rc.Train(Units.PROBE);
                }
            }

            if(Controller.maxSupply - Controller.currentSupply <= 5)
            {
                if (Controller.CanConstruct(Units.PYLON))
                    if (Controller.GetPendingCount(Units.PYLON) == 0)
                        Controller.Construct(Units.PYLON);
            }

            if (Controller.frame % 10 == 0)
                Controller.DistributeWorkers();

            if (Controller.CanConstruct(Units.GATEWAY))
                if (Controller.GetTotalCount(Units.GATEWAY) < 4)
                    Controller.Construct(Units.GATEWAY);

            if (Controller.CanConstruct(Units.CYBERNETICS_CORE))
                Controller.Construct(Units.CYBERNETICS_CORE);

            if(Controller.)

            if(Controller.CanConstruct(Units.ASSIMILATOR))
                

            List<Unit> units = Controller.GetUnits(Units.Structures);
            foreach(var i in units)
            {
                if(i.unitType == Units.CYBERNETICS_CORE)
                {

                }
            }

            return Controller.CloseFrame();
        }
    }
}
