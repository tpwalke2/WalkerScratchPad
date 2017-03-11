using System;
using System.Collections.Generic;
using System.Threading;

namespace ElevatorSim
{
    public class ElevatorBank : IElevatorObserver
    {
        private IList<Elevator> _elevators;
        private const int TICKTIMEOUT = 5000;
        private bool _done;

        public ElevatorBank()
        {
            _elevators = new List<Elevator>();
        }

        /**
         * Initializes the elevator bank with desired number of elevators and floors then starts the system.
         */
        public void Initialize(int numElevators, int numFloors)
        {
            for (int i = 1; i <= numElevators; i++)
            {
                var newElevator = new Elevator(Guid.NewGuid().ToString(), numFloors);
                newElevator.AddObserver(this);
                _elevators.Add(newElevator);
            }

            _done = false;

            var ticker = new Thread(new ThreadStart(this.TickSystem));
            ticker.Start();
        }

        /**
         * Handles calls made from outside of the elevator.
         */
        public void RequestElevator(int floorNum)
        {
            // first, find the first elevator that will be traveling by the requested floor
            foreach (Elevator e in _elevators)
            {
                if (!e.WillPass(floorNum)) { continue; }
                // found one that is moving, use it
                e.AddDestination(floorNum);
                return;
            }

            // next, find the first idle elevator
            foreach (Elevator e in _elevators)
            {
                if (!e.IsIdle()) { continue; }

                // found one, use it
                e.AddDestination(floorNum);
                return;
            }

            // elevator bank cannot handle this request, notify building maintenance
        }

        public void TickSystem()
        {
            while (!_done)
            {
                foreach (Elevator e in _elevators)
                {
                    e.Tick();
                }

                Thread.Sleep(TICKTIMEOUT);
            }
        }

        public void Stop()
        {
            _done = true;
        }

        public void ElevatorClosed(Elevator e) { }
        public void ElevatorOpened(Elevator e) { }
        public void ElevatorChangedFloors(Elevator e, int floorNum) { }
    }
}
