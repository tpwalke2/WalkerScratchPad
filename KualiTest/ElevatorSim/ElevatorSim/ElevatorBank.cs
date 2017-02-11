using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSim
{
    public class ElevatorBank : IElevatorObserver
    {
        private IList<Elevator> _elevators;

        public ElevatorBank()
        {
            _elevators = new List<Elevator>();
        }

        public void initialize(int numElevators, int numFloors)
        {
            for (int i = 1; i <= numElevators; i++)
            {
                var newElevator = new Elevator(Guid.NewGuid().ToString(), numFloors);
                newElevator.AddObserver(this);
                _elevators.Add(newElevator);
            }


        }

        public void RequestElevator(int floorNum)
        {
            bool handled = false;
            foreach (Elevator e in _elevators)
            {
                if (!e.WillPass(floorNum)) { continue; }

                e.AddDestination(floorNum);
                handled = true;
                break;
            }

            if (handled) { return; }

            foreach (Elevator e in _elevators)
            {
                if (!e.IsIdle()) { continue; }

                e.AddDestination(floorNum);
                handled = true;
            }

            if (!handled)
            {
                // elevator bank cannot handle request, notify building maintenance
            }
        }

        public void TickSystem()
        {
            foreach (Elevator e in _elevators)
            {
                e.Tick();
            }
        }

        public void ElevatorClosed(Elevator e) { }
        public void ElevatorOpened(Elevator e) { }
        public void ElevatorChangedFloors(Elevator e, int floorNum) { }
    }
}
