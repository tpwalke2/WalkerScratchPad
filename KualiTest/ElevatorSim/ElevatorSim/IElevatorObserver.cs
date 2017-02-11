using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim
{
    public interface IElevatorObserver
    {
        void ElevatorClosed(Elevator e);
        void ElevatorOpened(Elevator e);
        void ElevatorChangedFloors(Elevator e, int floorNum);
    }
}
